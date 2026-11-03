using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.UI;

namespace PlantMaster
{
    public partial class AttendanceReport : Page
    {
        private readonly string strConnString =
            ConfigurationManager
                .ConnectionStrings["PlantMasterDB"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnsureAttendanceContext())
                return;

            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtInTime.Text = "";
                txtOutTime.Text = "";
                lblMessage.Text = "";

                string id = Request.QueryString["AttendanceID"];

                if (!string.IsNullOrWhiteSpace(id))
                    LoadAttendance(id);
            }
        }

        private bool EnsureAttendanceContext()
        {
            bool isLoggedIn =
                Session["CommonLogin"] != null &&
                Convert.ToString(Session["CommonLogin"])
                    .Equals("True", StringComparison.OrdinalIgnoreCase);

            int userID;

            if (!isLoggedIn ||
                !int.TryParse(Convert.ToString(Session["UserID"]), out userID) ||
                userID <= 0)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            string location =
                Convert.ToString(Session["SelectedLocation"]).Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            try
            {
                GetSelectedLocation();
            }
            catch (InvalidOperationException)
            {
                Session.Remove("SelectedLocation");
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            if (Session["PlantID"] != null)
            {
                int existingPlantID;

                if (int.TryParse(
                    Convert.ToString(Session["PlantID"]),
                    out existingPlantID) && existingPlantID > 0)
                {
                    return true;
                }

                Session.Remove("PlantID");
            }

            try
            {
                string query = @"
                    SELECT DISTINCT PM.PlantID
                    FROM dbo.PlantMaster PM
                    INNER JOIN dbo.PlantLocationMapping PLM
                        ON PLM.PlantRecordID = PM.PlantRecordID
                    WHERE PLM.Location = @Location
                      AND PM.IsActive = 1
                      AND PM.PlantID IS NOT NULL;";

                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@Location", SqlDbType.VarChar, 50)
                        .Value = location;

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowMessage(
                                "No active plant is mapped to the selected location.");
                            return false;
                        }

                        int plantID = Convert.ToInt32(reader["PlantID"]);

                        if (reader.Read())
                        {
                            ShowMessage(
                                "Multiple plants are mapped to this location. Please check the plant mapping.");
                            return false;
                        }

                        Session["PlantID"] = plantID;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "ATTENDANCE CONTEXT ERROR: " + ex);

                ShowMessage(
                    "Unable to identify the plant for the selected location.");
                return false;
            }
        }

        private void LoadAttendance(string id)
        {
            int attendanceID;

            if (!int.TryParse(id, out attendanceID) || attendanceID <= 0)
            {
                ShowMessage("Invalid Attendance ID.");
                btnSave.Enabled = false;
                return;
            }

            try
            {
                string query = @"
                    SELECT AttendanceDate, PersonName,
                           InPhotoPath, InTime,
                           OutPhotoPath, OutTime
                    FROM dbo.AttendanceReport
                    WHERE AttendanceID = @AttendanceID
                      AND PlantID = @PlantID
                      AND LocationCode = @LocationCode;";

                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@AttendanceID", SqlDbType.Int)
                        .Value = attendanceID;

                    cmd.Parameters.Add("@PlantID", SqlDbType.Int)
                        .Value = Convert.ToInt32(Session["PlantID"]);

                    cmd.Parameters.Add("@LocationCode", SqlDbType.VarChar, 20)
                        .Value = GetSelectedLocation();

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            ShowMessage(
                                "Attendance record not found for this location.");
                            btnSave.Enabled = false;
                            return;
                        }

                        txtDate.Text =
                            Convert.ToDateTime(dr["AttendanceDate"])
                                .ToString("yyyy-MM-dd");

                        txtName.Text = Convert.ToString(dr["PersonName"]);

                        txtInTime.Text = dr["InTime"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["InTime"])
                                .ToString("HH:mm");

                        txtOutTime.Text = dr["OutTime"] == DBNull.Value
                            ? ""
                            : Convert.ToDateTime(dr["OutTime"])
                                .ToString("HH:mm");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "LOAD ATTENDANCE ERROR: " + ex);

                ShowMessage("Unable to load attendance record.");
                btnSave.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!EnsureAttendanceContext())
                return;

            if (string.IsNullOrWhiteSpace(txtDate.Text))
            {
                ShowMessage("Please select Date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowMessage("Please enter Name.");
                return;
            }

            DateTime attendanceDate;

            if (!DateTime.TryParseExact(
                txtDate.Text.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out attendanceDate))
            {
                ShowMessage("Please enter a valid Date.");
                return;
            }

            bool hasInPhoto = fuInPhoto.HasFile;
            bool hasOutPhoto = fuOutPhoto.HasFile;

            int plantID = Convert.ToInt32(Session["PlantID"]);
            string locationCode;

            try
            {
                locationCode = GetSelectedLocation();
            }
            catch (InvalidOperationException)
            {
                ShowMessage("Please select a valid location again.");
                return;
            }

            int attendanceID = 0;
            string id = Request.QueryString["AttendanceID"];

            if (!string.IsNullOrWhiteSpace(id) &&
                (!int.TryParse(id, out attendanceID) || attendanceID <= 0))
            {
                ShowMessage("Invalid Attendance ID.");
                return;
            }

            if (attendanceID == 0 && !hasInPhoto && !hasOutPhoto)
            {
                ShowMessage("Please upload an In Photo or an Out Photo.");
                return;
            }

            string inPhotoPath = null;
            string outPhotoPath = null;
            string savedInPhysicalPath = null;
            string savedOutPhysicalPath = null;

            TimeSpan? inTime = hasInPhoto
                ? (TimeSpan?)DateTime.Now.TimeOfDay
                : null;

            TimeSpan? outTime = hasOutPhoto
                ? (TimeSpan?)DateTime.Now.TimeOfDay
                : null;

            try
            {
                if (hasInPhoto)
                {
                    inPhotoPath = SavePhoto(
                        fuInPhoto, out savedInPhysicalPath);

                    if (inPhotoPath == null)
                        return;
                }

                if (hasOutPhoto)
                {
                    outPhotoPath = SavePhoto(
                        fuOutPhoto, out savedOutPhysicalPath);

                    if (outPhotoPath == null)
                    {
                        DeleteFileIfExists(savedInPhysicalPath);
                        return;
                    }
                }

                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            int rows = 0;

                            if (attendanceID > 0)
                            {
                                string updateQuery = @"
                                    UPDATE dbo.AttendanceReport
                                    SET AttendanceDate = @Date,
                                        PersonName = @Name,
                                        InPhotoPath =
                                            CASE WHEN @HasInPhoto = 1
                                                 THEN @InPhotoPath
                                                 ELSE InPhotoPath END,
                                        InTime =
                                            CASE WHEN @HasInPhoto = 1
                                                 THEN @InTime
                                                 ELSE InTime END,
                                        OutPhotoPath =
                                            CASE WHEN @HasOutPhoto = 1
                                                 THEN @OutPhotoPath
                                                 ELSE OutPhotoPath END,
                                        OutTime =
                                            CASE WHEN @HasOutPhoto = 1
                                                 THEN @OutTime
                                                 ELSE OutTime END
                                    WHERE AttendanceID = @AttendanceID
                                      AND PlantID = @PlantID
                                      AND LocationCode = @LocationCode;";

                                using (SqlCommand cmd =
                                    new SqlCommand(updateQuery, con, tran))
                                {
                                    AddAttendanceParameters(
                                        cmd, attendanceDate,
                                        txtName.Text.Trim(),
                                        inPhotoPath, inTime,
                                        outPhotoPath, outTime,
                                        hasInPhoto, hasOutPhoto,
                                        plantID, locationCode);

                                    cmd.Parameters.Add(
                                        "@AttendanceID", SqlDbType.Int)
                                        .Value = attendanceID;

                                    rows = cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                int existingID = 0;

                                string findQuery = @"
                                    SELECT TOP (1) AttendanceID
                                    FROM dbo.AttendanceReport
                                         WITH (UPDLOCK, HOLDLOCK)
                                    WHERE AttendanceDate = @Date
                                      AND PersonName = @Name
                                      AND PlantID = @PlantID
                                      AND LocationCode = @LocationCode
                                    ORDER BY
                                      CASE
                                        WHEN @HasOutPhoto = 1
                                             AND InPhotoPath IS NOT NULL
                                             AND OutPhotoPath IS NULL
                                            THEN 0
                                        WHEN @HasInPhoto = 1
                                             AND InPhotoPath IS NULL
                                            THEN 0
                                        WHEN InPhotoPath IS NOT NULL
                                            THEN 1
                                        ELSE 2
                                      END,
                                      AttendanceID DESC;";

                                using (SqlCommand findCmd =
                                    new SqlCommand(findQuery, con, tran))
                                {
                                    findCmd.Parameters.Add(
                                        "@Date", SqlDbType.Date)
                                        .Value = attendanceDate.Date;

                                    findCmd.Parameters.Add(
                                        "@Name", SqlDbType.NVarChar, 200)
                                        .Value = txtName.Text.Trim();

                                    findCmd.Parameters.Add(
                                        "@PlantID", SqlDbType.Int)
                                        .Value = plantID;

                                    findCmd.Parameters.Add(
                                        "@LocationCode", SqlDbType.VarChar, 20)
                                        .Value = locationCode;

                                    findCmd.Parameters.Add(
                                        "@HasInPhoto", SqlDbType.Bit)
                                        .Value = hasInPhoto;

                                    findCmd.Parameters.Add(
                                        "@HasOutPhoto", SqlDbType.Bit)
                                        .Value = hasOutPhoto;

                                    object result = findCmd.ExecuteScalar();

                                    if (result != null &&
                                        result != DBNull.Value)
                                    {
                                        existingID = Convert.ToInt32(result);
                                    }
                                }

                                if (existingID > 0)
                                {
                                    string updateExistingQuery = @"
                                        UPDATE dbo.AttendanceReport
                                        SET InPhotoPath =
                                            CASE WHEN @HasInPhoto = 1
                                                 THEN @InPhotoPath
                                                 ELSE InPhotoPath END,
                                            InTime =
                                            CASE WHEN @HasInPhoto = 1
                                                 THEN @InTime
                                                 ELSE InTime END,
                                            OutPhotoPath =
                                            CASE WHEN @HasOutPhoto = 1
                                                 THEN @OutPhotoPath
                                                 ELSE OutPhotoPath END,
                                            OutTime =
                                            CASE WHEN @HasOutPhoto = 1
                                                 THEN @OutTime
                                                 ELSE OutTime END
                                        WHERE AttendanceID = @AttendanceID
                                          AND PlantID = @PlantID
                                          AND LocationCode = @LocationCode;";

                                    using (SqlCommand cmd =
                                        new SqlCommand(
                                            updateExistingQuery, con, tran))
                                    {
                                        AddAttendanceParameters(
                                            cmd, attendanceDate,
                                            txtName.Text.Trim(),
                                            inPhotoPath, inTime,
                                            outPhotoPath, outTime,
                                            hasInPhoto, hasOutPhoto,
                                            plantID, locationCode);

                                        cmd.Parameters.Add(
                                            "@AttendanceID", SqlDbType.Int)
                                            .Value = existingID;

                                        rows = cmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    string insertQuery = @"
                                        INSERT INTO dbo.AttendanceReport
                                        (
                                            AttendanceDate, PersonName,
                                            InPhotoPath, InTime,
                                            OutPhotoPath, OutTime,
                                            PlantID, LocationCode
                                        )
                                        VALUES
                                        (
                                            @Date, @Name,
                                            @InPhotoPath, @InTime,
                                            @OutPhotoPath, @OutTime,
                                            @PlantID, @LocationCode
                                        );";

                                    using (SqlCommand cmd =
                                        new SqlCommand(insertQuery, con, tran))
                                    {
                                        AddAttendanceParameters(
                                            cmd, attendanceDate,
                                            txtName.Text.Trim(),
                                            inPhotoPath, inTime,
                                            outPhotoPath, outTime,
                                            hasInPhoto, hasOutPhoto,
                                            plantID, locationCode);

                                        rows = cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            if (rows > 0)
                            {
                                tran.Commit();

                                Response.Redirect(
                                    "~/AttendanceList.aspx", false);

                                Context.ApplicationInstance.CompleteRequest();
                                return;
                            }

                            tran.Rollback();

                            DeleteFileIfExists(savedInPhysicalPath);
                            DeleteFileIfExists(savedOutPhysicalPath);

                            ShowMessage(
                                "Record was not found or attendance was not saved.");
                        }
                        catch
                        {
                            try
                            {
                                tran.Rollback();
                            }
                            catch
                            {
                                // Ignore rollback errors.
                            }

                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "SAVE ATTENDANCE ERROR: " + ex);

                DeleteFileIfExists(savedInPhysicalPath);
                DeleteFileIfExists(savedOutPhysicalPath);

                ShowMessage(
                    "Unable to save attendance. Please check the database columns and photo folder.");
            }
        }

        private void AddAttendanceParameters(
            SqlCommand cmd,
            DateTime attendanceDate,
            string personName,
            string inPhotoPath,
            TimeSpan? inTime,
            string outPhotoPath,
            TimeSpan? outTime,
            bool hasInPhoto,
            bool hasOutPhoto,
            int plantID,
            string locationCode)
        {
            cmd.Parameters.Add("@Date", SqlDbType.Date)
                .Value = attendanceDate.Date;

            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200)
                .Value = personName;

            cmd.Parameters.Add("@InPhotoPath", SqlDbType.NVarChar, 500)
                .Value = (object)inPhotoPath ?? DBNull.Value;

            cmd.Parameters.Add("@InTime", SqlDbType.Time)
                .Value = (object)inTime ?? DBNull.Value;

            cmd.Parameters.Add("@OutPhotoPath", SqlDbType.NVarChar, 500)
                .Value = (object)outPhotoPath ?? DBNull.Value;

            cmd.Parameters.Add("@OutTime", SqlDbType.Time)
                .Value = (object)outTime ?? DBNull.Value;

            cmd.Parameters.Add("@HasInPhoto", SqlDbType.Bit)
                .Value = hasInPhoto;

            cmd.Parameters.Add("@HasOutPhoto", SqlDbType.Bit)
                .Value = hasOutPhoto;

            cmd.Parameters.Add("@PlantID", SqlDbType.Int)
                .Value = plantID;

            cmd.Parameters.Add("@LocationCode", SqlDbType.VarChar, 20)
                .Value = locationCode;
        }

    private bool TryParseTime(string value, out TimeSpan? time)
{
    time = null;

    if (string.IsNullOrWhiteSpace(value))
        return true;

    TimeSpan parsed;

    if (!TimeSpan.TryParseExact(
        value.Trim(),
        new[] { @"hh\:mm", @"h\:mm" },
        CultureInfo.InvariantCulture,
        out parsed))
    {
        return false;
    }

    if (parsed < TimeSpan.Zero ||
        parsed >= TimeSpan.FromDays(1))
    {
        return false;
    }

    time = parsed;
    return true;
}

private string SavePhoto(
    System.Web.UI.WebControls.FileUpload upload,
    out string savedPhysicalPath)
{
    savedPhysicalPath = null;

    string extension =
        Path.GetExtension(upload.FileName).ToLowerInvariant();

    if (extension != ".jpg" &&
        extension != ".jpeg" &&
        extension != ".png")
    {
        ShowMessage(
            "Only JPG, JPEG and PNG photos are allowed.");
        return null;
    }

    if (upload.PostedFile.ContentLength > 5 * 1024 * 1024)
    {
        ShowMessage(
            "Each photo must be 5 MB or less.");
        return null;
    }

    string folderPath =
        Server.MapPath("~/AttendancePhotos/");

    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);

    string fileName =
        Guid.NewGuid().ToString("N") + extension;

    savedPhysicalPath =
        Path.Combine(folderPath, fileName);

    upload.SaveAs(savedPhysicalPath);

    return "~/AttendancePhotos/" + fileName;
}

private void DeleteFileIfExists(string path)
{
    if (string.IsNullOrWhiteSpace(path) ||
        !File.Exists(path))
    {
        return;
    }

    try
    {
        File.Delete(path);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine(
            "PHOTO CLEANUP ERROR: " + ex);
    }
}

protected void btnClear_Click(object sender, EventArgs e)
{
    txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
    txtName.Text = "";
    txtInTime.Text = "";
    txtOutTime.Text = "";
    lblMessage.Text = "";
    btnSave.Enabled = true;
}

protected void btnBack_Click(object sender, EventArgs e)
{
    if (!EnsureAttendanceContext())
        return;

    Response.Redirect("~/AttendanceList.aspx", false);
    Context.ApplicationInstance.CompleteRequest();
}

private string GetSelectedLocation()
{
    string location =
        Convert.ToString(Session["SelectedLocation"]).Trim();

    int locationNumber;

    if (!location.StartsWith(
            "Location",
            StringComparison.OrdinalIgnoreCase) ||
        !int.TryParse(
            location.Substring("Location".Length),
            out locationNumber) ||
        locationNumber < 1 ||
        locationNumber > 10)
    {
        throw new InvalidOperationException(
            "Please select a valid location from Location1 to Location10.");
    }

    return "Location" + locationNumber;
}

private void ShowMessage(string message)
{
    lblMessage.Text = Server.HtmlEncode(message);
    lblMessage.ForeColor = System.Drawing.Color.Red;
}
    }
}
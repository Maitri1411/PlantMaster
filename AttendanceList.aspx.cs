
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class AttendanceList : Page
    {
        private readonly string strConnString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

            // Check permission before loading Attendance List
            if (!ScreenAccess.Check(this, "AttendanceList"))
                return;


            bool isLoggedIn =
                Session["CommonLogin"] != null &&
                Convert.ToString(Session["CommonLogin"])
                    .Equals("True", StringComparison.OrdinalIgnoreCase) &&
                Session["UserID"] != null;

            if (!isLoggedIn)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Session["SelectedLocation"] == null)
            {
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadAttendanceList("");
            }
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
                    "Invalid selected location. Please select the location again.");
            }

            return "Location" + locationNumber;
        }

        private int GetPlantID(string location)
        {
            int plantID;

            // Use the session PlantID if it is valid.
            if (Session["PlantID"] != null &&
                int.TryParse(
                    Convert.ToString(Session["PlantID"]),
                    out plantID) &&
                plantID > 0)
            {
                return plantID;
            }

            // Otherwise, identify the plant using the selected location.
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
                cmd.Parameters.Add(
                    "@Location",
                    SqlDbType.VarChar,
                    50).Value = location;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "No active plant is mapped to the selected location.");
                    }

                    plantID = Convert.ToInt32(reader["PlantID"]);

                    if (reader.Read())
                    {
                        throw new InvalidOperationException(
                            "Multiple plants are mapped to this location. Please check the plant mapping.");
                    }
                }
            }

            Session["PlantID"] = plantID;
            return plantID;
        }

        private void LoadAttendanceList(string searchText)
        {
            DataTable dt = new DataTable();

            try
            {
                string selectedLocation = GetSelectedLocation();
                int plantID = GetPlantID(selectedLocation);

                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand(
                    "dbo.usp_GetAttendanceList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(
                        "@PlantID",
                        SqlDbType.Int).Value = plantID;

                    cmd.Parameters.Add(
                        "@LocationCode",
                        SqlDbType.VarChar,
                        20).Value = selectedLocation;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // Search by name or date.
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    DataTable filteredTable = dt.Clone();
                    string search = searchText.Trim();

                    foreach (DataRow row in dt.Rows)
                    {
                        string personName =
                            Convert.ToString(row["PersonName"]);

                        string dateText = "";

                        if (row["AttendanceDate"] != DBNull.Value)
                        {
                            DateTime date =
                                Convert.ToDateTime(row["AttendanceDate"]);

                            dateText =
                                date.ToString("dd-MM-yyyy") + " " +
                                date.ToString("yyyy-MM-dd");
                        }

                        if (personName.IndexOf(
                                search,
                                StringComparison.OrdinalIgnoreCase) >= 0 ||
                            dateText.IndexOf(
                                search,
                                StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            filteredTable.ImportRow(row);
                        }
                    }

                    dt = filteredTable;
                }

                gvAttendance.DataSource = dt;
                gvAttendance.DataBind();

                if (dt.Rows.Count == 0)
                {
                    gvAttendance.EmptyDataText =
                        "No attendance records found for the selected location.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "ATTENDANCE LIST ERROR: " + ex);

                gvAttendance.DataSource = null;
                gvAttendance.DataBind();

                ShowMessage(
                    "Attendance list load nahi hui. Details: " +
                    ex.Message);
            }
        }

        protected bool HasPhoto(object photoPath)
        {
            return photoPath != null &&
                   photoPath != DBNull.Value &&
                   !string.IsNullOrWhiteSpace(
                       Convert.ToString(photoPath));
        }

        protected string GetPhotoUrl(object photoPath)
        {
            if (!HasPhoto(photoPath))
            {
                return "#";
            }

            return ResolveUrl(Convert.ToString(photoPath));
        }
        protected void btnReport_Click(object sender, EventArgs e)
        {

            Response.Redirect("~/AttendancePrint.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAttendanceList(txtSearch.Text.Trim());
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/AttendanceReport.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void gvAttendance_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAttendance")
            {
                int attendanceID;

                if (int.TryParse(
                        Convert.ToString(e.CommandArgument),
                        out attendanceID) &&
                    attendanceID > 0)
                {
                    Response.Redirect(
                        "~/AttendanceReport.aspx?AttendanceID=" +
                        attendanceID,
                        false);

                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        private void ShowMessage(string message)
        {
            // Add a visible error label to the page if needed.
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}


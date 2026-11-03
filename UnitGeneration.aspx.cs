
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class UnitGeneration : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        private bool IsEditMode
        {
            get
            {
                return !string.IsNullOrWhiteSpace(
                    Request.QueryString["GenerationDate"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check user login and form permission
            if (!ScreenAccess.Check(this, "UnitGeneration"))
                return;

            if (!IsValidLocation())
            {
                Response.Redirect("~/Select.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                int sessionPlantID;
                int plantID = 0;

                TryGetSessionPlantID(out sessionPlantID);

                if (IsEditMode)
                {
                    if (!int.TryParse(
                        Request.QueryString["PlantID"], out plantID) ||
                        plantID <= 0)
                    {
                        ShowMessage("Invalid Plant ID in edit URL.");
                        return;
                    }

                    if (!IsActivePlant(plantID))
                    {
                        ShowMessage(
                            "This plant is not active or is not mapped to the selected location.");
                        return;
                    }
                }
                else if (sessionPlantID > 0)
                {
                    plantID = sessionPlantID;

                    if (!IsActivePlant(plantID))
                    {
                        ShowMessage(
                            "This plant is not active or is not mapped to the selected location.");

                        txtPlantID.Text = "";
                        return;
                    }
                }

                txtGenerationDate.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");

                if (plantID > 0)
                {
                    txtPlantID.Text = plantID.ToString();
                    LoadInverters(plantID);
                }
                else
                {
                    txtPlantID.Text = "";
                    gvInverterValues.DataSource = null;
                    gvInverterValues.DataBind();
                    txtINVTWISE.Text = "0.00";
                }

                if (IsEditMode && plantID > 0)
                    LoadExistingGeneration(plantID);
            }
        }

        private bool TryGetSessionPlantID(out int plantID)
        {
            plantID = 0;

            return Session["PlantID"] != null &&
                int.TryParse(
                    Convert.ToString(Session["PlantID"]),
                    out plantID) &&
                plantID > 0;
        }

        private string GetSelectedLocation()
        {
            string location =
                Convert.ToString(Session["SelectedLocation"]).Trim();

            int number;

            if (!location.StartsWith(
                    "Location", StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(
                    location.Substring("Location".Length), out number) ||
                number < 1 || number > 10)
            {
                throw new InvalidOperationException(
                    "Please select a valid location.");
            }

            return "Location" + number;
        }

        private bool IsValidLocation()
        {
            try
            {
                GetSelectedLocation();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryGetEnteredPlantID(out int plantID)
        {
            plantID = 0;

            return int.TryParse(
                txtPlantID.Text.Trim(), out plantID) &&
                plantID > 0;
        }

        private bool IsActivePlant(int plantID)
        {
            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM dbo.PlantMaster
                WHERE PlantID = @PlantID
                  AND IsActive = 1;", con))
            {
                cmd.Parameters.Add("@PlantID", SqlDbType.Int)
                    .Value = plantID;

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Get current user's role
        private string GetCurrentUserRole()
        {
            int userID;

            if (Session["UserID"] == null ||
                !int.TryParse(
                    Convert.ToString(Session["UserID"]), out userID))
            {
                return "";
            }

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT [Role]
                FROM dbo.UserMaster
                WHERE UserID = @UserID
                  AND IsActive = 1;", con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int)
                    .Value = userID;

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? ""
                    : Convert.ToString(result).Trim();
            }
        }

        private bool IsManager()
        {
            return string.Equals(
                GetCurrentUserRole(),
                "Manager",
                StringComparison.OrdinalIgnoreCase);
        }

        // Manager -> UnitGenerationList
        // Worker -> Select
        private void NavigateAfterSave()
        {
            string destination = IsManager()
                ? "~/UnitGenerationList.aspx"
                : "~/Select.aspx";

            Response.Redirect(destination, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void LoadInverters(int plantID)
        {
            DataTable dt = new DataTable();

            const string query = @"
                SELECT InverterID
                FROM dbo.InverterMaster
                WHERE PlantID = @PlantID
                  AND IsActive = 1
                ORDER BY InverterID;";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@PlantID", SqlDbType.Int).Value = plantID;

                using (SqlDataAdapter da =
                    new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            gvInverterValues.DataSource = dt;
            gvInverterValues.DataBind();

            if (dt.Rows.Count == 0)
                ShowMessage("No active inverters found for this plant.");
        }

        protected void txtPlantID_TextChanged(
            object sender, EventArgs e)
        {
            int plantID;

            if (!TryGetEnteredPlantID(out plantID))
            {
                ShowMessage("Please enter a valid positive Plant ID.");
                ClearInverterGrid();
                return;
            }

            if (IsEditMode)
            {
                int urlPlantID = 0;

                if (!int.TryParse(
                        Request.QueryString["PlantID"], out urlPlantID) ||
                    plantID != urlPlantID)
                {
                    ShowMessage(
                        "Plant ID cannot be changed while editing an existing record.");

                    if (urlPlantID > 0)
                        txtPlantID.Text = urlPlantID.ToString();

                    return;
                }
            }

            try
            {
                if (!IsActivePlant(plantID))
                {
                    ClearInverterGrid();

                    ShowMessage(
                        "This plant is not active or is not mapped to the selected location.");

                    return;
                }

                LoadInverters(plantID);
                txtINVTWISE.Text = "0.00";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ShowMessage(
                    "Unable to load inverters. Check the database column names.");
            }
        }

        private void ClearInverterGrid()
        {
            gvInverterValues.DataSource = null;
            gvInverterValues.DataBind();
            txtINVTWISE.Text = "0.00";
        }

        private void LoadExistingGeneration(int plantID)
        {
            DateTime date;
            int urlPlantID;

            string inverterID =
                Convert.ToString(Request.QueryString["InverterID"]);

            if (!DateTime.TryParseExact(
                    Request.QueryString["GenerationDate"],
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date) ||
                !int.TryParse(
                    Request.QueryString["PlantID"], out urlPlantID) ||
                urlPlantID != plantID ||
                string.IsNullOrWhiteSpace(inverterID))
            {
                ShowMessage("Invalid Unit Generation record.");
                return;
            }

            try
            {
                DataTable existing = new DataTable();

                const string query = @"
                    SELECT InverterID, PGVCL, GETCO, Value
                    FROM dbo.UnitGeneration
                    WHERE PlantID = @PlantID
                      AND LocationCode = @LocationCode
                      AND GenerationDate >= @GenerationDate
                      AND GenerationDate < DATEADD(day, 1, @GenerationDate)
                      AND IsActive = 1
                    ORDER BY InverterID;";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@PlantID", SqlDbType.Int).Value = plantID;

                    cmd.Parameters.Add(
                        "@LocationCode", SqlDbType.VarChar, 20)
                        .Value = GetSelectedLocation();

                    cmd.Parameters.Add(
                        "@GenerationDate", SqlDbType.Date)
                        .Value = date.Date;

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        da.Fill(existing);
                    }
                }

                DataRow selectedRecord = null;

                foreach (DataRow record in existing.Rows)
                {
                    if (string.Equals(
                        Convert.ToString(record["InverterID"]),
                        inverterID,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        selectedRecord = record;
                        break;
                    }
                }

                if (selectedRecord == null)
                {
                    ShowMessage(
                        "Record not found for this plant, date and location.");
                    return;
                }

                txtGenerationDate.Text = date.ToString("yyyy-MM-dd");

                txtPGVCL.Text = Convert.ToDecimal(
                    selectedRecord["PGVCL"]).ToString(
                    CultureInfo.InvariantCulture);

                txtGETCO.Text = Convert.ToDecimal(
                    selectedRecord["GETCO"]).ToString(
                    CultureInfo.InvariantCulture);

                decimal total = 0m;

                foreach (GridViewRow gridRow in gvInverterValues.Rows)
                {
                    HiddenField hf =
                        gridRow.FindControl("hfInverterID")
                        as HiddenField;

                    TextBox txt =
                        gridRow.FindControl("txtInverterValue")
                        as TextBox;

                    if (hf == null || txt == null)
                        continue;

                    foreach (DataRow record in existing.Rows)
                    {
                        if (string.Equals(
                            Convert.ToString(record["InverterID"]),
                            hf.Value,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            decimal value =
                                Convert.ToDecimal(record["Value"]);

                            txt.Text = value.ToString(
                                CultureInfo.InvariantCulture);

                            total += value;
                            break;
                        }
                    }
                }

                txtINVTWISE.Text = total.ToString(
                    "0.00", CultureInfo.InvariantCulture);

                btnSave.Text = "Update";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ShowMessage(
                    "Unable to load the existing record. Check the database.");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Recheck form permission before saving.
            if (!ScreenAccess.Check(this, "UnitGeneration"))
                return;

            int plantID;

            if (!TryGetEnteredPlantID(out plantID))
            {
                ShowMessage("Please enter a valid Plant ID.");
                return;
            }

            if (IsEditMode)
            {
                int urlPlantID;

                if (!int.TryParse(
                        Request.QueryString["PlantID"], out urlPlantID) ||
                    urlPlantID != plantID)
                {
                    ShowMessage(
                        "Plant ID cannot be changed while editing.");
                    return;
                }
            }

            try
            {
                if (!IsActivePlant(plantID))
                {
                    ShowMessage(
                        "This plant is not active or is not mapped to the selected location.");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                ShowMessage("Unable to validate plant and location.");
                return;
            }

            DateTime date;

            if (!DateTime.TryParseExact(
                    txtGenerationDate.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
            {
                ShowMessage("Please select a valid date.");
                return;
            }

            decimal pgvcl;
            decimal getco;

            if (!TryDecimal(txtPGVCL.Text, out pgvcl) ||
                !TryDecimal(txtGETCO.Text, out getco) ||
                pgvcl < 0 || getco < 0)
            {
                ShowMessage(
                    "Enter valid non-negative PGVCL and GETCO values.");
                return;
            }

            string location;

            try
            {
                location = GetSelectedLocation();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                return;
            }

            string editID =
                Convert.ToString(Request.QueryString["InverterID"]);

            if (IsEditMode && string.IsNullOrWhiteSpace(editID))
            {
                ShowMessage("Invalid inverter selected for editing.");
                return;
            }

            DataTable values = new DataTable();
            values.Columns.Add("InverterID", typeof(string));
            values.Columns.Add("Value", typeof(decimal));

            decimal total = 0m;
            bool editInverterFound = false;

            foreach (GridViewRow row in gvInverterValues.Rows)
            {
                HiddenField hf =
                    row.FindControl("hfInverterID") as HiddenField;

                TextBox txt =
                    row.FindControl("txtInverterValue") as TextBox;

                if (hf == null || txt == null)
                {
                    ShowMessage("Unable to read inverter values.");
                    return;
                }

                string inverterID = hf.Value.Trim();
                decimal value = 0m;

                if (!string.IsNullOrWhiteSpace(txt.Text) &&
                    !TryDecimal(txt.Text, out value))
                {
                    ShowMessage(
                        "Enter a valid value for inverter " +
                        inverterID + ".");
                    return;
                }

                if (value < 0)
                {
                    ShowMessage("Inverter values cannot be negative.");
                    return;
                }

                if (IsEditMode && string.Equals(
                    inverterID, editID,
                    StringComparison.OrdinalIgnoreCase))
                {
                    editInverterFound = true;
                }

                values.Rows.Add(inverterID, value);
                total += value;
            }

            if (values.Rows.Count == 0)
            {
                ShowMessage("No active inverters found for this plant.");
                return;
            }

            if (IsEditMode && !editInverterFound)
            {
                ShowMessage(
                    "The selected inverter is not active or was not found.");
                return;
            }

            txtINVTWISE.Text = total.ToString(
                "0.00", CultureInfo.InvariantCulture);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow item in values.Rows)
                            {
                                string inverterID =
                                    Convert.ToString(item["InverterID"]);

                                if (IsEditMode && !string.Equals(
                                    inverterID, editID,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                bool exists;

                                const string checkQuery = @"
                                    SELECT COUNT(1)
                                    FROM dbo.UnitGeneration
                                    WHERE PlantID = @PlantID
                                      AND LocationCode = @LocationCode
                                      AND GenerationDate >= @GenerationDate
                                      AND GenerationDate < DATEADD(day, 1, @GenerationDate)
                                      AND InverterID = @InverterID;";

                                using (SqlCommand check =
                                    new SqlCommand(checkQuery, con, tran))
                                {
                                    check.Parameters.Add(
                                        "@PlantID", SqlDbType.Int)
                                        .Value = plantID;

                                    check.Parameters.Add(
                                        "@LocationCode", SqlDbType.VarChar, 20)
                                        .Value = location;

                                    check.Parameters.Add(
                                        "@GenerationDate", SqlDbType.Date)
                                        .Value = date.Date;

                                    check.Parameters.Add(
                                        "@InverterID", SqlDbType.VarChar, 25)
                                        .Value = inverterID;

                                    exists = Convert.ToInt32(
                                        check.ExecuteScalar()) > 0;
                                }

                                string sql = exists
                                    ? @"
                                        UPDATE dbo.UnitGeneration
                                        SET PGVCL = @PGVCL,
                                            GETCO = @GETCO,
                                            Value = @Value,
                                            INVTWISE = @INVTWISE,
                                            IsActive = 1
                                        WHERE PlantID = @PlantID
                                          AND LocationCode = @LocationCode
                                          AND GenerationDate >= @GenerationDate
                                          AND GenerationDate < DATEADD(day, 1, @GenerationDate)
                                          AND InverterID = @InverterID;"
                                    : @"
                                        INSERT INTO dbo.UnitGeneration
                                        (GenerationDate, PGVCL, GETCO, PlantID,
                                         InverterID, Value, INVTWISE, IsActive,
                                         LocationCode)
                                        VALUES
                                        (@GenerationDate, @PGVCL, @GETCO, @PlantID,
                                         @InverterID, @Value, @INVTWISE, 1,
                                         @LocationCode);";

                                using (SqlCommand cmd =
                                    new SqlCommand(sql, con, tran))
                                {
                                    cmd.Parameters.Add(
                                        "@PlantID", SqlDbType.Int)
                                        .Value = plantID;

                                    cmd.Parameters.Add(
                                        "@LocationCode", SqlDbType.VarChar, 20)
                                        .Value = location;

                                    cmd.Parameters.Add(
                                        "@GenerationDate", SqlDbType.Date)
                                        .Value = date.Date;

                                    cmd.Parameters.Add(
                                        "@InverterID", SqlDbType.VarChar, 25)
                                        .Value = inverterID;

                                    AddDecimal(cmd, "@PGVCL", pgvcl);
                                    AddDecimal(cmd, "@GETCO", getco);

                                    AddDecimal(
                                        cmd, "@Value",
                                        Convert.ToDecimal(item["Value"]));

                                    AddDecimal(cmd, "@INVTWISE", total);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            const string totalQuery = @"
                                UPDATE dbo.UnitGeneration
                                SET INVTWISE = @Total
                                WHERE PlantID = @PlantID
                                  AND LocationCode = @LocationCode
                                  AND GenerationDate >= @GenerationDate
                                  AND GenerationDate < DATEADD(day, 1, @GenerationDate);";

                            using (SqlCommand totalCmd =
                                new SqlCommand(totalQuery, con, tran))
                            {
                                AddDecimal(totalCmd, "@Total", total);

                                totalCmd.Parameters.Add(
                                    "@PlantID", SqlDbType.Int)
                                    .Value = plantID;

                                totalCmd.Parameters.Add(
                                    "@LocationCode", SqlDbType.VarChar, 20)
                                    .Value = location;

                                totalCmd.Parameters.Add(
                                    "@GenerationDate", SqlDbType.Date)
                                    .Value = date.Date;

                                totalCmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                // Redirect after successful save/update.
                NavigateAfterSave();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);

                ShowMessage(
                    "Unable to save Unit Generation. Please check the database and try again.");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            NavigateAfterSave();
        }

        private bool TryDecimal(string input, out decimal value)
        {
            return decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value);
        }

        private void AddDecimal(
            SqlCommand cmd, string name, decimal value)
        {
            SqlParameter parameter =
                cmd.Parameters.Add(name, SqlDbType.Decimal);

            parameter.Precision = 18;
            parameter.Scale = 2;
            parameter.Value = value;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = Server.HtmlEncode(message);
        }
    }
}

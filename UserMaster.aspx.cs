using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;

namespace PlantMaster
{
    public partial class UserMaster : Page
    {
        private readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["PlantMasterDB"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearFields();
            }
        }

        // ==========================================
        // SAVE USER
        // ==========================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string userName = txtUserName.Text.Trim();
            string password = txtPassword.Text;
            string role = ddlRole.SelectedValue;

            // ==========================================
            // VALIDATION
            // ==========================================
            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowMessage("Please enter First Name.", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ShowMessage("Please enter Last Name.", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                ShowMessage("Please enter User Name.", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Please enter Password.", Color.Red);
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                ShowMessage("Please select User Role.", Color.Red);
                return;
            }

            if (role != "Manager" && role != "Worker")
            {
                ShowMessage("Please select a valid User Role.", Color.Red);
                return;
            }

            List<string> selectedScreens = GetSelectedScreens();
            List<string> selectedLocations = GetSelectedLocations();

            // Login screen permission save nahi karni.
            selectedScreens.Remove("Login");

            if (selectedScreens.Count == 0)
            {
                ShowMessage(
                    "Please select at least one screen.",
                    Color.Red);
                return;
            }

            if (selectedLocations.Count == 0)
            {
                ShowMessage(
                    "Please select at least one location.",
                    Color.Red);
                return;
            }

            int userID;

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction transaction =
                        con.BeginTransaction())
                    {
                        try
                        {
                            // ==========================================
                            // 1. INSERT USER DETAILS + ROLE
                            // ==========================================
                            string insertUserQuery = @"
                                INSERT INTO dbo.UserMaster
                                (
                                    FirstName,
                                    LastName,
                                    UserName,
                                    [Password],
                                    IsActive,
                                    [Role]
                                )
                                VALUES
                                (
                                    @FirstName,
                                    @LastName,
                                    @UserName,
                                    @Password,
                                    @IsActive,
                                    @Role
                                );

                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    insertUserQuery,
                                    con,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@FirstName",
                                    SqlDbType.NVarChar,
                                    50).Value = firstName;

                                cmd.Parameters.Add(
                                    "@LastName",
                                    SqlDbType.NVarChar,
                                    50).Value = lastName;

                                cmd.Parameters.Add(
                                    "@UserName",
                                    SqlDbType.NVarChar,
                                    100).Value = userName;

                                cmd.Parameters.Add(
                                    "@Password",
                                    SqlDbType.NVarChar,
                                    100).Value = password;

                                cmd.Parameters.Add(
                                    "@IsActive",
                                    SqlDbType.Bit).Value =
                                    chkActive.Checked;

                                cmd.Parameters.Add(
                                    "@Role",
                                    SqlDbType.NVarChar,
                                    20).Value = role;

                                userID = Convert.ToInt32(
                                    cmd.ExecuteScalar());
                            }

                            // ==========================================
                            // 2. SAVE USER LOCATION MAPPING
                            // ==========================================
                            foreach (string locationName in selectedLocations)
                            {
                                string locationQuery = @"
                                    INSERT INTO dbo.UserLocationMapping
                                    (
                                        UserID,
                                        LocationCode
                                    )
                                    VALUES
                                    (
                                        @UserID,
                                        @LocationCode
                                    );";

                                using (SqlCommand cmd =
                                    new SqlCommand(
                                        locationQuery,
                                        con,
                                        transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@UserID",
                                        SqlDbType.Int).Value = userID;

                                    cmd.Parameters.Add(
                                        "@LocationCode",
                                        SqlDbType.VarChar,
                                        50).Value = locationName;

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // ==========================================
                            // 3. SAVE SCREEN PERMISSIONS
                            // ==========================================
                            foreach (string screenName in selectedScreens)
                            {
                                string permissionQuery = @"
                                    INSERT INTO dbo.UserScreenPermissions
                                    (
                                        UserID,
                                        ScreenName,
                                        IsAllowed
                                    )
                                    VALUES
                                    (
                                        @UserID,
                                        @ScreenName,
                                        1
                                    );";

                                using (SqlCommand cmd =
                                    new SqlCommand(
                                        permissionQuery,
                                        con,
                                        transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@UserID",
                                        SqlDbType.Int).Value = userID;

                                    cmd.Parameters.Add(
                                        "@ScreenName",
                                        SqlDbType.VarChar,
                                        50).Value = screenName;

                                    cmd.ExecuteNonQuery();
                                }

                                // ==========================================
                                // 4. SAVE SCREEN-WISE LOCATIONS
                                // ==========================================
                                foreach (string locationName in selectedLocations)
                                {
                                    string screenLocationQuery = @"
                                        INSERT INTO dbo.UserScreenLocation
                                        (
                                            UserID,
                                            ScreenName,
                                            LocationName
                                        )
                                        VALUES
                                        (
                                            @UserID,
                                            @ScreenName,
                                            @LocationName
                                        );";

                                    using (SqlCommand cmd =
                                        new SqlCommand(
                                            screenLocationQuery,
                                            con,
                                            transaction))
                                    {
                                        cmd.Parameters.Add(
                                            "@UserID",
                                            SqlDbType.Int).Value = userID;

                                        cmd.Parameters.Add(
                                            "@ScreenName",
                                            SqlDbType.VarChar,
                                            50).Value = screenName;

                                        cmd.Parameters.Add(
                                            "@LocationName",
                                            SqlDbType.VarChar,
                                            50).Value = locationName;

                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            // ==========================================
                            // 5. COMMIT TRANSACTION
                            // ==========================================
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                // ==========================================
                // 6. REDIRECT AFTER SUCCESSFUL SAVE
                // ==========================================
                Response.Redirect(
                    "~/UserMasterList.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "USER MASTER SAVE ERROR: " + ex.ToString());

                ShowMessage(
                    "Save failed: " + ex.Message,
                    Color.Red);
            }
        }

        // ==========================================
        // GET SELECTED SCREENS
        // ==========================================
        private List<string> GetSelectedScreens()
        {
            List<string> screens = new List<string>();

            if (chkAttendanceList.Checked)
                screens.Add("AttendanceList");

            if (chkAttendanceReport.Checked)
                screens.Add("AttendanceReport");

            if (chkInverterMaster.Checked)
                screens.Add("InverterMaster");

            if (chkLogin.Checked)
                screens.Add("Login");

            if (chkPlantMaster.Checked)
                screens.Add("PlantMaster");

            if (chkSelect.Checked)
                screens.Add("Select");

            if (chkUnitGeneration.Checked)
                screens.Add("UnitGeneration");

            if (chkUnitGenerationList.Checked)
                screens.Add("UnitGenerationList");

            if (chkWorkReport.Checked)
                screens.Add("WorkReport");

            if (chkWorkReportList.Checked)
                screens.Add("WorkReportList");

            return screens;
        }

        // ==========================================
        // GET SELECTED LOCATIONS
        // ==========================================
        private List<string> GetSelectedLocations()
        {
            List<string> locations = new List<string>();

            if (chkLocation1.Checked)
                locations.Add("Location1");

            if (chkLocation2.Checked)
                locations.Add("Location2");

            if (chkLocation3.Checked)
                locations.Add("Location3");

            if (chkLocation4.Checked)
                locations.Add("Location4");

            if (chkLocation5.Checked)
                locations.Add("Location5");

            if (chkLocation6.Checked)
                locations.Add("Location6");

            if (chkLocation7.Checked)
                locations.Add("Location7");

            if (chkLocation8.Checked)
                locations.Add("Location8");

            if (chkLocation9.Checked)
                locations.Add("Location9");

            if (chkLocation10.Checked)
                locations.Add("Location10");

            return locations;
        }

        // ==========================================
        // CLEAR FORM
        // ==========================================
        private void ClearFields()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;

            // Default role: Select Role
            if (ddlRole.Items.Count > 0)
                ddlRole.SelectedIndex = 0;

            chkActive.Checked = true;

            chkAttendanceList.Checked = false;
            chkAttendanceReport.Checked = false;
            chkInverterMaster.Checked = false;
            chkLogin.Checked = false;
            chkPlantMaster.Checked = false;
            chkSelect.Checked = false;
            chkUnitGeneration.Checked = false;
            chkUnitGenerationList.Checked = false;
            chkWorkReport.Checked = false;
            chkWorkReportList.Checked = false;

            chkLocation1.Checked = false;
            chkLocation2.Checked = false;
            chkLocation3.Checked = false;
            chkLocation4.Checked = false;
            chkLocation5.Checked = false;
            chkLocation6.Checked = false;
            chkLocation7.Checked = false;
            chkLocation8.Checked = false;
            chkLocation9.Checked = false;
            chkLocation10.Checked = false;
        }

        // ==========================================
        // SHOW MESSAGE
        // ==========================================
        private void ShowMessage(string message, Color color)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.ForeColor = color;
        }
    }
}

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.UI;

namespace PlantMaster
{
    public partial class Select : Page
    {
        private readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["PlantMasterDB"]
                .ConnectionString;

        private string currentRole = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            int userID;

            if (!TryGetUserID(out userID))
                return;

            if (!LoadUserRole(userID))
                return;

            if (!IsPostBack)
            {
               
                HidePermissionButtons();

                lblSelectedLocation.Visible = false;
                lblSelectedLocation.Text = "";
                lblMessage.Text = "";

                LoadAssignedLocation(userID);
            }
        }

        // Verify login session
        private bool TryGetUserID(out int userID)
        {
            userID = 0;

            bool isLoggedIn =
                Session["CommonLogin"] != null &&
                Convert.ToString(Session["CommonLogin"])
                    .Equals("True",
                        StringComparison.OrdinalIgnoreCase);

            if (!isLoggedIn ||
                !int.TryParse(
                    Convert.ToString(Session["UserID"]),
                    out userID) ||
                userID <= 0)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return false;
            }

            return true;
        }

        // Read the current user's role from UserMaster
        private bool LoadUserRole(int userID)
        {
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

                if (result == null || result == DBNull.Value)
                {
                    ShowMessage(
                        "Your account is inactive or has no assigned role.",
                        Color.Red);
                    HidePermissionButtons();
                    return false;
                }

                currentRole = Convert.ToString(result).Trim();
            }

            if (!currentRole.Equals(
                    "Manager",
                    StringComparison.OrdinalIgnoreCase) &&
                !currentRole.Equals(
                    "Worker",
                    StringComparison.OrdinalIgnoreCase))
            {
                ShowMessage(
                    "Your role has not been assigned. Please contact the administrator.",
                    Color.Red);

                HidePermissionButtons();
                return false;
            }

            return true;
        }

        // Find an assigned active location.
        // This page currently has no location dropdown.
        private void LoadAssignedLocation(int userID)
        {
            try
            {
                DataTable locations = new DataTable();

                string query = @"
                    SELECT DISTINCT
                        LTRIM(RTRIM(USL.LocationName)) AS LocationName
                    FROM dbo.UserScreenLocation USL
                    INNER JOIN dbo.UserScreenPermissions USP
                        ON USP.UserID = USL.UserID
                       AND USP.ScreenName = USL.ScreenName
                       AND USP.IsAllowed = 1
                    INNER JOIN dbo.PlantLocationMapping PLM
                        ON LTRIM(RTRIM(PLM.Location)) =
                           LTRIM(RTRIM(USL.LocationName))
                    INNER JOIN dbo.PlantMaster PM
                        ON PM.PlantRecordID = PLM.PlantRecordID
                    WHERE USL.UserID = @UserID
                      AND PM.IsActive = 1
                      AND USL.LocationName IS NOT NULL
                      AND LTRIM(RTRIM(USL.LocationName)) <> ''
                    ORDER BY LocationName;";

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@UserID", SqlDbType.Int)
                        .Value = userID;

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(cmd))
                    {
                        da.Fill(locations);
                    }
                }

                if (locations.Rows.Count == 0)
                {
                    ShowMessage(
                        "No active location is assigned. Please assign a location to this user.",
                        Color.Red);
                    return;
                }

                if (locations.Rows.Count > 1)
                {
                    ShowMessage(
                        "Multiple locations are assigned. A location selection option is required.",
                        Color.Red);
                    return;
                }

                string location =
                    Convert.ToString(
                        locations.Rows[0]["LocationName"]).Trim();

                if (string.IsNullOrWhiteSpace(location))
                {
                    ShowMessage(
                        "The assigned location is invalid.",
                        Color.Red);
                    return;
                }

                Session["SelectedLocation"] = location;

                lblSelectedLocation.Text =
                    "Location: " + Server.HtmlEncode(location);

                lblSelectedLocation.Visible = true;

                LoadScreenPermissions(userID, location);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "SELECT LOCATION ERROR: " + ex);

                ShowMessage(
                    "Unable to load the assigned location. Please check the database.",
                    Color.Red);
            }
        }

        // Hide all buttons initially
        private void HidePermissionButtons()
        {
            btnUnitGeneration.Visible = false;
            btnWorkReport.Visible = false;
            btnAttendanceReport.Visible = false;
        }

        // Manager gets all three screens.
        // Worker gets only assigned screens for the location.
        private void LoadScreenPermissions(
            int userID,
            string location)
        {
            HidePermissionButtons();

            if (currentRole.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
            {
                btnUnitGeneration.Visible = true;
                btnWorkReport.Visible = true;
                btnAttendanceReport.Visible = true;

                lblMessage.Text = "";
                return;
            }

            btnUnitGeneration.Visible =
                HasPermission(userID, "UnitGeneration", location);

            btnWorkReport.Visible =
                HasPermission(userID, "WorkReport", location);

            btnAttendanceReport.Visible =
                HasPermission(userID, "AttendanceReport", location);

            if (!btnUnitGeneration.Visible &&
                !btnWorkReport.Visible &&
                !btnAttendanceReport.Visible)
            {
                ShowMessage(
                    "No screens are assigned to this user for the selected location.",
                    Color.Red);
            }
            else
            {
                lblMessage.Text = "";
            }
        }

        // Verify screen and location permission for Worker
        private bool HasPermission(
            int userID,
            string screenName,
            string location)
        {
            string query = @"
                SELECT COUNT(1)
                FROM dbo.UserScreenPermissions USP
                INNER JOIN dbo.UserScreenLocation USL
                    ON USL.UserID = USP.UserID
                   AND USL.ScreenName = USP.ScreenName
                INNER JOIN dbo.PlantLocationMapping PLM
                    ON LTRIM(RTRIM(PLM.Location)) =
                       LTRIM(RTRIM(USL.LocationName))
                INNER JOIN dbo.PlantMaster PM
                    ON PM.PlantRecordID = PLM.PlantRecordID
                WHERE USP.UserID = @UserID
                  AND USP.ScreenName = @ScreenName
                  AND USP.IsAllowed = 1
                  AND LTRIM(RTRIM(USL.LocationName)) =
                      LTRIM(RTRIM(@Location))
                  AND PM.IsActive = 1;";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int)
                    .Value = userID;

                cmd.Parameters.Add("@ScreenName",
                    SqlDbType.VarChar, 50).Value = screenName;

                cmd.Parameters.Add("@Location",
                    SqlDbType.VarChar, 50).Value = location;

                con.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0;
            }
        }

        // Validate the session location before opening a screen
        private bool ValidateSelection()
        {
            int userID;

            if (!TryGetUserID(out userID))
                return false;

            if (!LoadUserRole(userID))
                return false;

            string location =
                Convert.ToString(
                    Session["SelectedLocation"]).Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                ShowMessage(
                    "No location has been selected or assigned.",
                    Color.Red);
                return false;
            }

            // Manager bypasses screen-specific permission checks,
            // but the selected location must still be active.
            if (currentRole.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
            {
                if (!IsActiveLocation(location))
                {
                    ShowMessage(
                        "The selected location is not active.",
                        Color.Red);
                    return false;
                }

                return true;
            }

            if (!IsLocationAssigned(userID, location))
            {
                Session.Remove("SelectedLocation");
                HidePermissionButtons();

                lblSelectedLocation.Visible = false;

                ShowMessage(
                    "This location is not assigned to this user.",
                    Color.Red);

                return false;
            }

            return true;
        }

        // Verify that the location is active
        private bool IsActiveLocation(string location)
        {
            string query = @"
                SELECT COUNT(1)
                FROM dbo.PlantLocationMapping PLM
                INNER JOIN dbo.PlantMaster PM
                    ON PM.PlantRecordID = PLM.PlantRecordID
                WHERE LTRIM(RTRIM(PLM.Location)) =
                      LTRIM(RTRIM(@Location))
                  AND PM.IsActive = 1;";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@Location",
                    SqlDbType.VarChar, 50).Value = location;

                con.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0;
            }
        }

        // Verify Worker location assignment
        private bool IsLocationAssigned(
            int userID,
            string location)
        {
            string query = @"
                SELECT COUNT(1)
                FROM dbo.UserScreenLocation USL
                INNER JOIN dbo.UserScreenPermissions USP
                    ON USP.UserID = USL.UserID
                   AND USP.ScreenName = USL.ScreenName
                   AND USP.IsAllowed = 1
                INNER JOIN dbo.PlantLocationMapping PLM
                    ON LTRIM(RTRIM(PLM.Location)) =
                       LTRIM(RTRIM(USL.LocationName))
                INNER JOIN dbo.PlantMaster PM
                    ON PM.PlantRecordID = PLM.PlantRecordID
                WHERE USL.UserID = @UserID
                  AND LTRIM(RTRIM(USL.LocationName)) =
                      LTRIM(RTRIM(@Location))
                  AND PM.IsActive = 1;";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int)
                    .Value = userID;

                cmd.Parameters.Add("@Location",
                    SqlDbType.VarChar, 50).Value = location;

                con.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0;
            }
        }

        // Unit Generation
        protected void btnUnitGeneration_Click(
            object sender, EventArgs e)
        {
            if (!ValidateSelection())
                return;

            if (!CheckPermission("UnitGeneration"))
                return;

            Response.Redirect(
                "~/UnitGeneration.aspx", false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // Work Report
        protected void btnWorkReport_Click(
            object sender, EventArgs e)
        {
            if (!ValidateSelection())
                return;

            if (!CheckPermission("WorkReport"))
                return;

            Response.Redirect(
                "~/WorkReport.aspx", false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // Attendance Report
        protected void btnAttendanceReport_Click(
            object sender, EventArgs e)
        {
            if (!ValidateSelection())
                return;

            if (!CheckPermission("AttendanceReport"))
                return;

            Response.Redirect(
                "~/AttendanceReport.aspx", false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // Manager can open any configured screen.
        // Worker must have permission for that screen and location.
        private bool CheckPermission(string screenName)
        {
            int userID;

            if (!TryGetUserID(out userID))
                return false;

            if (!LoadUserRole(userID))
                return false;

            string location =
                Convert.ToString(
                    Session["SelectedLocation"]).Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                ShowMessage(
                    "No location has been selected or assigned.",
                    Color.Red);
                return false;
            }

            if (currentRole.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
            {
                return IsActiveLocation(location);
            }

            if (currentRole.Equals(
                    "Worker",
                    StringComparison.OrdinalIgnoreCase) &&
                HasPermission(userID, screenName, location))
            {
                return true;
            }

            ShowMessage(
                "Access denied for this screen and location.",
                Color.Red);

            return false;
        }

        private void ShowMessage(
            string message,
            Color color)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.ForeColor = color;
        }
    }
}

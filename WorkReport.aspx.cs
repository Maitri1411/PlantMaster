
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace PlantMaster
{
    public partial class WorkReport : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ScreenAccess.Check(this, "WorkReport"))
                return;

            if (Session["SelectedLocation"] == null ||
                string.IsNullOrWhiteSpace(
                    Convert.ToString(Session["SelectedLocation"])))
            {
                Response.Redirect("~/Select.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtWorkDetails.Text = string.Empty;

                lblMessage.Text = string.Empty;
                lblHeading.Text = "Work Report";
                btnSave.Text = "Save";

                string workReportID =
                    Request.QueryString["WorkReportID"];

                if (!string.IsNullOrWhiteSpace(workReportID))
                {
                    LoadWorkReport(workReportID);
                }
            }
        }

        private string GetCurrentUserRole()
        {
            if (Session["UserID"] == null)
                return string.Empty;

            int userID;

            if (!int.TryParse(
                Convert.ToString(Session["UserID"]), out userID))
                return string.Empty;

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT [Role]
                  FROM dbo.UserMaster
                  WHERE UserID = @UserID
                    AND IsActive = 1", con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == null || result == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(result).Trim();
            }
        }

        private void NavigateAfterSave()
        {
            string role = GetCurrentUserRole();

            if (role.Equals(
                "Manager", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/WorkReportList.aspx", false);
            }
            else
            {
                Response.Redirect("~/Select.aspx", false);
            }

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void LoadWorkReport(string workReportID)
        {
            int reportID;

            if (!int.TryParse(workReportID, out reportID))
            {
                ShowError("Invalid Work Report ID.");
                return;
            }

            string selectedLocation =
                Convert.ToString(Session["SelectedLocation"]);

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT WorkDate, WorkDetails
                  FROM dbo.WorkReport
                  WHERE WorkReportID = @WorkReportID
                    AND LocationCode = @LocationCode", con))
            {
                cmd.Parameters.Add("@WorkReportID", SqlDbType.Int)
                    .Value = reportID;

                cmd.Parameters.Add("@LocationCode", SqlDbType.NVarChar, 50)
                    .Value = selectedLocation;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowError("Work Report record was not found.");
                        return;
                    }

                    if (reader["WorkDate"] != DBNull.Value)
                    {
                        txtDate.Text = Convert.ToDateTime(
                            reader["WorkDate"]).ToString("yyyy-MM-dd");
                    }

                    txtWorkDetails.Text =
                        reader["WorkDetails"] == DBNull.Value
                            ? string.Empty
                            : Convert.ToString(reader["WorkDetails"]);

                    lblHeading.Text = "Edit Work Report";
                    btnSave.Text = "Update";
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ScreenAccess.Check(this, "WorkReport"))
                return;

            lblMessage.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(txtDate.Text))
            {
                ShowError("Please select date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWorkDetails.Text))
            {
                ShowError("Please enter work details.");
                return;
            }

            DateTime workDate;

            if (!DateTime.TryParse(txtDate.Text, out workDate))
            {
                ShowError("Please enter a valid date.");
                return;
            }

            int userID;

            if (Session["UserID"] == null ||
                !int.TryParse(
                    Convert.ToString(Session["UserID"]), out userID))
            {
                Response.Redirect("~/Login.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return;
            }

            string selectedLocation =
                Convert.ToString(Session["SelectedLocation"]);

            string workReportID =
                Request.QueryString["WorkReportID"];

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    con.Open();

                    if (!string.IsNullOrWhiteSpace(workReportID))
                    {
                        int reportID;

                        if (!int.TryParse(workReportID, out reportID))
                        {
                            ShowError("Invalid Work Report ID.");
                            return;
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            @"UPDATE dbo.WorkReport
                              SET WorkDate = @WorkDate,
                                  WorkDetails = @WorkDetails
                              WHERE WorkReportID = @WorkReportID
                                AND LocationCode = @LocationCode", con))
                        {
                            cmd.Parameters.Add("@WorkDate", SqlDbType.Date)
                                .Value = workDate.Date;

                            cmd.Parameters.Add("@WorkDetails",
                                SqlDbType.NVarChar, -1)
                                .Value = txtWorkDetails.Text.Trim();

                            cmd.Parameters.Add("@WorkReportID",
                                SqlDbType.Int).Value = reportID;

                            cmd.Parameters.Add("@LocationCode",
                                SqlDbType.NVarChar, 50)
                                .Value = selectedLocation;

                            int rows = cmd.ExecuteNonQuery();

                            if (rows == 0)
                            {
                                ShowError(
                                    "Work Report was not found or could not be updated.");
                                return;
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO dbo.WorkReport
                                (WorkDate, WorkDetails, LocationCode, UserID)
                              VALUES
                                (@WorkDate, @WorkDetails, @LocationCode, @UserID)",
                            con))
                        {
                            cmd.Parameters.Add("@WorkDate", SqlDbType.Date)
                                .Value = workDate.Date;

                            cmd.Parameters.Add("@WorkDetails",
                                SqlDbType.NVarChar, -1)
                                .Value = txtWorkDetails.Text.Trim();

                            cmd.Parameters.Add("@LocationCode",
                                SqlDbType.NVarChar, 50)
                                .Value = selectedLocation;

                            cmd.Parameters.Add("@UserID", SqlDbType.Int)
                                .Value = userID;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                NavigateAfterSave();
            }
            catch (Exception ex)
            {
                ShowError("Error saving Work Report: " + ex.Message);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            if (!ScreenAccess.Check(this, "WorkReport"))
                return;

            // Fresh form kholein, edit query string ke bina
            Response.Redirect("~/WorkReport.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (!ScreenAccess.Check(this, "WorkReport"))
                return;

            NavigateAfterSave();
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
        }
    }
}

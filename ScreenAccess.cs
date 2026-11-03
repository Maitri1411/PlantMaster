
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace PlantMaster
{
    public static class ScreenAccess
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"]
                .ConnectionString;

        public static bool Check(Page page, string screenName)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            int userID;

            if (page.Session["CommonLogin"] == null ||
                !string.Equals(
                    Convert.ToString(page.Session["CommonLogin"]),
                    "True",
                    StringComparison.OrdinalIgnoreCase) ||
                page.Session["UserID"] == null ||
                !int.TryParse(
                    Convert.ToString(page.Session["UserID"]),
                    out userID))
            {
                page.Response.Redirect("~/Login.aspx", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return false;
            }

            if (string.IsNullOrWhiteSpace(screenName))
            {
                RedirectToSelect(page);
                return false;
            }

            string role = "";

            using (SqlConnection con =
                new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT [Role]
                FROM dbo.UserMaster
                WHERE UserID = @UserID
                  AND IsActive = 1;", con))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    role = Convert.ToString(result).Trim();
            }

            // Manager can access all screens.
            if (string.Equals(
                role, "Manager",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Worker cannot open list pages.
            if (string.Equals(
                role, "Worker",
                StringComparison.OrdinalIgnoreCase))
            {
                string key = screenName.Trim();

                if (string.Equals(key, "UnitGenerationList",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "WorkReportList",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "AttendanceList",
                        StringComparison.OrdinalIgnoreCase))
                {
                    RedirectToSelect(page);
                    return false;
                }

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT COUNT(1)
                    FROM dbo.UserScreenPermissions
                    WHERE UserID = @UserID
                      AND ScreenName = @ScreenName
                      AND IsAllowed = 1;", con))
                {
                    cmd.Parameters.Add(
                        "@UserID", SqlDbType.Int).Value = userID;

                    cmd.Parameters.Add(
                        "@ScreenName", SqlDbType.NVarChar, 100).Value = key;

                    con.Open();

                    int allowed = Convert.ToInt32(cmd.ExecuteScalar());

                    if (allowed > 0)
                        return true;
                }
            }

            RedirectToSelect(page);
            return false;
        }

        private static void RedirectToSelect(Page page)
        {
            page.Response.Redirect("~/Select.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}

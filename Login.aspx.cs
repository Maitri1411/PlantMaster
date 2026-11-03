
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;

namespace PlantMaster
{
    public partial class Login : Page
    {
        private readonly string connectionString =
            ConfigurationManager
            .ConnectionStrings["PlantMasterDB"]
            .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string password = txtUserPassword.Text;

            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
            {
                ShowMessage(
                    "Please enter User Name and Password.",
                    Color.Red);
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT UserID, [Role]
                        FROM dbo.UserMaster
                        WHERE UserName = @UserName
                          AND [Password] = @Password
                          AND IsActive = 1;";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@UserName",
                            SqlDbType.NVarChar, 100).Value = userName;

                        cmd.Parameters.Add(
                            "@Password",
                            SqlDbType.NVarChar, 100).Value = password;

                        con.Open();

                        int userID;
                        string role;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                ShowMessage(
                                    "Invalid username, password, or inactive account.",
                                    Color.Red);
                                return;
                            }

                            userID = Convert.ToInt32(reader["UserID"]);

                            role = reader["Role"] == DBNull.Value
                                ? ""
                                : Convert.ToString(reader["Role"]).Trim();
                        }

                        if (!role.Equals("Manager",
                                StringComparison.OrdinalIgnoreCase) &&
                            !role.Equals("Worker",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            ShowMessage(
                                "Your account has no valid role. Contact the administrator.",
                                Color.Red);
                            return;
                        }

                        // Set login session
                        Session.Clear();
                        Session["CommonLogin"] = "True";
                        Session["UserID"] = userID;
                        Session["UserName"] = userName;
                        Session["Role"] = role;

                        Response.Redirect("~/Select.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "LOGIN ERROR: " + ex);

                ShowMessage(
                    "Unable to login. Please check the database connection.",
                    Color.Red);
            }
        }

        private void ShowMessage(string message, Color color)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.ForeColor = color;
        }
    }
}

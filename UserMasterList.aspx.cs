using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PlantMaster
{
    public partial class UserMasterList : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid(string.Empty);
            }
        }

        private void BindGrid(string searchText)
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT
                            UserID,
                            UserName,
                            FirstName,
                            LastName,
                            IsActive
                        FROM dbo.UserMaster
                        WHERE
                            @Search = ''
                            OR UserName LIKE @Pattern
                            OR FirstName LIKE @Pattern
                            OR LastName LIKE @Pattern
                        ORDER BY UserID DESC;";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@Search",
                            SqlDbType.NVarChar,
                            200).Value = searchText;

                        cmd.Parameters.Add(
                            "@Pattern",
                            SqlDbType.NVarChar,
                            202).Value = "%" + searchText + "%";

                        using (SqlDataAdapter da =
                            new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();

                            da.Fill(dt);

                            gvUsers.DataSource = dt;
                            gvUsers.DataBind();

                            lblMessage.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

                gvUsers.DataSource = null;
                gvUsers.DataBind();

                lblMessage.Text = "Unable to load user list. Please check database connection.";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text.Trim());
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserMaster.aspx");
        }
    }
}
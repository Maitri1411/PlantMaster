
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class PlantLogin : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CommonLogin"] == null ||
                !(Session["CommonLogin"] is bool) ||
                !(bool)Session["CommonLogin"] ||
                Session["UserID"] == null)
            {
                Session.Clear();

                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadPlants();
            }
        }

        // Load all active plants dynamically
        private void LoadPlants()
        {
            string query = @"
                SELECT PlantRecordID, PlantID, PlantName
                FROM dbo.PlantMaster
                WHERE IsActive = 1
                ORDER BY PlantName;";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlPlant.Items.Clear();
                ddlPlant.DataSource = dt;
                ddlPlant.DataTextField = "PlantName";
                ddlPlant.DataValueField = "PlantRecordID";
                ddlPlant.DataBind();
            }

            ddlPlant.Items.Insert(
                0,
                new ListItem("-- Select Plant --", ""));

            if (ddlPlant.Items.Count == 1)
            {
                ShowMessage(
                    "No active plants found in PlantMaster.",
                    Color.Red);
            }
            else
            {
                lblMessage.Text = string.Empty;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(ddlPlant.SelectedValue))
            {
                ShowMessage("Please select a plant.", Color.Red);
                return;
            }

            string password = txtPlantPassword.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Please enter Plant Password.", Color.Red);
                return;
            }

            int plantRecordID;

            if (!int.TryParse(
                ddlPlant.SelectedValue, out plantRecordID))
            {
                ShowMessage("Invalid plant selection.", Color.Red);
                return;
            }

            string query = @"
                SELECT PlantID, PlantRecordID
                FROM dbo.PlantMaster
                WHERE PlantRecordID = @PlantRecordID
                  AND [Password] = @Password
                  AND IsActive = 1;";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(
                    "@PlantRecordID", SqlDbType.Int).Value = plantRecordID;

                // Use the actual SQL data type of your Password column.
                cmd.Parameters.Add(
                    "@Password", SqlDbType.NVarChar, 15).Value = password;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowMessage(
                            "Invalid Plant Password.",
                            Color.Red);
                        return;
                    }

                    int plantID =
                        Convert.ToInt32(reader["PlantID"]);

                    int actualPlantRecordID =
                        Convert.ToInt32(reader["PlantRecordID"]);

                    Session["PlantID"] = plantID;
                    Session["PlantRecordID"] = actualPlantRecordID;
                    Session.Remove("SelectedLocation");
                }
            }

            Response.Redirect("Select.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ShowMessage(string message, Color color)
        {
            lblMessage.Text = Server.HtmlEncode(message);
            lblMessage.ForeColor = color;
        }
    }
}


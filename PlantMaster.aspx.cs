
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class PlantMaster : Page
    {
        private readonly string connectionString =
            ConfigurationManager
            .ConnectionStrings["PlantMasterDB"]
            .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowList();
                FillGrid();
            }
        }

        private void ShowList()
        {
            pnlList.Visible = true;
            pnlDetails.Visible = false;
        }

        private void ShowDetails()
        {
            pnlList.Visible = false;
            pnlDetails.Visible = true;
        }

        private void FillGrid()
        {
            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT PlantID, PlantName, Password, IsActive
                    FROM dbo.PlantMaster
                    WHERE @Search = ''
                       OR CONVERT(VARCHAR(20), PlantID)
                            LIKE '%' + @Search + '%'
                       OR PlantName LIKE '%' + @Search + '%'
                    ORDER BY PlantID;";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@Search", SqlDbType.VarChar, 100)
                        .Value = txtSearch.Text.Trim();

                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvPlantMaster.DataSource = dt;
                        gvPlantMaster.DataBind();
                    }
                }
            }
        }

        protected void txtSearch_TextChanged(
            object sender, EventArgs e)
        {
            FillGrid();
        }

        protected void btnNew_Click(
            object sender, EventArgs e)
        {
            ClearForm();
            ShowDetails();
        }

        protected void gvPlantMaster_RowCommand(
            object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EditRecord")
                return;

            int rowIndex;

            if (!int.TryParse(
                Convert.ToString(e.CommandArgument),
                out rowIndex))
                return;

            if (rowIndex < 0 ||
                rowIndex >= gvPlantMaster.DataKeys.Count)
                return;

            int plantID = Convert.ToInt32(
                gvPlantMaster.DataKeys[rowIndex].Value);

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT PlantID, PlantName, Password, IsActive
                    FROM dbo.PlantMaster
                    WHERE PlantID = @PlantID;";

                using (SqlCommand cmd =
                       new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@PlantID", SqlDbType.Int)
                        .Value = plantID;

                    con.Open();

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return;

                        txtPlantID.Text =
                            Convert.ToString(reader["PlantID"]);

                        txtPlantName.Text =
                            Convert.ToString(reader["PlantName"]);

                        // Load the saved password
                        txtPassword.Text =
                            reader["Password"] == DBNull.Value
                            ? ""
                            : Convert.ToString(reader["Password"]);

                        chkActive.Checked =
                            Convert.ToBoolean(reader["IsActive"]);

                        hfOriginalPlantID.Value =
                            Convert.ToString(reader["PlantID"]);

                        lblRecordValue.Text =
                            Convert.ToString(reader["PlantID"]);
                    }
                }
            }

            ShowDetails();
        }

        protected void btnSave_Click(
            object sender, EventArgs e)
        {
            int plantID;
            string plantName = txtPlantName.Text.Trim();
            string password = txtPassword.Text;

            bool isEdit =
                !string.IsNullOrWhiteSpace(
                    hfOriginalPlantID.Value);

            if (!int.TryParse(
                txtPlantID.Text.Trim(), out plantID))
            {
                ShowResponse("Please enter a valid Plant ID.");
                return;
            }

            if (string.IsNullOrWhiteSpace(plantName) ||
                plantName.Length > 25)
            {
                ShowResponse(
                    "Plant Name is required and must be at most 25 characters.");
                return;
            }

            if (!isEdit &&
                string.IsNullOrWhiteSpace(password))
            {
                ShowResponse("Please enter Password.");
                return;
            }

            if (!string.IsNullOrEmpty(password) &&
                password.Length > 20)
            {
                ShowResponse(
                    "Password must be at most 20 characters.");
                return;
            }

            using (SqlConnection con =
                   new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction transaction =
                       con.BeginTransaction())
                {
                    try
                    {
                        string query;

                        if (isEdit && string.IsNullOrEmpty(password))
                        {
                            query = @"
                                UPDATE dbo.PlantMaster
                                SET PlantID = @PlantID,
                                    PlantName = @PlantName,
                                    IsActive = @IsActive
                                WHERE PlantID = @OriginalPlantID;";
                        }
                        else if (isEdit)
                        {
                            query = @"
                                UPDATE dbo.PlantMaster
                                SET PlantID = @PlantID,
                                    PlantName = @PlantName,
                                    Password = @Password,
                                    IsActive = @IsActive
                                WHERE PlantID = @OriginalPlantID;";
                        }
                        else
                        {
                            query = @"
                                INSERT INTO dbo.PlantMaster
                                (
                                    PlantID,
                                    PlantName,
                                    Password,
                                    IsActive
                                )
                                VALUES
                                (
                                    @PlantID,
                                    @PlantName,
                                    @Password,
                                    @IsActive
                                );";
                        }

                        using (SqlCommand cmd =
                               new SqlCommand(query, con, transaction))
                        {
                            cmd.Parameters.Add(
                                "@PlantID", SqlDbType.Int)
                                .Value = plantID;

                            cmd.Parameters.Add(
                                "@PlantName", SqlDbType.VarChar, 25)
                                .Value = plantName;

                            cmd.Parameters.Add(
                                "@IsActive", SqlDbType.Bit)
                                .Value = chkActive.Checked;

                            if (!isEdit ||
                                !string.IsNullOrEmpty(password))
                            {
                                cmd.Parameters.Add(
                                    "@Password", SqlDbType.VarChar, 20)
                                    .Value = password;
                            }

                            if (isEdit)
                            {
                                cmd.Parameters.Add(
                                    "@OriginalPlantID", SqlDbType.Int)
                                    .Value = Convert.ToInt32(
                                        hfOriginalPlantID.Value);
                            }

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected != 1)
                                throw new Exception(
                                    "Plant record was not saved.");
                        }

                        transaction.Commit();
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();

                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            ShowResponse(
                                "This Plant ID already exists.");
                            return;
                        }

                        throw;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }


            string responseMessage = isEdit
      ? "Plant updated successfully."
      : "Plant saved successfully.";

            
             Session["SelectedPlantID"] = plantID;

            ClearForm();
            ShowList();
            FillGrid();

            ShowResponse(responseMessage);

        }

        protected void btnBack_Click(
            object sender, EventArgs e)
        {
            ClearForm();
            ShowList();
            FillGrid();
        }

        private void ClearForm()
        {
            txtPlantID.Text = "";
            txtPlantName.Text = "";
            txtPassword.Text = "";

            chkActive.Checked = true;

            hfOriginalPlantID.Value = "";
            lblRecordValue.Text = "0";
        }

        private void ShowResponse(string message)
        {
            string safeMessage =
                message.Replace("\\", "\\\\")
                       .Replace("'", "\\'")
                       .Replace("\r", "")
                       .Replace("\n", "");

            ClientScript.RegisterStartupScript(
                GetType(),
                "PlantMasterResponse",
                "alert('" + safeMessage + "');",
                true);
        }
    }
}

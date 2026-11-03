
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class InverterMaster : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillGrid();
            }
        }

        // Load and Search Inverter Records
        private void FillGrid()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT PlantID, InverterID, InverterName, IsActive
                        FROM dbo.InverterMaster
                        WHERE @Search = ''
                           OR CAST(PlantID AS VARCHAR(20))
                                LIKE '%' + @Search + '%'
                           OR CAST(InverterID AS VARCHAR(20))
                                LIKE '%' + @Search + '%'
                           OR InverterName LIKE '%' + @Search + '%'
                        ORDER BY PlantID, InverterID";

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

                            gvInverterMaster.DataSource = dt;
                            gvInverterMaster.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowResponse(
                    "Unable to load inverter list: " + ex.Message);
            }
        }

        // Search
        protected void txtSearch_TextChanged(
            object sender, EventArgs e)
        {
            FillGrid();
        }

        // Add New Inverter
        protected void btnNew_Click(
            object sender, EventArgs e)
        {
            ClearForm();

            lblFormTitle.Text = "Add Inverter";
            lblRecordValue.Text = "0";

            pnlList.Visible = false;
            pnlDetails.Visible = true;
        }

        // Back to List
        protected void btnBack_Click(
            object sender, EventArgs e)
        {
            ClearForm();

            pnlDetails.Visible = false;
            pnlList.Visible = true;

            FillGrid();
        }

        // Save or Update Inverter
        protected void btnSave_Click(
            object sender, EventArgs e)
        {
            int plantID;
            int inverterID;

            if (!int.TryParse(
                txtPlantID.Text.Trim(), out plantID))
            {
                ShowResponse("Please enter a valid Plant ID.");
                return;
            }

            if (!int.TryParse(
                txtInverterID.Text.Trim(), out inverterID))
            {
                ShowResponse(
                    "Please enter a valid numeric Inverter ID.");
                return;
            }

            string inverterName = txtInverterName.Text.Trim();

            if (string.IsNullOrWhiteSpace(inverterName))
            {
                ShowResponse("Please enter Inverter Name.");
                return;
            }

            bool isEdit =
                !string.IsNullOrWhiteSpace(
                    hfOriginalInverterID.Value);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query;

                    if (isEdit)
                    {
                        query = @"
                            UPDATE dbo.InverterMaster
                            SET PlantID = @PlantID,
                                InverterID = @InverterID,
                                InverterName = @InverterName,
                                IsActive = @IsActive
                            WHERE PlantID = @OriginalPlantID
                              AND InverterID = @OriginalInverterID";
                    }
                    else
                    {
                        query = @"
                            INSERT INTO dbo.InverterMaster
                            (
                                PlantID,
                                InverterID,
                                InverterName,
                                IsActive
                            )
                            VALUES
                            (
                                @PlantID,
                                @InverterID,
                                @InverterName,
                                @IsActive
                            )";
                    }

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@PlantID", SqlDbType.Int)
                            .Value = plantID;

                        cmd.Parameters.Add(
                            "@InverterID", SqlDbType.Int)
                            .Value = inverterID;

                        cmd.Parameters.Add(
                            "@InverterName", SqlDbType.VarChar, 100)
                            .Value = inverterName;

                        cmd.Parameters.Add(
                            "@IsActive", SqlDbType.Bit)
                            .Value = chkActive.Checked;

                        if (isEdit)
                        {
                            cmd.Parameters.Add(
                                "@OriginalPlantID", SqlDbType.Int)
                                .Value = Convert.ToInt32(
                                    hfOriginalPlantID.Value);

                            cmd.Parameters.Add(
                                "@OriginalInverterID", SqlDbType.Int)
                                .Value = Convert.ToInt32(
                                    hfOriginalInverterID.Value);
                        }

                        con.Open();

                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            ShowResponse(
                                isEdit
                                ? "Inverter record not found."
                                : "Inverter could not be saved.");
                            return;
                        }
                    }
                }

                ClearForm();

                pnlDetails.Visible = false;
                pnlList.Visible = true;

                FillGrid();

                ShowResponse(
                    isEdit
                    ? "Inverter updated successfully."
                    : "Inverter saved successfully.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    ShowResponse(
                        "Duplicate record. Check Plant ID and Inverter ID.");
                }
                else
                {
                    ShowResponse(
                        "Database error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowResponse(
                    "Unable to save inverter: " + ex.Message);
            }
        }

        // Edit Inverter
        protected void gvInverterMaster_RowCommand(
            object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EditRecord")
                return;

            int index;

            if (!int.TryParse(
                Convert.ToString(e.CommandArgument), out index))
            {
                return;
            }

            if (index < 0 ||
                index >= gvInverterMaster.Rows.Count)
            {
                return;
            }

            int plantID = Convert.ToInt32(
                gvInverterMaster.DataKeys[index]
                    .Values["PlantID"]);

            int inverterID = Convert.ToInt32(
                gvInverterMaster.DataKeys[index]
                    .Values["InverterID"]);

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT PlantID, InverterID,
                               InverterName, IsActive
                        FROM dbo.InverterMaster
                        WHERE PlantID = @PlantID
                          AND InverterID = @InverterID";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@PlantID", SqlDbType.Int)
                            .Value = plantID;

                        cmd.Parameters.Add(
                            "@InverterID", SqlDbType.Int)
                            .Value = inverterID;

                        con.Open();

                        using (SqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtPlantID.Text =
                                    reader["PlantID"].ToString();

                                txtInverterID.Text =
                                    reader["InverterID"].ToString();

                                txtInverterName.Text =
                                    reader["InverterName"].ToString();

                                chkActive.Checked =
                                    Convert.ToBoolean(
                                        reader["IsActive"]);

                                hfOriginalPlantID.Value =
                                    plantID.ToString();

                                hfOriginalInverterID.Value =
                                    inverterID.ToString();

                                lblFormTitle.Text = "Edit Inverter";
                                lblRecordValue.Text =
                                    inverterID.ToString();

                                pnlList.Visible = false;
                                pnlDetails.Visible = true;
                            }
                            else
                            {
                                ShowResponse("Inverter not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowResponse(
                    "Unable to open inverter: " + ex.Message);
            }
        }

        // Clear Add/Edit Form
        private void ClearForm()
        {
            txtPlantID.Text = "";
            txtInverterID.Text = "";
            txtInverterName.Text = "";

            chkActive.Checked = true;

            hfOriginalPlantID.Value = "";
            hfOriginalInverterID.Value = "";

            lblRecordValue.Text = "0";
        }

        // Display Message
        private void ShowResponse(string message)
        {
            string safeMessage =
                HttpUtility.JavaScriptStringEncode(message);

            ClientScript.RegisterStartupScript(
                GetType(),
                Guid.NewGuid().ToString("N"),
                "alert('" + safeMessage + "');",
                true);
        }
    }
}

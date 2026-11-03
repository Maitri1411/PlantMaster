
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;

namespace PlantMaster
{
    public partial class UnitGenerationPrint : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUnitGenerationReport();
            }
        }

        private void LoadUnitGenerationReport()
        {
            try
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings["PlantMasterDB"];

                if (settings == null ||
                    string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    ShowMessage("PlantMasterDB connection string not found in Web.config.");
                    return;
                }

                string locationCode =
                    Request.QueryString["LocationCode"];

                if (string.IsNullOrWhiteSpace(locationCode))
                {
                    ShowMessage("Location is not available. Please open the report from Unit Generation List.");
                    return;
                }

                bool validLocation = false;

                for (int i = 1; i <= 10; i++)
                {
                    if (locationCode.Equals(
                        "Location" + i,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        locationCode = "Location" + i;
                        validLocation = true;
                        break;
                    }
                }

                if (!validLocation)
                {
                    ShowMessage("Invalid Location.");
                    return;
                }

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(settings.ConnectionString))
                {
                    string query = @"
                        SELECT
                            GenerationDate,
                            PlantID,
                            PGVCL,
                            GETCO,
                            InverterID,
                            Value,
                            INVTWISE
                        FROM dbo.UnitGeneration
                        WHERE LocationCode = @LocationCode
                        ORDER BY GenerationDate DESC, PlantID, InverterID";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@LocationCode",
                            SqlDbType.VarChar,
                            20
                        ).Value = locationCode;

                        using (SqlDataAdapter da =
                            new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    ShowMessage("No Unit Generation records found for " +
                                locationCode + ".");
                    return;
                }

                rvUnitGeneration.Reset();

                rvUnitGeneration.ProcessingMode =
                    ProcessingMode.Local;

                rvUnitGeneration.ShowToolBar = true;
                rvUnitGeneration.ShowPrintButton = true;
                rvUnitGeneration.ShowExportControls = true;
                rvUnitGeneration.ShowFindControls = true;
                rvUnitGeneration.ShowPageNavigationControls = true;
                rvUnitGeneration.ShowRefreshButton = true;
                rvUnitGeneration.ShowZoomControl = true;

                rvUnitGeneration.LocalReport.DataSources.Clear();

                rvUnitGeneration.LocalReport.ReportPath =
                    Server.MapPath("~/UnitGenerationPrint.rdlc");

                ReportDataSource rds =
                    new ReportDataSource(
                        "UnitGenerationDataSet",
                        dt
                    );

                rvUnitGeneration.LocalReport.DataSources.Add(rds);

                rvUnitGeneration.LocalReport.Refresh();

                lblMessage.Visible = false;
            }
            catch (Exception ex)
            {
                ShowMessage("Report Error: " + ex.Message);
            }
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
        }
    }
}

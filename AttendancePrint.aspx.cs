
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;

namespace PlantMaster
{
    public partial class AttendancePrint : System.Web.UI.Page
    {
        string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["CommonLogin"] == null ||
                !Session["CommonLogin"].ToString().Equals(
                    "True", StringComparison.OrdinalIgnoreCase) ||
                Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Session["PlantID"] == null)
            {
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Session["SelectedLocation"] == null)
            {
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadAttendanceReport();
            }
        }

        private void LoadAttendanceReport()
        {
            try
            {
                int plantID = Convert.ToInt32(Session["PlantID"]);

                string locationCode =
                    Session["SelectedLocation"].ToString();

                DataTable dt = new DataTable();

                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd =
                        new SqlCommand("dbo.usp_GetAttendanceList", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PlantID",
                            SqlDbType.Int).Value = plantID;

                        cmd.Parameters.Add("@LocationCode",
                            SqlDbType.VarChar, 20).Value = locationCode;

                        using (SqlDataAdapter da =
                            new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                rvAttendance.LocalReport.DataSources.Clear();

                rvAttendance.LocalReport.ReportPath =
                    Server.MapPath("~/AttendancePrint.rdlc");

                ReportDataSource rds =
                    new ReportDataSource("AteendanceDataSet", dt);

                rvAttendance.LocalReport.DataSources.Add(rds);

                rvAttendance.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Report Error: " + ex.Message;
                lblMessage.Visible = true;
            }
        }
    }
}

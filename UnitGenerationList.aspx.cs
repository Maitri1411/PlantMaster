using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlantMaster
{
    public partial class UnitGenerationList : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["PlantMasterDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsValidLocation())
            {
                Response.Redirect("~/Select.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                LoadUnitGeneration();
            }
        }

        // Get selected location.
        private string GetSelectedLocation()
        {
            string location =
                Convert.ToString(Session["SelectedLocation"]).Trim();

            int number;

            if (!location.StartsWith(
                    "Location",
                    StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(
                    location.Substring("Location".Length),
                    out number) ||
                number < 1 || number > 10)
            {
                throw new InvalidOperationException(
                    "Please select a valid location.");
            }

            return "Location" + number;
        }

        // Validate selected location.
        private bool IsValidLocation()
        {
            try
            {
                GetSelectedLocation();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Load Unit Generation records.
        private void LoadUnitGeneration()
        {
            try
            {
                string location = GetSelectedLocation();

                using (SqlConnection con =
                       new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        GenerationDate,
                        PlantID,
                        PGVCL,
                        GETCO,
                        InverterID,
                        Value,
                        INVTWISE,
                        IsActive
                    FROM dbo.UnitGeneration
                    WHERE LocationCode = @LocationCode
                    ORDER BY
                        GenerationDate DESC,
                        PlantID,
                        InverterID;", con))
                {
                    cmd.Parameters.Add(
                        "@LocationCode",
                        SqlDbType.VarChar, 20).Value = location;

                    DataTable dt = new DataTable();

                    using (SqlDataAdapter da =
                           new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Search filter.
                    string search = txtSearch.Text.Trim();

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow row = dt.Rows[i];

                            string rowDate =
                                Convert.ToDateTime(row["GenerationDate"])
                                .ToString("dd-MM-yyyy");

                            string plant =
                                Convert.ToString(row["PlantID"]);

                            string inverter =
                                Convert.ToString(row["InverterID"]);

                            bool match =
                                plant.IndexOf(
                                    search,
                                    StringComparison.OrdinalIgnoreCase) >= 0
                                ||
                                inverter.IndexOf(
                                    search,
                                    StringComparison.OrdinalIgnoreCase) >= 0
                                ||
                                rowDate.IndexOf(
                                    search,
                                    StringComparison.OrdinalIgnoreCase) >= 0;

                            if (!match)
                            {
                                dt.Rows.RemoveAt(i);
                            }
                        }
                    }

                    gvUnitGeneration.DataSource = dt;
                    gvUnitGeneration.DataBind();

                    lblMessage.Text = dt.Rows.Count == 0
                        ? "No matching Unit Generation records found."
                        : "";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Unable to load records: " + ex.Message;
            }
        }

        // SEARCH BUTTON.
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvUnitGeneration.EditIndex = -1;
            LoadUnitGeneration();
        }

        // REPORT BUTTON.
        protected void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                string location = GetSelectedLocation();

                string url =
                    "~/UnitGenerationPrint.aspx?LocationCode=" +
                    Server.UrlEncode(location);

                Response.Redirect(url, false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Unable to open report: " + ex.Message;
            }
        }

        // ADD NEW BUTTON.
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                "~/UnitGeneration.aspx",
                false);

            Context.ApplicationInstance.CompleteRequest();
        }

        // EDIT BUTTON.
        protected void gvUnitGeneration_RowEditing(
            object sender,
            GridViewEditEventArgs e)
        {
            e.Cancel = true;

            try
            {
                DataKey key =
                    gvUnitGeneration.DataKeys[e.NewEditIndex];

                DateTime date =
                    Convert.ToDateTime(
                        key.Values["GenerationDate"]);

                int plantID =
                    Convert.ToInt32(
                        key.Values["PlantID"]);

                string inverterID =
                    Convert.ToString(
                        key.Values["InverterID"]);

                string url =
                    "~/UnitGeneration.aspx?GenerationDate=" +
                    Server.UrlEncode(
                        date.ToString("yyyy-MM-dd")) +
                    "&PlantID=" + plantID +
                    "&InverterID=" +
                    Server.UrlEncode(inverterID);

                Response.Redirect(url, false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Unable to open record: " + ex.Message;
            }
        }

        // CANCEL EDIT.
        protected void gvUnitGeneration_RowCancelingEdit(
            object sender,
            GridViewCancelEditEventArgs e)
        {
            gvUnitGeneration.EditIndex = -1;

            LoadUnitGeneration();
        }

        // UPDATE RECORD.
        protected void gvUnitGeneration_RowUpdating(
            object sender,
            GridViewUpdateEventArgs e)
        {
            try
            {
                string location = GetSelectedLocation();

                GridViewRow row =
                    gvUnitGeneration.Rows[e.RowIndex];

                TextBox txtPGVCL =
                    row.Cells[2].Controls[0] as TextBox;

                TextBox txtGETCO =
                    row.Cells[3].Controls[0] as TextBox;

                TextBox txtValue =
                    row.Cells[5].Controls[0] as TextBox;

                decimal pgvcl;
                decimal getco;
                decimal value;

                if (txtPGVCL == null ||
                    txtGETCO == null ||
                    txtValue == null ||
                    !TryDecimal(txtPGVCL.Text, out pgvcl) ||
                    !TryDecimal(txtGETCO.Text, out getco) ||
                    !TryDecimal(txtValue.Text, out value))
                {
                    lblMessage.Text =
                        "Enter valid numeric values.";

                    return;
                }

                if (pgvcl < 0 || getco < 0 || value < 0)
                {
                    lblMessage.Text =
                        "Values cannot be negative.";

                    return;
                }

                DataKey key =
                    gvUnitGeneration.DataKeys[e.RowIndex];

                DateTime date =
                    Convert.ToDateTime(
                        key.Values["GenerationDate"]);

                int plantID =
                    Convert.ToInt32(
                        key.Values["PlantID"]);

                string inverterID =
                    Convert.ToString(
                        key.Values["InverterID"]);

                using (SqlConnection con =
                       new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlTransaction tran =
                           con.BeginTransaction())
                    {
                        try
                        {
                            int affected;

                            // Update selected record.
                            using (SqlCommand cmd =
                                   new SqlCommand(@"
                                UPDATE dbo.UnitGeneration
                                SET
                                    PGVCL = @PGVCL,
                                    GETCO = @GETCO,
                                    Value = @Value
                                WHERE
                                    PlantID = @PlantID
                                    AND LocationCode = @LocationCode
                                    AND GenerationDate = @GenerationDate
                                    AND InverterID = @InverterID
                                    AND IsActive = 1;",
                                    con, tran))
                            {
                                cmd.Parameters.Add(
                                    "@PlantID",
                                    SqlDbType.Int).Value = plantID;

                                cmd.Parameters.Add(
                                    "@LocationCode",
                                    SqlDbType.VarChar, 20).Value = location;

                                cmd.Parameters.Add(
                                    "@GenerationDate",
                                    SqlDbType.Date).Value = date.Date;

                                cmd.Parameters.Add(
                                    "@InverterID",
                                    SqlDbType.VarChar, 25).Value = inverterID;

                                AddDecimal(cmd, "@PGVCL", pgvcl);
                                AddDecimal(cmd, "@GETCO", getco);
                                AddDecimal(cmd, "@Value", value);

                                affected = cmd.ExecuteNonQuery();
                            }

                            if (affected == 0)
                            {
                                throw new InvalidOperationException(
                                    "Record not found. Please refresh the list.");
                            }

                            // Recalculate INVTWISE.
                            using (SqlCommand cmd =
                                   new SqlCommand(@"
                                UPDATE dbo.UnitGeneration
                                SET INVTWISE = (
                                    SELECT ISNULL(SUM(Value), 0)
                                    FROM dbo.UnitGeneration
                                    WHERE
                                        PlantID = @PlantID
                                        AND LocationCode = @LocationCode
                                        AND GenerationDate = @GenerationDate
                                        AND IsActive = 1
                                )
                                WHERE
                                    PlantID = @PlantID
                                    AND LocationCode = @LocationCode
                                    AND GenerationDate = @GenerationDate
                                    AND IsActive = 1;",
                                    con, tran))
                            {
                                cmd.Parameters.Add(
                                    "@PlantID",
                                    SqlDbType.Int).Value = plantID;

                                cmd.Parameters.Add(
                                    "@LocationCode",
                                    SqlDbType.VarChar, 20).Value = location;

                                cmd.Parameters.Add(
                                    "@GenerationDate",
                                    SqlDbType.Date).Value = date.Date;

                                cmd.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }

                gvUnitGeneration.EditIndex = -1;

                LoadUnitGeneration();

                lblMessage.Text =
                    "Record updated successfully.";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Unable to update record: " + ex.Message;
            }
        }

        // Validate decimal values.
        private bool TryDecimal(
            string input,
            out decimal value)
        {
            return decimal.TryParse(
                input,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value);
        }

        // Add decimal SQL parameter.
        private void AddDecimal(
            SqlCommand cmd,
            string name,
            decimal value)
        {
            SqlParameter p =
                cmd.Parameters.Add(
                    name,
                    SqlDbType.Decimal);

            p.Precision = 18;
            p.Scale = 2;
            p.Value = value;
        }
    }
}
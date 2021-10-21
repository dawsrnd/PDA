using Database;
using System;
using System.Configuration;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace PDA
{
    public partial class Scan : Page
    {
        readonly db db = new db();
        readonly string planta = ConfigurationManager.AppSettings["plant"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (planta != null)
                {
                    panelPage.Visible = true;
                    ddl_linea.Items.Clear();
                    ddl_linea.Items.Add(new ListItem("- Linea -", "-1"));
                    ddl_linea.DataSource = db.GetDataTableDaws("SELECT idLinea, nomLinea FROM lineas WHERE [Planta] = " + planta + " ORDER BY nomLinea ASC");
                    lblShift.Text = db.GetStringDaws("(SELECT CASE WHEN  CAST(GETDATE() AS TIME)  BETWEEN '07:00:00' AND '16:40:00' THEN 'TURNO A' " +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '16:40:00' AND '23:59:59' THEN 'TURNO B'" +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '00:00:00' AND '01:20:00' THEN 'TURNO B' " +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '01:20:00' AND '07:00:00' THEN 'TURNO C'" +
                        "						END)");
                    ddl_linea.DataTextField = "nomLinea";
                    ddl_linea.DataValueField = "idLinea";
                    ddl_linea.DataBind();
                    txtEmpleado.Enabled = false;
                }
                else
                {
                    Response.Write("<h4 style='color:red'>ERR01: URL Incorrecta</h4>");
                }
            }
        }

        public void bind()
        {
            gv_estacion.DataSource = db.GetDataTableDaws("SELECT IDX,estacion, tipo, orden FROM posiciones WHERE idLinea = " + ddl_linea.SelectedValue + " ORDER BY orden asc");
            gv_estacion.DataBind();
        }

        protected void gv_estacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdfOrden.Value = gv_estacion.DataKeys[gv_estacion.SelectedIndex].Values[0].ToString();
            hdfEstacion.Value = gv_estacion.DataKeys[gv_estacion.SelectedIndex].Values[1].ToString();
            hdfTipo.Value = gv_estacion.DataKeys[gv_estacion.SelectedIndex].Values[2].ToString();
            foreach (GridViewRow row in gv_estacion.Rows)
            {
                if (row.RowIndex == gv_estacion.SelectedIndex)
                {
                    row.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                    txtEmpleado.Focus();
                    txtEmpleado.BackColor = ColorTranslator.FromHtml("#A1DCF2");
                    txtEmpleado.Enabled = true;
                }
                else
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            }
        }

        protected void txtEmpleado_TextChanged(object sender, EventArgs e)
        {
            db.SP_INSERTORUPDATE_ATTENDANCE(Convert.ToInt32(ddl_linea.SelectedValue),
                                              txtEmpleado.Text,
                                              hdfEstacion.Value,
                                              hdfTipo.Value,
                                              1,
                                              Convert.ToInt32(hdfOrden.Value),
                                              Convert.ToInt32(planta));
            lblTotal.Text = db.SP_COUNT_ATTENDANCE(Convert.ToInt32(ddl_linea.SelectedValue));
            txtEmpleado.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            txtEmpleado.Text = "";
            txtEmpleado.Focus();
        }

        protected void ddl_linea_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddl_linea.SelectedItem.Value != "-1")
            {
                gv_estacion.Visible = true;
                lblTotal.Text = db.SP_COUNT_ATTENDANCE(Convert.ToInt32(ddl_linea.SelectedValue));
                txtEmpleado.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                txtEmpleado.Enabled = false;
                txtEmpleado.Text = "";
                bind();
            }
            else
            {
                gv_estacion.Visible = false;
                lblTotal.Text = "";
            }
        }
    }
}
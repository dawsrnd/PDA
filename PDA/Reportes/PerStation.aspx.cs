using Database;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;

namespace PDA.Reportes
{
    public partial class PerStation : Page
    {
        readonly db db = new db();
        readonly string planta = ConfigurationManager.AppSettings["plant"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddl_linea.Items.Clear();
                ddl_linea.DataSource = db.GetDataTableDaws("SELECT idLinea, nomLinea FROM lineas WHERE [Planta] = " + planta + " ORDER BY nomLinea ASC");
                ddl_linea.DataTextField = "nomLinea";
                ddl_linea.DataValueField = "idLinea";
                ddl_linea.DataBind();
                ddl_station.DataSource = db.GetDataTableDaws("SELECT estacion, orden FROM posiciones WHERE idLinea = '" + ddl_linea.SelectedValue + "' ORDER BY orden");
                ddl_station.DataTextField = "estacion";
                ddl_station.DataValueField = "estacion";
                ddl_station.DataBind();
                txtFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            gv_station.DataSource = db.GetDataTableDaws("SELECT a.numero, b.nomLinea as 'Linea', a.estacion, resultdate as 'fecha' " +
                " FROM[daws].[dbo].[attendance] a" +
                " INNER JOIN[daws].[dbo].[lineas] b" +
                " ON a.idLinea = b.idLinea" +
                " WHERE" +
                " a.idLinea = " + ddl_linea.SelectedValue +
                " AND a.estacion = '" + ddl_station.SelectedItem.Text + "'" +
                " AND a.resultshift = " + ddl_shift.SelectedValue + "" +
                " AND a.resultdate BETWEEN '" + txtFrom.Text + "'" +
                " AND '" + txtTo.Text + "'" +
                " ORDER BY resultdate DESC");
            gv_station.DataBind();
            HttpCookie cookie = new HttpCookie("generateFlag");
            cookie.Value = "Flag";
            cookie.Expires = DateTime.Now.AddDays(1);
        }

        protected void ddl_linea_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddl_station.Items.Clear();
            ddl_station.DataSource = db.GetDataTableDaws("SELECT estacion, orden FROM posiciones WHERE idLinea = '" + ddl_linea.SelectedValue + "' ORDER BY orden");
            ddl_station.DataTextField = "estacion";
            ddl_station.DataValueField = "estacion";
            ddl_station.DataBind();
        }
    }
}
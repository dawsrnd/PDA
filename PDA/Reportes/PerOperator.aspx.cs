using Database;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;

namespace PDA
{
    public partial class PerOperator : Page
    {
        readonly db db = new db();
        readonly string planta = ConfigurationManager.AppSettings["plant"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFromOperador.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtToOperador.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        static bool UrlExists(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "HEAD";
                request.AllowAutoRedirect = false;
                request.GetResponse();
            }
            catch (UriFormatException)
            {
                // Invalid Url
                return false;
            }
            catch (WebException ex)
            {
                // Valid Url but not exists
                HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                if (webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
            }
            return true;
        }

        protected void btnShowOperador_Click(object sender, EventArgs e)
        {
            if (UrlExists("http://192.168.141.6:1111/Fotos/" + txtEmpleado.Text.Trim() + ".jpg"))
                imgTrabajador.ImageUrl = "http://192.168.141.6:1111/Fotos/" + txtEmpleado.Text.Trim() + ".jpg";
            else
                imgTrabajador.ImageUrl = "~/src/no-image-available.jpg";

            imgTrabajador.Visible = true;

            if (planta == "1")
                ListView1.DataSource = db.GetDataTableDaws("SELECT [numero],[nombre],[ingreso],[turno],[categoria],[puesto],[linea],[depto] FROM[daws].[dbo].[headcount] where numero = '" + txtEmpleado.Text + "'");
            else if (planta == "2")
                ListView1.DataSource = db.GetDataTableDaws("SELECT [Numreloj] as [numero],[Nombre] as [Nombre],[ingreso],[Turno] as [turno],[TipoEmp] as [categoria],[puesto] as [Puesto],[Linea] as [linea],[Depto] as [depto] FROM [HeadCount].[dbo].[NDDHeadcount] where [Numreloj] = '" + txtEmpleado.Text + "'");
            ListView1.DataBind();

            gv_defectos.DataSource = db.GetDataTableDaws("	SELECT c.[nomLinea], a.[estacion], [resultdate] as 'fecha',(SELECT COUNT(IDX) FROM [vistadefectos] d WHERE d.[ActionWorker] = a.[numero] COLLATE Korean_Wansung_CI_AS AND d.DefectDate=a.resultdate  COLLATE Korean_Wansung_CI_AS)'Defectos/Dia'  FROM [daws].[dbo].[attendance] a  INNER JOIN [daws].[dbo].[lineas] c on a.[idLinea] = c.[idLinea] WHERE " +
                "	a.numero='" + txtEmpleado.Text + "' AND a.resultdate BETWEEN '" + txtFromOperador.Text + "' AND '" + txtToOperador.Text + "' ORDER BY resultdate DESC");
            gv_defectos.DataBind();

            gv_defectos2.DataSource = db.GetDataTableDaws("	SELECT B.[IDX],[DefectDate],[numero],[resultshift] as 'Turno',[nomLinea],[estacion],[codigo],[categoria],[defecto] " +
                "	FROM [daws].[dbo].[attendance] A " +
                "	INNER JOIN [daws].[dbo].[vistadefectos]  B ON [resultdate]  COLLATE Korean_Wansung_CI_AS  = [DefectDate] and [numero]  COLLATE Korean_Wansung_CI_AS = [ActionWorker]" +
                "	INNER JOIN [daws].[dbo].[lineas] C on A.[idLinea] = C.[idLinea] WHERE" +
                "	[numero] = '" + txtEmpleado.Text + "' AND [resultdate] BETWEEN '" + txtFromOperador.Text + "' AND '" + txtToOperador.Text + "'");
            gv_defectos2.DataBind();
            HttpCookie cookie = new HttpCookie("generateFlag");
            cookie.Value = "Flag";
            cookie.Expires = DateTime.Now.AddDays(1);
        }
    }
}
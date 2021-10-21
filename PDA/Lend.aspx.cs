using Database;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Web;
using System.Web.UI;


namespace PDA
{
    public partial class Lend : Page
    {
        readonly db db = new db();
        readonly int planta = Convert.ToInt32(ConfigurationManager.AppSettings["plant"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                panelPage.Visible = true;
                ddl_destino.Items.Clear();
                ddl_destino.DataSource = db.GetDataTable("SELECT   [IDX],[area_name] FROM [lines_management].[dbo].[areas_lended]  ORDER BY area_name ASC");
                ddl_destino.DataTextField = "area_name";
                ddl_destino.DataValueField = "IDX";
                ddl_destino.DataBind();
                lblTotal.Text = db.SP_GET_TOTAL_LENDED(planta);
                lblShift.Text = db.GetStringDaws("(SELECT CASE WHEN  CAST(GETDATE() AS TIME)  BETWEEN '07:00:00' AND '16:40:00' THEN 'TURNO A' " +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '16:40:00' AND '23:59:59' THEN 'TURNO B'" +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '00:00:00' AND '01:20:00' THEN 'TURNO B' " +
                        "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '01:20:00' AND '07:00:00' THEN 'TURNO C'" +
                        "						END)");
            }
        }

        protected void txtboxEmpleado_TextChanged(object sender, EventArgs e)
        {
            if (txtboxEmpleado.Text.Length <= 10)
            {
                db.SP_INSERT_LENDEDSTAFF(txtboxEmpleado.Text,
                                        Convert.ToInt32(ddl_destino.SelectedValue), planta);
            }
            txtboxEmpleado.Text = "";
            lblTotal.Text = db.SP_GET_TOTAL_LENDED(planta);
            txtboxEmpleado.Focus();
        }

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Si")
            {
                try
                {
                    CreateXLSX();
                    string message = "<h2 style='color:#757575'> DAWS </h2><br><br><br><br><p style='font-size: 13px;'> <small>Please do not reply directly to this email. If you have any questions or comments regarding this email, please contact us at klrnd@kyungshinlear.com<br><br>This message was produced and distributed by the automated software of the R&D department </small></p>";
                    if (planta==1)
                    SendEmail(DateTime.Now.ToString("dd/MM/yyyy"), message, "mledezma@kyungshinlear.com");
                    else
                    SendEmail(DateTime.Now.ToString("dd/MM/yyyy"), message, "rmedrano@kyungshinlear.com");
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
        }

        public void SendEmail(string asunto, string cuerpo, string destinatario)
        {
            int shiftCurrent = db.GetInt("SELECT " +
                "						CASE" +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '07:00:00' AND '16:40:00' THEN '1' " +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '16:40:00' AND '23:59:59' THEN '2'" +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '00:00:00' AND '01:20:00' THEN '2' " +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '01:20:00' AND '07:00:00' THEN '3'" +
                "						END");
            string plant = (planta == 1) ? "DGO" : "NDD";
            MailAddress email_from = new MailAddress("InternalApp@kyungshin.co.kr", "PERSONAL PRESTADO"); ;
            MailAddress email_to = new MailAddress(destinatario);

            using (MailMessage mm = new MailMessage(email_from, email_to))
            {
                mm.Subject = asunto;
                mm.Body = cuerpo;
                mm.IsBodyHtml = true;


                string[] lines = File.ReadAllLines(HttpContext.Current.Server.MapPath("~/src/cc.txt"));
                foreach (string line in lines)
                    mm.CC.Add(new MailAddress(line));

                Attachment data;
                string file;

                file = HttpContext.Current.Server.MapPath("~/src/LendedStaff.xlsx");
                data = new Attachment(file);
                data.Name = "PERSONAL PRESTADO " + plant + " - Shift " + shiftCurrent + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                mm.Attachments.Add(data);

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "192.168.2.12";
                smtp.Port = 25;

                try
                {
                    smtp.Send(mm);
                    data.Dispose();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                }
            }
        }


        public void CreateXLSX()
        {
            //Reporte 
            int shiftCurrent = db.GetInt("SELECT " +
                "						CASE" +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '07:00:00' AND '16:40:00' THEN '1' " +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '16:40:00' AND '23:59:59' THEN '2'" +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '00:00:00' AND '01:20:00' THEN '2' " +
                "							WHEN  CAST(GETDATE() AS TIME)  BETWEEN '01:20:00' AND '07:00:00' THEN '3'" +
                "						END");


            string resultdate = db.GetString("SELECT " +
                "			CASE" +
                "				WHEN  CAST(GETDATE() AS TIME)  BETWEEN '07:00:00' AND '16:40:00' THEN CONVERT(DATE,GETDATE())" +
                "				WHEN  CAST(GETDATE() AS TIME)  BETWEEN '16:40:00' AND '23:59:59' THEN CONVERT(DATE,GETDATE())" +
                "				WHEN  CAST(GETDATE() AS TIME)  BETWEEN '00:00:00' AND '01:20:00' THEN CONVERT(DATE,GETDATE()-1) " +
                "				WHEN  CAST(GETDATE() AS TIME)  BETWEEN '01:20:00' AND '07:00:00' THEN CONVERT(DATE,GETDATE()-1)" +
                "			END");


            string plant = (planta == 1) ? "DGO" : "NDD";

            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Reporte");

            workSheet.Row(1).Height = 30;
            workSheet.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
            workSheet.Row(2).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
            workSheet.Cells["A1:E2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["A1:E2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
            workSheet.Row(1).Height = 30;
            workSheet.Row(2).Height = 30;

            using (Image image = Image.FromFile(HttpContext.Current.Server.MapPath("~/src/png/Logo.png")))
            {
                var excelImage = workSheet.Drawings.AddPicture("Logo", image);
                excelImage.SetPosition(0, 15, 0, 30);
            }


            workSheet.Cells[1, 4].Value = "FECHA";
            workSheet.Cells[1, 5].Value = DateTime.Now.ToString("yyyy-MM-dd");
            workSheet.Cells[2, 4].Value = "PLANTA";
            workSheet.Cells[2, 5].Value = plant;

            workSheet.Cells["A4:E4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Cells["A4:E4"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
            workSheet.Cells["A4:E4"].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));

            if (planta == 1)
                workSheet.Cells["A4"].LoadFromDataTable(db.GetDataTable("SELECT [NumEmpleado] as [Numero]" +
                    "      ,[nombre]" +
                    "      ,[ingreso]" +
                    "      ,[D].[linea] as [Origen]" +
                    "      ,[area_name] as [Destino]" +
                    "  FROM[lines_management].[dbo].[lended_staff] A" +
                    "  INNER JOIN[lines_management].[dbo].[areas_lended] C" +
                    "  ON[A].[area] = [C].[IDX]" +
                    "  INNER JOIN[daws].[dbo].[headcount] D" +
                    "  ON[A].[NumEmpleado] = [D].[numero] COLLATE SQL_Latin1_General_CP1_CI_AS" +
                    "  WHERE [resultshift] = " + shiftCurrent + " AND [Plant] = " + planta + " AND resultdateShift BETWEEN convert(date,'" + resultdate + "') AND convert(date,'" + resultdate + "')" +
                    "  ORDER BY[resultdateShift]  ASC, [D].[linea] ASC, [area_name] ASC"), true);
            else
                workSheet.Cells["A4"].LoadFromDataTable(db.GetDataTable("SELECT [NumEmpleado] as [Numero]" +
                "      ,[nombre]" +
                "      ,[ingreso]" +
                "      ,[D].[linea] as [Origen]" +
                "      ,[area_name] as [Destino]" +
                "  FROM[lines_management].[dbo].[lended_staff] A" +
                "  INNER JOIN[lines_management].[dbo].[areas_lended] C" +
                "  ON[A].[area] = [C].[IDX]" +
                "  INNER JOIN [HeadCount].[dbo].[NDDHeadcount] D" +
                "  ON[A].[NumEmpleado] = [D].[Numreloj] COLLATE SQL_Latin1_General_CP1_CI_AS" +
                "  WHERE [resultshift] = " + shiftCurrent + " AND [Plant] = " + planta + " AND resultdateShift BETWEEN convert(date,'" + resultdate + "') AND convert(date,'" + resultdate + "')" +
                "  ORDER BY[resultdateShift]  ASC, [D].[linea] ASC, [area_name] ASC"), true);

            ExcelRange rg = workSheet.Cells[4, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
            string tableName = "Table1";
            ExcelTable tab = workSheet.Tables.Add(rg, tableName);
            tab.TableStyle = TableStyles.Medium16;
            workSheet.View.ZoomScale = 75;
            workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Row(4).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Column(3).Style.Numberformat.Format = "yyyy-mm-dd";
            workSheet.Column(6).Style.Numberformat.Format = "yyyy-mm-dd";



            workSheet.View.ShowGridLines = false;
            var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
            var cellFont = allCells.Style.Font;
            cellFont.SetFromFont(new Font("Arial", 12));
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();


            //Pivote
            var workSheet2 = excel.Workbook.Worksheets.Add("info");
            workSheet2.Cells["A1"].LoadFromDataTable(db.GetDataTable("SELECT [NumEmpleado]" +
                "    ,[area_name]" +
                " FROM[lines_management].[dbo].[lended_staff] A" +
                " INNER JOIN[lines_management].[dbo].[areas_lended] B" +
                " ON[A].[area] = [B].[IDX]" +
                " WHERE Plant = " + planta + " and NumEmpleado in" +
                " (SELECT distinct NumEmpleado from lended_staff where [resultshift] = " + shiftCurrent + " and resultdateShift BETWEEN convert(date,'" + resultdate + "') AND convert(date,'" + resultdate + "')) ORDER BY NumEmpleado ASC"), true);

            var workSheet3 = excel.Workbook.Worksheets.Add("Matriz");
            workSheet3.TabColor = ColorTranslator.FromHtml("#92D050");

            var dataRange = workSheet2.Cells[workSheet2.Dimension.Address];

            var pivotTable = workSheet3.PivotTables.Add(workSheet3.Cells["B2"], dataRange, "PivotTable");

            pivotTable.RowFields.Add(pivotTable.Fields["NumEmpleado"]);
            pivotTable.DataOnRows = false;

            pivotTable.ColumnFields.Add(pivotTable.Fields["area_name"]);

            var field = pivotTable.DataFields.Add(pivotTable.Fields["area_name"]);
            field.Name = "Matriz de habilidades";
            field.Function = DataFieldFunctions.Count;

            pivotTable.PivotTableStyle = PivotTableStyles.Dark2;
            workSheet3.View.ShowGridLines = false;
            workSheet2.Hidden = eWorkSheetHidden.VeryHidden;


            var filePath = HttpContext.Current.Server.MapPath("~/src/LendedStaff.xlsx");
            FileInfo fi = new FileInfo(filePath);
            excel.SaveAs(fi);
        }
    }

}

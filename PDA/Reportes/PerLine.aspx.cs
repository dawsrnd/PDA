using Database;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PDA
{
    public partial class PerLine : Page
    {
        readonly db db = new db();
        readonly string planta = ConfigurationManager.AppSettings["plant"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddl_type_report.Items.Clear();
                ddl_type_report.Items.Add(new ListItem("PERSONAL ESCANEADO", "200"));
                ddl_type_report.Items.Add(new ListItem("PERSONAL PRESTADO", "201"));
                ddl_type_report.Items.Add(new ListItem("HEADCOUNT", "202"));
                ddl_type_report.Items.Add(new ListItem("HISTORIAL ASISTENCIA", "203"));
                ddl_type_report.Items.Add(new ListItem("", "404"));

                ddl_type_report.DataSource = db.GetDataTableDaws("SELECT idLinea, nomLinea FROM lineas WHERE [Planta] = " + planta + " ORDER BY nomLinea ASC");
                ddl_type_report.DataTextField = "nomLinea";
                ddl_type_report.DataValueField = "idLinea";
                ddl_type_report.DataBind();
                var listItem = ddl_type_report.Items.FindByText("");
                listItem.Attributes["disabled"] = "disabled";
                txtFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            gv_prestados.Visible = false;
            gv_report.Visible = false;
            switch (ddl_type_report.SelectedValue)
            {
                case "201":
                    gv_prestados.Visible = true;
                    if (planta == "1")
                        gv_prestados.DataSource = db.GetDataTable("SELECT A.[IDX],[NumEmpleado] as [Numero]" +
                "      ,[nombre]" +
                "      ,[D].[linea] as [Origen]" +
                "      ,[area_name] as [Destino]" +
                "      ,[resultdateShift] as [Fecha]" +
                "      ,[resultshift] as [Turno]" +
                "  FROM[lines_management].[dbo].[lended_staff] A" +
                "  INNER JOIN[lines_management].[dbo].[areas_lended] C" +
                "  ON[A].[area] = [C].[IDX]" +
                "  INNER JOIN[daws].[dbo].[headcount] D" +
                "  ON[A].[NumEmpleado] = [D].[numero] COLLATE SQL_Latin1_General_CP1_CI_AS" +
                "  WHERE [resultshift] = '" + ddl_shift.SelectedValue + "' AND [Plant] = " + Convert.ToInt32(planta) + " AND resultdateShift BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
                "  ORDER BY [resultdateShift] ASC, [D].[linea] ASC, [area_name] ASC, [Turno] ASC ");
                    else
                        gv_prestados.DataSource = db.GetDataTable("SELECT A.[IDX],[NumEmpleado] as [Numero]" +
               "      ,[nombre]" +
               "      ,[D].[linea] as [Origen]" +
               "      ,[area_name] as [Destino]" +
               "      ,[resultdateShift] as [Fecha]" +
               "      ,[resultshift] as [Turno]" +
               "  FROM[lines_management].[dbo].[lended_staff] A" +
               "  INNER JOIN[lines_management].[dbo].[areas_lended] C" +
               "  ON[A].[area] = [C].[IDX]" +
               "  INNER JOIN [HeadCount].[dbo].[NDDHeadcount]  D" +
               "  ON[A].[NumEmpleado] = [D].[Numreloj] COLLATE SQL_Latin1_General_CP1_CI_AS" +
               "  WHERE [resultshift] = '" + ddl_shift.SelectedValue + "' AND [Plant] = " + Convert.ToInt32(planta) + " AND resultdateShift BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
               "  ORDER BY [resultdateShift] ASC, [D].[linea] ASC, [area_name] ASC, [Turno] ASC ");
                    gv_prestados.DataBind();
                    break;
                case "200":
                    gv_report.Visible = true;
                    gv_report.DataSource = db.SP_READ_PERSONAL_ESCANEADO(ddl_shift.SelectedItem.Text, Convert.ToInt32(planta), txtFrom.Text, txtTo.Text);
                    gv_report.DataBind();
                    break;
                case "202":
                    gv_report.Visible = true;
                    gv_report.DataSource = db.GetDataTableDaws("SELECT B.nomLinea as 'Linea', CONCAT(COUNT(numero),' / ',Qty) Cantidad FROM [daws].[dbo].[attendance] A" +
                        " INNER JOIN [daws].[dbo].[lineas] B" +
                        " ON A.idLinea = B.idLinea and B.Planta = " + Convert.ToInt32(planta) +
                        " LEFT JOIN [daws].[dbo].[Manning_Cantidad] C" +
                        " ON A.idLinea = C.idLinea" +
                        " WHERE resultdate2 BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
                        " AND resultshift = " + ddl_shift.SelectedValue +
                        " GROUP BY B.nomLinea, Qty" +
                        " ORDER BY B.nomLinea asc");
                    gv_report.DataBind();
                    break;
                case "203":
                    string script = "ShowToast(3,'Solo disponible para descargar.');";
                    ClientScript.RegisterStartupScript(this.GetType(), "showError", script, true);
                    break;
                default:
                    gv_report.Visible = true;
                    gv_report.DataSource = db.GetDataTableDaws("SELECT estacion, tipo, COUNT(numero) AS 'operadores' FROM  [daws].[dbo].[attendance]" +
                        " WHERE" +
                        " resultdate2 BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
                        " AND idLinea = '" + ddl_type_report.SelectedValue + "'" +
                        " AND resultshift = " + ddl_shift.SelectedValue +
                        " GROUP BY estacion, tipo, orden" +
                        " ORDER BY orden");
                    gv_report.DataBind();
                    break;
            }
        }

        protected void btnExcelDown_Click(object sender, EventArgs e)
        {
            if (ddl_type_report.SelectedValue == "201")
            {
                //Reporte 

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Reporte");

                workSheet.Row(1).Height = 30;
                workSheet.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet.Row(2).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet.Cells["A1:G2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A1:G2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet.Row(1).Height = 30;
                workSheet.Row(2).Height = 30;

                using (System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/src/png/Logo.png")))
                {
                    var excelImage = workSheet.Drawings.AddPicture("Logo", image);
                    excelImage.SetPosition(0, 15, 0, 30);
                }

                workSheet.Cells["F1:G1"].Merge = true;
                workSheet.Cells["F2:G2"].Merge = true;

                workSheet.Cells[1, 5].Value = "FECHA CONSULTA";
                workSheet.Cells[1, 6].Value = DateTime.Now.ToString("yyyy-MM-dd");
                workSheet.Cells[2, 5].Value = "PLANTA";
                workSheet.Cells[2, 6].Value = (Convert.ToInt32(planta) == 1) ? "DGO" : "NDD";

                workSheet.Cells["A4:G4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A4:G4"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet.Cells["A4:G4"].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));

                if (planta == "1")
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
                    "  WHERE [resultshift] = '" + ddl_shift.SelectedValue + "' AND [Plant] = " + Convert.ToInt32(planta) + " AND resultdateShift BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
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
                    "  WHERE [resultshift] = '" + ddl_shift.SelectedValue + "' AND [Plant] = " + Convert.ToInt32(planta) + " AND resultdateShift BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
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
                    " WHERE Plant = " + Convert.ToInt32(planta) + " and NumEmpleado in" +
                    " (SELECT distinct NumEmpleado from lended_staff where resultdateShift BETWEEN '" + txtFrom.Text + "'  AND '" + txtFrom.Text + "') ORDER BY NumEmpleado ASC"), true);

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
                MemoryStream memoryStream = new MemoryStream();
                HttpCookie cookie = new HttpCookie("generateFlag");
                cookie.Value = "Flag";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.AppendCookie(cookie);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=PERSONAL PRESTADO " + txtFrom.Text + "_TO_" + txtTo.Text + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            else if (ddl_type_report.SelectedValue == "200")
            {
                //Reporte 

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Reporte");

                workSheet.Row(1).Height = 30;
                workSheet.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet.Row(2).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet.Cells["A1:F2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A1:F2"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet.Row(1).Height = 30;
                workSheet.Row(2).Height = 30;

                using (System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("~/src/png/Logo.png")))
                {
                    var excelImage = workSheet.Drawings.AddPicture("Logo", image);
                    excelImage.SetPosition(0, 15, 0, 30);
                }

                workSheet.Cells["F1:F1"].Merge = true;
                workSheet.Cells["F2:F2"].Merge = true;

                workSheet.Cells[1, 4].Value = "FECHA CONSULTA";
                workSheet.Cells[1, 5].Value = DateTime.Now.ToString("yyyy-MM-dd");
                workSheet.Cells[2, 4].Value = "PLANTA";
                workSheet.Cells[2, 5].Value = (Convert.ToInt32(planta) == 1) ? "DGO" : "NDD";

                workSheet.Cells["A4:F4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A4:F4"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet.Cells["A4:F4"].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));


                workSheet.Cells["A4"].LoadFromDataTable(db.SP_READ_PERSONAL_ESCANEADO(ddl_shift.SelectedItem.Text, Convert.ToInt32(planta), txtFrom.Text, txtTo.Text), true);

                for (int renglon = 5; renglon <= workSheet.Dimension.End.Row; renglon++)
                {
                    if (workSheet.Cells[renglon, 5].Value.ToString() == "0")
                    {
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFC7CE"));
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#9C0006"));
                    }
                    else
                    {
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C6EFCE"));
                        workSheet.Cells["A" + renglon + ":F" + renglon].Style.Font.Color.SetColor(ColorTranslator.FromHtml("#006100"));
                    }

                }

                ExcelRange rg = workSheet.Cells[4, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                string tableName = "Table1";
                ExcelTable tab = workSheet.Tables.Add(rg, tableName);
                tab.TableStyle = TableStyles.Medium16;
                workSheet.View.ZoomScale = 75;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Row(4).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(3).Style.Numberformat.Format = "yyyy-mm-dd";



                workSheet.View.ShowGridLines = false;
                var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                var cellFont = allCells.Style.Font;
                cellFont.SetFromFont(new Font("Arial", 12));
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();


                MemoryStream memoryStream = new MemoryStream();
                HttpCookie cookie = new HttpCookie("generateFlag");
                cookie.Value = "Flag";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.AppendCookie(cookie);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=PERSONAL ESCANEADO (" + ddl_shift.SelectedItem.Text + ")  " + txtFrom.Text + "_TO_" + txtTo.Text + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            else if (ddl_type_report.SelectedValue == "203")
            {
                //Reporte 

                ExcelPackage excel = new ExcelPackage();


                //Pivote
                var workSheet2 = excel.Workbook.Worksheets.Add("info");
                //workSheet2.Cells["A1"].LoadFromDataTable(db.GetDataTable("SELECT [linea]" +
                //    "      ,[fechaBusq]" +
                //    "  FROM[daws].[dbo].[asistencia]" +
                //    "  where fechaBusq BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "' AND [turno] = '" + ddl_shift.SelectedItem.Text + "'"), true);



                workSheet2.Cells["A1"].LoadFromDataTable(db.GetDataTable("SELECT [nomLinea] as linea" +
                    "      ,[resultdate] as FechaBusq" +
                    "  FROM [daws].[dbo].[attendance] a" +
                    "  INNER JOIN [daws].[dbo].[lineas] b" +
                    "  on a.idLinea = b.idLinea" +
                    "  where [planta] = " + planta + " AND [resultdate2] BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "' AND [resultshift] = " + ddl_shift.SelectedValue), true);

                workSheet2.Column(2).Style.Numberformat.Format = "yyyy-mm-dd";

                if (workSheet2.Cells["A2"].Value != null)
                {
                    var workSheet3 = excel.Workbook.Worksheets.Add("Matriz");
                    workSheet3.TabColor = ColorTranslator.FromHtml("#92D050");
                    var dataRange = workSheet2.Cells[workSheet2.Dimension.Address];
                    var pivotTable = workSheet3.PivotTables.Add(workSheet3.Cells["B2"], dataRange, "PivotTable");
                    pivotTable.RowFields.Add(pivotTable.Fields["linea"]);
                    pivotTable.DataOnRows = false;
                    pivotTable.ColumnFields.Add(pivotTable.Fields["fechaBusq"]);
                    var field = pivotTable.DataFields.Add(pivotTable.Fields["fechaBusq"]);
                    field.Name = "Historial de asistencia";
                    field.Function = DataFieldFunctions.Count;
                    pivotTable.PivotTableStyle = PivotTableStyles.Dark2;
                    workSheet3.View.ShowGridLines = false;
                    workSheet2.Hidden = eWorkSheetHidden.VeryHidden;
                }


                MemoryStream memoryStream = new MemoryStream();
                HttpCookie cookie = new HttpCookie("generateFlag");
                cookie.Value = "Flag";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.AppendCookie(cookie);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=HISTORIAL DE ASISTENCIA" + txtFrom.Text + "_TO_" + txtTo.Text + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
            else if (Convert.ToInt32(ddl_type_report.SelectedValue) < 199)
            {
                //Reporte 

                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Estaciones");

                workSheet.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet.Cells["A1:D1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells["A1:D1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet.Row(1).Height = 30;


                workSheet.Cells["A1"].LoadFromDataTable(db.GetDataTableDaws("SELECT  [nomLinea] AS 'Linea',[estacion], [tipo], COUNT([numero]) AS 'operadores' " +
                    " FROM [daws].[dbo].[attendance] A" +
                    " INNER JOIN [daws].[dbo].[lineas]  B ON A.[idLinea] = B.[idLinea]" +
                    " WHERE A.[idLinea] = " + ddl_type_report.SelectedValue +
                    " AND [resultshift] = " + ddl_shift.SelectedValue +
                    " AND [resultdate2] BETWEEN '" + txtFrom.Text + "' AND '" + txtFrom.Text + "'" +
                    " GROUP BY [nomLinea], [estacion], [tipo],[orden] ORDER BY [orden]"), true);



                ExcelRange rg = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                string tableName = "Table1";
                ExcelTable tab = workSheet.Tables.Add(rg, tableName);
                tab.TableStyle = TableStyles.Medium16;
                workSheet.View.ZoomScale = 75;
                workSheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                workSheet.View.ShowGridLines = false;
                var allCells = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                var cellFont = allCells.Style.Font;
                cellFont.SetFromFont(new Font("Arial", 12));
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();

                // OPERADORES
                var workSheet2 = excel.Workbook.Worksheets.Add("OPERADORES");

                workSheet2.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet2.Cells["A1:L1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet2.Cells["A1:L1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet2.Row(1).Height = 30;


                workSheet2.Cells["A1"].LoadFromDataTable(db.SP_READ_PERSONAL_POR_LINEA(ddl_type_report.SelectedValue, txtFrom.Text, ddl_shift.SelectedValue), true);



                ExcelRange rg2 = workSheet2.Cells[1, 1, workSheet2.Dimension.End.Row, workSheet2.Dimension.End.Column];
                string tableName2 = "Table2";
                ExcelTable tab2 = workSheet2.Tables.Add(rg2, tableName2);
                tab2.TableStyle = TableStyles.Medium16;
                workSheet2.View.ZoomScale = 75;
                workSheet2.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                workSheet2.Cells["N2"].Value = "ROJO";
                workSheet2.Cells["O2"].Value = "AMARILLO";
                workSheet2.Cells["P2"].Value = "VERDE";
                workSheet2.Cells["N3"].Formula = "=COUNTIF($F:$F,\"<=3\")";
                workSheet2.Cells["O3"].Formula = "=COUNTIFS($F:$F,\">=4\",$F:$F,\"<=10\")";
                workSheet2.Cells["P3"].Formula = "=COUNTIFS($F:$F,\">=11\")";

                workSheet2.Cells["N2:N3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet2.Cells["N2:N3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFC7CE"));

                workSheet2.Cells["O2:O3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet2.Cells["O2:O3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#FFEB9C"));

                workSheet2.Cells["P2:P3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet2.Cells["P2:P3"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C6EFCE"));



                workSheet2.View.ShowGridLines = false;
                var allCells2 = workSheet2.Cells[1, 1, workSheet2.Dimension.End.Row, workSheet2.Dimension.End.Column];
                var cellFont2 = allCells2.Style.Font;
                cellFont2.SetFromFont(new Font("Arial", 12));
                workSheet2.Cells[workSheet2.Dimension.Address].AutoFitColumns();



                var workSheet3 = excel.Workbook.Worksheets.Add("DEFECTOS");

                workSheet3.Row(1).Style.Font.Color.SetColor(ColorTranslator.FromHtml("#FFFFFF"));
                workSheet3.Cells["A1:G1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet3.Cells["A1:G1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#002060"));
                workSheet3.Row(1).Height = 30;


                workSheet3.Cells["A1"].LoadFromDataTable(db.GetDataTableDaws("SELECT d.IDX, d.DefectDate, a.estacion, d.ActionWorker, d.codigo, d.categoria, d.defecto" +
                    " FROM attendance a" +
                    " INNER JOIN vistadefectos d" +
                    " ON a.numero COLLATE Korean_Wansung_CI_AS = d.ActionWorker COLLATE Korean_Wansung_CI_AS AND a.resultdate2 COLLATE Korean_Wansung_CI_AS = d.DefectDate" +
                    " WHERE d.DefectDate = '" + txtFrom.Text + "'" +
                    " AND a.idLinea = " + ddl_type_report.SelectedValue +
                    " AND a.resultshift =" + ddl_shift.SelectedValue +
                    " ORDER BY IDX"), true);


                MemoryStream memoryStream = new MemoryStream();
                HttpCookie cookie = new HttpCookie("generateFlag");
                cookie.Value = "Flag";
                cookie.Expires = DateTime.Now.AddDays(1);
                Response.AppendCookie(cookie);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + ddl_type_report.SelectedItem.Text + "  (" + ddl_shift.SelectedItem.Text + ")  " + txtFrom.Text + "_TO_" + txtTo.Text + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Si")
            {
                db.DeleteLN("DELETE FROM [dbo].[lended_staff] WHERE [IDX] = " + e.CommandArgument.ToString());
                gv_prestados.Visible = true;
                gv_prestados.DataSource = db.GetDataTable("SELECT A.[IDX],[NumEmpleado] as [Numero]" +
            "      ,[nombre]" +
            "      ,[D].[linea] as [Origen]" +
            "      ,[area_name] as [Destino]" +
            "      ,[resultdateShift] as [Fecha]" +
            "      ,[resultshift] as [Turno]" +
            "  FROM[lines_management].[dbo].[lended_staff] A" +
            "  INNER JOIN[lines_management].[dbo].[areas_lended] C" +
            "  ON[A].[area] = [C].[IDX]" +
            "  INNER JOIN[daws].[dbo].[headcount] D" +
            "  ON[A].[NumEmpleado] = [D].[numero] COLLATE SQL_Latin1_General_CP1_CI_AS" +
            "  WHERE [resultshift] = '" + ddl_shift.SelectedValue + "' AND [Plant] = " + Convert.ToInt32(planta) + " AND resultdateShift BETWEEN '" + txtFrom.Text + "' AND '" + txtTo.Text + "'" +
            "  ORDER BY [resultdateShift] ASC, [D].[linea] ASC, [area_name] ASC, [Turno] ASC ");
                gv_prestados.DataBind();
                string script = "ShowToast(1,'Empleado eliminado.');";
                ClientScript.RegisterStartupScript(this.GetType(), "showError", script, true);


            }
        }
    }
}
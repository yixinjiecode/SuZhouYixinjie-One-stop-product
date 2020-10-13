using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;


using System.Data;

using System.Web;

namespace BusinessLayer
{
    public class ExcelImportHelper
    {
        #region 创建Workbook

        //public static HSSFWorkbook CreateHSSFWorkbook()
        //{
        //    HSSFWorkbook workbook = new HSSFWorkbook();
        //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //    dsi.Company = "Lx Co., Ltd.";
        //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //    si.Author = "TX System";
        //    workbook.DocumentSummaryInformation = dsi;
        //    workbook.SummaryInformation = si;
        //    return workbook;
        //}

        #endregion

        #region  NPOI
        //public static IWorkbook GetHSSFWorkbookByPath(string path)
        //{
        //    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    {
        //        HSSFWorkbook workbook = new HSSFWorkbook(file);
        //        return workbook;
        //    }
        //}

        //public static IWorkbook GetXSSFWorkbookByPath(string path)
        //{
        //    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    {
        //        XSSFWorkbook workbook = new XSSFWorkbook(file);
        //        return workbook;
        //    }
        //}

        //public static object GetValue(ICell cell)
        //{
        //    switch (cell.CellType)
        //    {
        //        case CellType.Blank: //BLANK:
        //            return null;
        //        case CellType.Boolean: //BOOLEAN:
        //            return cell.BooleanCellValue;
        //        case CellType.Numeric: //NUMERIC:
        //            return cell.NumericCellValue;
        //        case CellType.String: //STRING:
        //            return cell.StringCellValue;
        //        case CellType.Error: //ERROR:
        //            return cell.ErrorCellValue;
        //        case CellType.Formula: //FORMULA:
        //            HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
        //            return GetValue(e.Evaluate(cell));
        //        default:
        //            return "=" + cell.CellFormula;
        //    }
        //}

        //public static object GetValue(CellValue cellValue)
        //{
        //    switch (cellValue.CellType)
        //    {
        //        case CellType.Boolean: //BOOLEAN:
        //            return cellValue.BooleanValue;
        //        case CellType.Numeric: //NUMERIC:
        //            return cellValue.NumberValue;
        //        case CellType.String: //STRING:
        //            return cellValue.StringValue;
        //        case CellType.Error: //ERROR:
        //            return cellValue.ErrorValue;
        //        case CellType.Blank: //BLANK:
        //        default:
        //            return null;
        //    }
        //}

        //public static MemoryStream WriteToStream(string[] colHeads, string[] colNames, DataTable dataSource)
        //{
        //    HSSFWorkbook workbook = WriteHSSFWorkbook(colHeads, colNames, dataSource);
        //    using (MemoryStream file = new MemoryStream())
        //    {
        //        workbook.Write(file);
        //        return file;
        //    }
        //}

        //public static HSSFWorkbook WriteHSSFWorkbook(string[] colHeads, string[] colNames, DataTable dataSource)
        //{
        //    HSSFWorkbook workbook = CreateHSSFWorkbook();

        //    ICellStyle headStyle = workbook.CreateCellStyle();
        //    headStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;
        //    headStyle.FillPattern = FillPattern.SolidForeground;
        //    headStyle.BorderTop = BorderStyle.Thin;
        //    headStyle.BorderRight = BorderStyle.Thin;
        //    headStyle.BorderBottom = BorderStyle.Thin;
        //    headStyle.BorderLeft = BorderStyle.Thin;

        //    ICellStyle contentStyle = workbook.CreateCellStyle();
        //    contentStyle.BorderTop = BorderStyle.Thin;
        //    contentStyle.BorderRight = BorderStyle.Thin;
        //    contentStyle.BorderBottom = BorderStyle.Thin;
        //    contentStyle.BorderLeft = BorderStyle.Thin;

        //    ISheet sheet = workbook.CreateSheet();
        //    WriteHead(sheet, colHeads, headStyle);

        //    WriteData(sheet, colNames, dataSource, contentStyle);

        //    for (int i = 0; i < colHeads.Length; i++)
        //    {
        //        sheet.AutoSizeColumn(i);
        //    }

        //    return workbook;
        //}

        //private static void WriteHead(ISheet sheet, string[] colHeads, ICellStyle headStyle)
        //{
        //    IRow row = sheet.CreateRow(0);
        //    for (int i = 0; i < colHeads.Length; i++)
        //    {
        //        ICell cell = row.CreateCell(i, CellType.String);
        //        cell.CellStyle = headStyle;
        //        cell.SetCellValue(colHeads[i]);
        //    }
        //}

        //private static void WriteData(ISheet sheet, string[] colNames, DataTable dataSource, ICellStyle contentStyle)
        //{
        //    int rowNum = 1;
        //    foreach (DataRow dr in dataSource.Rows)
        //    {
        //        for (int i = 0; i < colNames.Length; i++)
        //        {
        //            if (dataSource.Columns.Contains(colNames[i]))
        //            {
        //                ICell cell = WriteCell(sheet, rowNum, i, dr[colNames[i]]);
        //                cell.CellStyle = contentStyle;
        //            }
        //        }
        //        rowNum++;
        //    }
        //}

        //#region 设置Cell的值

        //public static ICell WriteCell(ISheet sheet, int rowNum, int cellNum, object cellValue)
        //{
        //    IRow row = sheet.GetRow(rowNum);
        //    if (row == null)
        //    {
        //        row = sheet.CreateRow(rowNum);
        //    }
        //    return WriteCell(row, cellNum, cellValue);
        //}

        //public static ICell WriteCell(IRow row, int cellNum, object cellValue)
        //{
        //    ICell cell = row.GetCell(cellNum);
        //    if (cell == null)
        //    {
        //        cell = row.CreateCell(cellNum);
        //    }
        //    if (cellValue is string)
        //    {
        //        cell.SetCellValue((string)cellValue);
        //    }
        //    else if (cellValue is bool)
        //    {
        //        cell.SetCellValue((bool)cellValue);
        //    }
        //    else if (cellValue is DateTime)
        //    {
        //        cell.SetCellValue(((DateTime)cellValue).ToString("yyyy-MM-dd"));
        //    }
        //    else if (cellValue.GetType().IsValueType)
        //    {
        //        cell.SetCellValue(Convert.ToDouble(cellValue));
        //    }
        //    else
        //    {
        //        cell.SetCellValue(cellValue.ToString());
        //    }
        //    return cell;
        //}

        //#endregion

        //public static bool CellHasValue(ICell cell)
        //{
        //    if (cell == null)
        //    {
        //        return false;
        //    }

        //    if (string.IsNullOrEmpty(cell.ToString()))
        //    {
        //        return false;
        //    }

        //    return true;
        //}
        #endregion
        /// <summary>
        ///可导出多个sheet表
        /// </summary>
        /// <param name="Author">作者</param>
        /// <param name="Company">公司</param>
        /// <param name="dt">多个DataTable</param>
        /// <param name="fileName">文件名</param>
        public static void PushExcelToClientEx(HttpContext curContext, string Author, string Company, DataTable[] dt, string fileName)
        {
            if (!fileName.Contains(".xls"))
            {
                fileName += ".xls";
            }

            StringBuilder sbBody = new StringBuilder();
            StringBuilder sbSheet = new StringBuilder();

            sbBody.AppendFormat(
                    "MIME-Version: 1.0\r\n" +
                    "X-Document-Type: Workbook\r\n" +
                    "Content-Type: multipart/related; boundary=\"-=BOUNDARY_EXCEL\"\r\n\r\n" +
                    "---=BOUNDARY_EXCEL\r\n" +
                    "Content-Type: text/html; charset=\"gbk\"\r\n\r\n" +
                    "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n" +
                    "xmlns:x=\"urn:schemas-microsoft-com:office:excel\">\r\n\r\n" +
                    "<head>\r\n" +
                    "<xml>\r\n" +
                    "<o:DocumentProperties>\r\n" +
                    "<o:Author>{0}</o:Author>\r\n" +
                    "<o:LastAuthor>{0}</o:LastAuthor>\r\n" +
                    "<o:Created>{1}</o:Created>\r\n" +
                    "<o:LastSaved>{1}</o:LastSaved>\r\n" +
                    "<o:Company>{2}</o:Company>\r\n" +
                    "<o:Version>11.5606</o:Version>\r\n" +
                    "</o:DocumentProperties>\r\n" +
                    "</xml>\r\n" +
                    "<xml>\r\n" +
                    "<x:ExcelWorkbook>\r\n" +
                    "<x:ExcelWorksheets>\r\n"
                   , Author
                   , DateTime.Now.ToString()
                   , Company);

            foreach (var d in dt)
            {
                string gid = Guid.NewGuid().ToString();
                sbBody.AppendFormat("<x:ExcelWorksheet>\r\n" +
                    "<x:Name>{0}</x:Name>\r\n" +
                    "<x:WorksheetSource HRef=\"cid:{1}\"/>\r\n" +
                    "</x:ExcelWorksheet>\r\n"
                    , d.TableName.Replace(":", "").Replace("\\", "").Replace("/", "").Replace("?", "").Replace("*", "").Replace("[", "").Replace("]", "").Trim()
                    , gid);


                sbSheet.AppendFormat(
                 "---=BOUNDARY_EXCEL\r\n" +
                 "Content-ID: {0}\r\n" +
                 "Content-Type: text/html; charset=\"gbk\"\r\n\r\n" +
                 "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n" +
                 "xmlns:x=\"urn:schemas-microsoft-com:office:excel\">\r\n\r\n" +
                 "<head>\r\n" +
                 "<xml>\r\n" +
                 "<x:WorksheetOptions>\r\n" +
                 "<x:ProtectContents>False</x:ProtectContents>\r\n" +
                 "<x:ProtectObjects>False</x:ProtectObjects>\r\n" +
                 "<x:ProtectScenarios>False</x:ProtectScenarios>\r\n" +
                 "</x:WorksheetOptions>\r\n" +
                 "</xml>\r\n" +
                 "</head>\r\n" +
                 "<body>\r\n"
                 , gid);

                sbSheet.Append("<table border='1'>");
                sbSheet.Append("<tr style='background-color: #CCC;'>");
                for (int i = 0; i < d.Columns.Count; i++)
                {
                    sbSheet.AppendFormat("<td style='vnd.ms-excel.numberformat: @;font-weight:bold'>{0}</td>", d.Columns[i].ColumnName);
                }
                sbSheet.Append("</tr>");
                for (int j = 0; j < d.Rows.Count; j++)
                {
                    sbSheet.Append("<tr>");
                    for (int k = 0; k < d.Columns.Count; k++)
                    {
                        sbSheet.AppendFormat("<td style='vnd.ms-excel.numberformat: @;'>{0}</td>", Convert.ToString(d.Rows[j][k]));
                    }
                    sbSheet.Append("</tr>");
                }
                sbSheet.Append("</table>");
                sbSheet.Append("</body>\r\n" +
                    "</html>\r\n\r\n");
            }

            StringBuilder sb = new StringBuilder(sbBody.ToString());

            sb.Append("</x:ExcelWorksheets>\r\n" +
                "</x:ExcelWorkbook>\r\n" +
               "</xml>\r\n" +
                "</head>\r\n" +
                "</html>\r\n\r\n");

            sb.Append(sbSheet.ToString());

            sb.Append("---=BOUNDARY_EXCEL--");

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Buffer = true;

            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            //    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gbk");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.Write(sb.ToString());
            HttpContext.Current.Response.End();



         




        }




        /// <summary>
        /// DataTable导出到Excel,解决 Excel 2010导出 问题
        /// </summary>
        /// <param name="pData">DataTable</param>
        /// <param name="pFileName">导出文件名</param>
        /// <param name="pHeader">导出标题以|分割</param>
        public static void DataTableExcel(System.Data.DataTable pData, string pFileName, string pHeader)
        {
            System.Web.UI.WebControls.DataGrid dgExport = null;
            // 当前对话 
            System.Web.HttpContext curContext = System.Web.HttpContext.Current;
            // IO用于导出并返回excel文件 
            System.IO.StringWriter strWriter = null;
            System.Web.UI.HtmlTextWriter htmlWriter = null;
            if (pData != null)
            {
                string UserAgent = curContext.Request.ServerVariables["http_user_agent"].ToLower();
                if (UserAgent.IndexOf("firefox") == -1)//火狐浏览器
                    pFileName = HttpUtility.UrlEncode(pFileName, System.Text.Encoding.UTF8);
                curContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + pFileName + ".xls");
                curContext.Response.ContentType = "application/ms-excel";
                strWriter = new System.IO.StringWriter();
                htmlWriter = new System.Web.UI.HtmlTextWriter(strWriter);
                // 为了解决dgData中可能进行了分页的情况，需要重新定义一个无分页的DataGrid 
                dgExport = new System.Web.UI.WebControls.DataGrid();
                dgExport.DataSource = pData.DefaultView;
                dgExport.AllowPaging = false;
                dgExport.ShowHeader = false;//去掉标题
                dgExport.DataBind();
                string[] arrHeader = pHeader.Split(',');
                string strHeader = "<table border=\"1\" style=\"background-color:Green;font-weight:bold;\"><tr>";
                foreach (string j in arrHeader)
                {
                    strHeader += "<td>" + j.ToString() + "</td>";
                }
                strHeader += "</tr></table>";
                // 返回客户端 
                dgExport.RenderControl(htmlWriter);
                string strMeta = "<meta http-equiv=\"content-type\" content=\"application/ms-excel; charset=UTF-8\"/>";
                curContext.Response.Write(strMeta + strHeader + strWriter.ToString());

                curContext.Response.End(); 
            }
        }


        /// <summary>  
        /// DataTable导出到Excel  
        /// </summary>  
        /// <param name="table">DataTable类型的数据源</param>  
        /// <param name="file">需要导出的文件路径</param>  
        public void dataTableToCsv(DataTable table, string file)
        {
            string title = "";
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格  
            }
            title = title.Substring(0, title.Length - 1) + "\n";
            sw.Write(title);
            foreach (DataRow row in table.Rows)
            {
                string line = "";
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格  
                }
                line = line.Substring(0, line.Length - 1) + "\n";
                sw.Write(line);
            }
            sw.Close();
            fs.Close();
        }
    }
}

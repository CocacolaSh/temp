using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Ocean.Core.Npoi
{
    /// <summary>
    /// Excel生成操作类
    /// 注意：操作Excel2003与操作Excel2007使用的是不同的命名空间下的内容
    ///使用NPOI.HSSF.UserModel空间下的HSSFWorkbook操作Excel2003
    ///使用NPOI.XSSF.UserModel空间下的XSSFWorkbook操作Excel2007
    /// </summary>
    public class NPOIHelper
    {
        public static int HeaderRowsIndex { set; get; }

        public static int HeaderCellsIndex { set; get; }

        public static void FindHeader(ISheet sheet, string identifying)
        {
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);

                if (row == null)
                {
                    continue;
                }

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    if (row.GetCell(j) == null)
                    {
                        continue;
                    }

                    string value = row.GetCell(j).ToString();

                    if (!string.IsNullOrWhiteSpace(value) && string.Equals(value, identifying, StringComparison.OrdinalIgnoreCase))
                    {
                        HeaderRowsIndex = i;
                        HeaderCellsIndex = j;
                    }
                }
            }
        }

        #region Excel2003
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xls)  
        /// </summary>
        /// <param name="file">文件源</param>
        /// <param name="identifying">表头标识</param>
        /// <param name="sheetIndex">工作簿索引</param>
        /// <returns></returns>
        public static DataTable ExcelToTableForXLS(string file, string identifying, int sheetIndex = 0)
        {
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new HSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0);
                FindHeader(sheet, identifying);
                //表头  
                IRow header = sheet.GetRow(HeaderRowsIndex);
                List<int> columns = new List<int>();

                for (int i = HeaderCellsIndex; i <= header.LastCellNum; i++)
                {
                    object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell);

                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    }

                    columns.Add(i);
                }
                //数据  
                for (int i = HeaderRowsIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    IRow rowData = sheet.GetRow(i);

                    if (rowData == null)
                    {
                        continue;
                    }

                    for (int j = 0; j <= columns.Count - 1; j++)
                    {
                        ICell cellData = rowData.GetCell(j + HeaderCellsIndex);

                        if (cellData == null)
                        {
                            continue;
                        }

                        dr[j] = GetValueTypeForXLS(rowData.GetCell(j + HeaderCellsIndex) as HSSFCell, dt.Columns[j].ColumnName.Contains("日期"));

                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }

                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }

            return dt;
        }
        public static DataTable ExcelToTableForXLS(Stream fileStream, string identifying,string mergedCells, int sheetIndex = 0)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook = new HSSFWorkbook(fileStream);
            ISheet sheet = workbook.GetSheetAt(0);
            FindHeader(sheet, identifying);
            //表头  
            IRow header = sheet.GetRow(HeaderRowsIndex);
            List<int> columns = new List<int>();
            int c = 0;
           IList<int> mergedIndexs =new List<int>();

            object preColumnValue=null;
            int cellIndex = 0;
            for (int i = HeaderCellsIndex; i <= header.LastCellNum; i++)
            {
                object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell);
                if (obj == null || obj.ToString() == string.Empty)
                {
                    dt.Columns.Add(new DataColumn("Columns" + c.ToString()));
                    columns.Add(i);
                    c++;
                }
                else
                {
                    if (!dt.Columns.Contains(obj.ToString().Trim()))
                    {
                        dt.Columns.Add(new DataColumn(obj.ToString().Trim()));
                        columns.Add(i);
                        if (!string.IsNullOrEmpty(mergedCells) && mergedCells.Contains(obj.ToString().Trim()))
                        {
                            mergedIndexs.Add(cellIndex);
                        }
                    }

                }
                cellIndex++;
            }
            //数据  
            for (int i = HeaderRowsIndex + 1; i <= sheet.LastRowNum; i++)
            {
                DataRow dr = dt.NewRow();
                bool hasValue = false;
                IRow rowData = sheet.GetRow(i);

                if (rowData == null)
                {
                    continue;
                }

                for (int j = 0; j <= columns.Count - 1; j++)
                {
                    ICell cellData = rowData.GetCell(columns[j]);

                    if (cellData == null)
                    {
                        continue;
                    }
                    HSSFCell cell = rowData.GetCell(columns[j]) as HSSFCell;
                    dr[j] = GetValueTypeForXLS(cell, dt.Columns[j].ColumnName.Contains("日期"));
                    if (cell.IsMergedCell && mergedIndexs.Where(mi=>mi==j).Count()>0)
                    {
                        if (string.IsNullOrEmpty(dr[j].ToString()))
                        {
                            dr[j] = preColumnValue;
                        }
                        preColumnValue = dr[j];
                    }
                    if (dr[j] != null && dr[j].ToString() != string.Empty && dr[j].ToString().Trim()!=string.Empty)
                    {
                        hasValue = true;
                    }
                }

                if (hasValue)
                {
                    dt.Rows.Add(dr);
                }
            }
            fileStream.Close();
            fileStream.Dispose();
            return dt;
        }
        /// <summary>  
        /// 将DataTable数据导出到Excel文件中(xls)  
        /// </summary>  
        public static void TableToExcelForXLS(DataTable dt, string file, string name)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(name);
            //表头  
            IRow rowHeader = sheet.CreateRow(0);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = rowHeader.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }
            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow rowData = sheet.CreateRow(i + 1);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = rowData.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //转为字节数组  
            using (MemoryStream stream = new MemoryStream())
            {
                workbook.Write(stream);
                var arrByte = stream.ToArray();
                //保存为Excel文件  
                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(arrByte, 0, arrByte.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>  
        /// 获取单元格类型(xls)  
        /// </summary>  
        private static object GetValueTypeForXLS(HSSFCell cell, bool dateType = false)
        {
            if (cell == null)
                return null;

            if (dateType && cell.CellType == CellType.Numeric)
            {
                return cell.DateCellValue;
            }

            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }

        
        #endregion  

        #region Excel2007
        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xlsx)  
        /// </summary>  
        /// <param name="file"></param>  
        /// <returns></returns>  
        public static DataTable ExcelToTableForXLSX(string file, string identifying, int sheetIndex = 0)
        {
            DataTable dt = new DataTable();

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                ISheet sheet = xssfworkbook.GetSheetAt(0);
                FindHeader(sheet, identifying);
                //表头  
                IRow header = sheet.GetRow(HeaderRowsIndex);
                List<int> columns = new List<int>();

                for (int i = HeaderCellsIndex; i <= header.LastCellNum; i++)
                {
                    object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);

                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    }

                    columns.Add(i);
                }
                //数据  
                for (int i = HeaderRowsIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    IRow rowData = sheet.GetRow(i);

                    if (rowData == null)
                    {
                        continue;
                    }

                    for (int j = 0; j <= columns.Count - 1; j++)
                    {
                        ICell cellData = rowData.GetCell(j + HeaderCellsIndex);

                        if (cellData == null)
                        {
                            continue;
                        }

                        dr[j] = GetValueTypeForXLSX(rowData.GetCell(j + HeaderCellsIndex) as XSSFCell, dt.Columns[j].ColumnName.Contains("日期"));

                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }

                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }

            return dt;
        }

        /// <summary>  
        /// 将Excel文件中的数据读出到DataTable中(xlsx)  
        /// </summary>  
        /// <param name="file"></param>  
        /// <returns></returns>  
        public static DataTable ExcelToTableForXLSX(Stream fileStream, string identifying, int sheetIndex = 0)
        {
            DataTable dt = new DataTable();

            XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
                ISheet sheet = xssfworkbook.GetSheetAt(0);
                FindHeader(sheet, identifying);
                //表头  
                IRow header = sheet.GetRow(HeaderRowsIndex);
                List<int> columns = new List<int>();

                for (int i = HeaderCellsIndex; i <= header.LastCellNum; i++)
                {
                    object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);

                    if (obj == null || obj.ToString().Trim() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(obj.ToString().Trim()));
                    }

                    columns.Add(i);
                }
                //数据  
                for (int i = HeaderRowsIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    IRow rowData = sheet.GetRow(i);

                    if (rowData == null)
                    {
                        continue;
                    }

                    for (int j = 0; j <= columns.Count - 1; j++)
                    {
                        ICell cellData = rowData.GetCell(j + HeaderCellsIndex);

                        if (cellData == null)
                        {
                            continue;
                        }

                        dr[j] = GetValueTypeForXLSX(rowData.GetCell(j + HeaderCellsIndex) as XSSFCell, dt.Columns[j].ColumnName.Contains("日期"));

                        if (dr[j] != null && dr[j].ToString().Trim() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }

                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
                fileStream.Close();
                fileStream.Flush();
            return dt;
        }

        /// <summary>  
        /// 将DataTable数据导出到Excel文件中(xlsx)  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <param name="file"></param>  
        public static void TableToExcelForXLSX(DataTable dt, string file, string name)
        {
            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
            ISheet sheet = xssfworkbook.CreateSheet(name);
            //表头  
            IRow row = sheet.CreateRow(0);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }
            //数据  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //转为字节数组  
            MemoryStream stream = new MemoryStream();
            xssfworkbook.Write(stream);
            var buf = stream.ToArray();
            //保存为Excel文件  
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>  
        /// 获取单元格类型(xlsx)  
        /// </summary>  
        /// <param name="cell"></param>  
        /// <returns></returns>  
        private static object GetValueTypeForXLSX(XSSFCell cell, bool dateType = false)
        {
            if (cell == null)
            {
                return null;
            }

            if (dateType && cell.CellType == CellType.Numeric)
            {
                return cell.DateCellValue;
            }

            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion
    }
}
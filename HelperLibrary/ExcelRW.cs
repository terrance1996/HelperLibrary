using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace HelperLibrary
{
    public class ExcelRW
    {
        public static void CreatExcelFile(string fileName, DataTable table, string sheetName)
        {

            var newFile = new FileInfo(fileName);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets.Add(sheetName);
                ws.Cells.LoadFromDataTable(table, true);

                ws.Cells["A:P"].Style.Numberformat.Format = null;
                ws.Cells["K:P"].Style.Numberformat.Format = "$###,##0.00";
                xlPackage.Save();
            }
        }

        public static DataTable ReadXLSX(string path, int sheetNumber, int headerRow=1, bool hasHeader = true)
        {
            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets[sheetNumber];
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[headerRow, 1, headerRow, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? headerRow + 1 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using System.IO;
using System.Data;

namespace HelperLibrary.NetCore
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
                xlPackage.Save();
            }
        }

        public static DataTable ReadXLSX(string path, int sheetNumber, int headerRow = 1, bool hasHeader = true)
        {
            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets[sheetNumber];
                DataTable tbl = new DataTable();
                int dupColCount = 0;
                var range = ws.Cells[headerRow, 1, headerRow, ws.Dimension.End.Column];
                foreach (var firstRowCell in ws.Cells[headerRow, 1, headerRow, ws.Dimension.End.Column])
                {
                    if (tbl.Columns.Contains(firstRowCell.Text))
                    {
                        dupColCount++;
                        tbl.Columns.Add(hasHeader ? $"{firstRowCell.Text}_{dupColCount}" : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    else
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
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

        public static DataTable ReadXlsxColumns(string path, int sheetNumber, int columnCount, int headerRow = 1, bool hasHeader = true)
        {
            using (var pck = new ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets[sheetNumber];
                DataTable tbl = new DataTable();
                int dupColCount = 0;
                var range = ws.Cells[headerRow, 1, headerRow, columnCount];
                foreach (var firstRowCell in ws.Cells[headerRow, 1, headerRow, columnCount])
                {
                    if (tbl.Columns.Contains(firstRowCell.Text))
                    {
                        dupColCount++;
                        tbl.Columns.Add(hasHeader ? $"{firstRowCell.Text}_{dupColCount}" : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                    else
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                    }
                }
                var startRow = hasHeader ? headerRow + 1 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, columnCount];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

        public static void CreatExcelFileForREX(string fileName, DataTable table, DataTable duplicateCodes = null)
        {

            var newFile = new FileInfo(fileName);
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets.Add("Product Master");


                ws.Cells.LoadFromDataTable(table, true);
                if (duplicateCodes != null)
                {
                    var wsDuplicates = xlPackage.Workbook.Worksheets.Add("Duplicate Codes");
                    wsDuplicates.Cells.LoadFromDataTable(duplicateCodes, true);
                }


                ws.InsertRow(1, 9);
                ws.Cells["A:P"].Style.Numberformat.Format = null;
                ws.Cells["K:P"].Style.Numberformat.Format = "$###,##0.00";
                xlPackage.Save();
            }
        }

    }

}

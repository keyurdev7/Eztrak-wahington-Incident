using System;
using System.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;

namespace Helpers.File
{
    public static class OpenXMLExcelHelper
    {
        public static byte[] ExportToExcelWithColumnDataType(DataTable table, ILogger _logger, string sheetName = "Sheet 1")
        {
            _logger.LogInformation("START: ExportToExcelWithColumnDataType", Array.Empty<object>());
            MemoryStream stream = new MemoryStream();
            SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
            WorkbookPart part = document.AddWorkbookPart();
            part.Workbook = new Workbook();
            WorksheetPart part2 = part.AddNewPart<WorksheetPart>();
            SheetData data = new SheetData();
            OpenXmlElement[] childElements = new OpenXmlElement[] { data };
            part2.Worksheet = new Worksheet(childElements);
            Sheet newChild = new Sheet
            {
                Id = document.WorkbookPart.GetIdOfPart(part2),
                SheetId = 1,
                Name = sheetName
            };
            document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets()).AppendChild<Sheet>(newChild);
            Row row = new Row();
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            foreach (DataColumn column in table.Columns)
            {
                dictionary.Add(column.ColumnName, IsNumeric(column));
                Cell cell = new Cell
                {
                    DataType = new EnumValue<CellValues>(CellValues.String),
                    CellValue = new CellValue(column.ColumnName)
                };
                row.AppendChild<Cell>(cell);
            }
            data.AppendChild<Row>(row);
            foreach (DataRow row2 in table.Rows)
            {
                Row row3 = new Row();
                foreach (KeyValuePair<string, bool> pair in dictionary)
                {
                    Cell cell2 = new Cell();
                    if (pair.Value)
                    {
                        cell2.DataType = new EnumValue<CellValues>(CellValues.Number);
                        cell2.CellValue = new CellValue(row2[pair.Key].ToString());
                    }
                    else
                    {
                        cell2.DataType = new EnumValue<CellValues>(CellValues.String);
                        cell2.CellValue = new CellValue(row2[pair.Key].ToString());
                    }
                    row3.AppendChild<Cell>(cell2);
                }
                data.AppendChild<Row>(row3);
            }
            part.Workbook.Save();
            document.Close();
            _logger.LogInformation("END: ExportToExcelWithColumnDataType", Array.Empty<object>());
            return stream.ToArray();
        }

        private static bool IsNumeric(DataColumn col)
        {
            if (col == null)
            {
                return false;
            }
            Type[] typeArray1 = new Type[] { typeof(byte), typeof(decimal), typeof(double), typeof(short), typeof(int), typeof(long), typeof(sbyte), typeof(float), typeof(ushort), typeof(uint), typeof(ulong) };
            return Enumerable.Contains<Type>(typeArray1, col.DataType);
        }


        public static void AddDataTableColumnIfNotExcluded<T>(this DataTable dt, string columnName)
        {
            dt.Columns.Add(columnName, typeof(T));
        }

        public static void AddRowItemColumnIfNotExcluded(this DataRow dr, object value, int colIdx)
        {
            object obj = ((value != null) ? value : DBNull.Value);
            dr[colIdx] = obj;
        }
    }
}


using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Repositories.Services.ExcelHelper.Interface;
using System.Data;
using System.Reflection;

namespace Repositories.Services.ExcelHelper
{
    public class ExcelHelper : IExcelHelper
    {
        private readonly ILogger<ExcelHelper> _logger;

        public ExcelHelper(ILogger<ExcelHelper> logger)
        {
            _logger = logger;
        }
        public List<DataTable> GetData(Stream stream)
        {
            try
            {
                var dtList = new List<DataTable>();

                //Started reading the Excel file.
                using (XLWorkbook workbook = new XLWorkbook(stream))
                {
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        bool FirstRow = true;
                        //Range for reading the cells based on the last cell used.
                        var dt = new DataTable();
                        string readRange = "1:1";
                        foreach (IXLRow row in worksheet.RowsUsed())
                        {
                            //If Reading the First Row (used) then add them as column name
                            if (FirstRow)
                            {
                                //Checking the Last cell used for column generation in datatable
                                readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    dt.Columns.Add(cell.Value.ToString());
                                }
                                FirstRow = false;
                            }
                            else
                            {
                                //Adding a Row in datatable
                                dt.Rows.Add();
                                int cellIndex = 0;
                                //Updating the values of datatable
                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }
                            }
                        }
                        dtList.Add(dt);
                    }
                    return dtList;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetData(Stream stream) method in ExcelReader threw an exception");
                return new List<DataTable>();
            }
        }

        public List<DataTable> GetData(string filePath)
        {
            try
            {
                var dtList = new List<DataTable>();
                if (System.IO.File.Exists(filePath))
                {
                    //Started reading the Excel file.
                    using (XLWorkbook workbook = new XLWorkbook(filePath))
                    {
                        foreach (var worksheet in workbook.Worksheets)
                        {
                            bool FirstRow = true;
                            //Range for reading the cells based on the last cell used.
                            var dt = new DataTable();
                            string readRange = "1:1";
                            foreach (IXLRow row in worksheet.RowsUsed())
                            {
                                //If Reading the First Row (used) then add them as column name
                                if (FirstRow)
                                {
                                    //Checking the Last cellused for column generation in datatable
                                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                    foreach (IXLCell cell in row.Cells(readRange))
                                    {
                                        dt.Columns.Add(cell.Value.ToString());
                                    }
                                    FirstRow = false;
                                }
                                else
                                {
                                    //Adding a Row in datatable
                                    dt.Rows.Add();
                                    int cellIndex = 0;
                                    //Updating the values of datatable
                                    foreach (IXLCell cell in row.Cells(readRange))
                                    {
                                        dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                        cellIndex++;
                                    }
                                }
                            }
                            dtList.Add(dt);
                        }
                        return dtList;
                    }
                }
                return new List<DataTable>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetData(string filePath) method in ExcelReader threw an exception");
                return new List<DataTable>();
            }
        }

        public bool AddModel<T>(List<DataTable> dataLists, int ind, int startInd, List<T> itemList, Dictionary<string, string>? columnMappings = null) where T : new()
        {
            try
            {
                for (int i = startInd; i < dataLists[ind].Rows.Count; i++)
                {
                    var item = new T();
                    for (int j = 0; j < dataLists[ind].Columns.Count; j++)
                    {
                        var columnName = dataLists[ind].Columns[j].ColumnName.ToString();
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            if (columnMappings != null && columnMappings.Any(x => x.Key == columnName))
                                columnName = columnMappings.Where(x => x.Key == columnName).Select(x => x.Value).FirstOrDefault();
                            SetObjectProperty(item, columnName, (dataLists[ind].Rows[i][j]).ToString());
                        }
                    }
                    itemList.Add(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"bool AddModel<T> method in ExcelReader threw an exception");
                return false;
            }
        }
        void SetObjectProperty(object myObject, string propertyName, string value)
        {

            try
            {
                foreach (PropertyInfo pi in myObject.GetType().GetProperties())
                {
                    if (pi.Name == propertyName.Trim())
                    {
                        //casted the property to its actual type dynamically
                        var type = pi.PropertyType;
                        if (type == typeof(float))
                            if (value != "")
                                pi.SetValue(myObject, float.Parse(value));
                        if (type == typeof(string))
                            pi.SetValue(myObject, value);
                        if (type == typeof(double))
                            if (value != "")
                                pi.SetValue(myObject, double.Parse(value));
                        if (type == typeof(int))
                            if (value != "")
                                pi.SetValue(myObject, int.Parse(value));
                        if (type == typeof(Int64))
                            if (value != "")
                                pi.SetValue(myObject, long.Parse(value));
                        if (type == typeof(Guid))
                            if (value != "")
                                pi.SetValue(myObject, Guid.Parse(value));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<T> AddModel<T>(List<DataTable> dataLists, int ind, int startInd, Dictionary<string, string>? columnMappings = null) where T : new()
        {
            var itemList = new List<T>();
            try
            {
                for (int i = startInd; i < dataLists[ind].Rows.Count; i++)
                {
                    var item = new T();
                    for (int j = 0; j < dataLists[ind].Columns.Count; j++)
                    {
                        var columnName = dataLists[ind].Columns[j].ColumnName.ToString();
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            if (columnMappings != null && columnMappings.Any(x => x.Key == columnName))
                                columnName = columnMappings.Where(x => x.Key == columnName).Select(x => x.Value).FirstOrDefault();
                            SetObjectProperty(item, columnName, (dataLists[ind].Rows[i][j]).ToString());
                        }
                    }
                    itemList.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"List<T> AddModel<T> method in ExcelReader threw an exception");
            }
            return itemList;
        }

        public List<string> GetColumnNamesFromExcel(Stream stream, int columnStartIndex, int columnEndIndex)
        {
            var columnNames = new List<string>();

            using (XLWorkbook workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);
                var headerRow = worksheet.Row(1);

                // Assuming columns K to O
                for (int i = columnStartIndex; i <= columnEndIndex; i++)
                {
                    var cell = headerRow.Cell(i);
                    columnNames.Add(cell.Value.ToString());
                }
            }

            return columnNames;
        }
    }
}
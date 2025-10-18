using System.Data;

namespace Repositories.Services.ExcelHelper.Interface
{
    public interface IExcelHelper
    {
        bool AddModel<T>(List<DataTable> dataLists, int ind, int startInd, List<T> itemList, Dictionary<string, string>? columnMappings = null) where T : new();
        List<T> AddModel<T>(List<DataTable> dataLists, int ind, int startInd, Dictionary<string, string>? columnMappings = null) where T : new();
        List<DataTable> GetData(Stream stream);
        List<DataTable> GetData(string filePath);
        List<string> GetColumnNamesFromExcel(Stream stream, int columnStartIndex, int columnEndIndex);
    }
}
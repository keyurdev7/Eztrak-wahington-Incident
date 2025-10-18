using Enums;
using ViewModels.DataTable;

namespace ViewModels
{
    public class TaskTypeSearchViewModel : BaseDateSearchModel
    {
        public string? Code { get; set; }
        public string? Title { get; set; }
        public TaskCatalog? Category { get; set; }
        public override string OrderByColumn { get; set; } = "Code";


    }
}

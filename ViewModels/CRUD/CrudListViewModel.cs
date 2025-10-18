using Pagination;
using System.Text.Json;
using ViewModels.DataTable;

namespace ViewModels.CRUD
{
    public class CrudListViewModel
    {
        public CrudListViewModel()
        {
            SearchViewPath = "_Search";
        }
        public string Title { get; set; }
        public bool HideTitle { get; set; } = false;
        public List<DataTableViewModel> DatatableColumns { get; set; }
        public IBaseSearchModel Filters { get; set; }
        public string DatatableColumnsJson
        {
            get
            {
                return JsonSerializer.Serialize(DatatableColumns);
            }
        }
        public string CreateUrl { get; set; } = "";
        public string DataUrl { get; set; }
        public bool ShowSearchSaveButton { get; set; } = false;
        public string SearchViewPath { get; set; }
        public string TableViewPath { get; set; }
        public bool LoadDatatableScript { get; set; } = true;
        public bool LoadDefaultDatatableScript { get; set; } = true;
        public string DataTableHeaderHtml { get; set; }
        public bool HideCreateButton { get; set; } = false;
        public bool HideSearchFiltersButton { get; set; } = false;
        public string CreateButtonAction { get; set; } = "Create";
        public string CreateButtonTitle { get; set; } = "";
        public bool DisableSearch { get; set; } = true;
        public bool HideTopSearchBar { get; set; } = false;

        public bool EnableHiddenValueSearch { get; set; } = false;
        public string ActionName { get; set; }
        public string TitleHtml { get; set; }
        public string SearchBarHtml { get; set; }
        public string ControllerName { get; set; }
        public bool IsPasscodeRequiredForDelete { get; set; } = false;
        public bool IsLayoutNull { get; set; } = false;

        public string RowClickDefaultAction { get; set; } = "Detail";

        public bool ShowDatatableButtons { get; set; } = false;
        public bool MakeColumnsClickable { get; set; }
    }

    public class ReportCrudListViewModel : CrudListViewModel
    {
        public List<DataTableActionViewModel> ActionsList { get; set; } = new();
    }
}

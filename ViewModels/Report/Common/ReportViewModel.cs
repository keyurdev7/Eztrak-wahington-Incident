using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ViewModels.DataTable;

namespace ViewModels.Report.Common
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            SearchViewPath = "_Search";
        }
        public string Title { get; set; }
        public bool ShowPageTitle { get; set; }
        public List<DataTableViewModel> DatatableColumns { get; set; }
        public IBaseSearchModel Filters { get; set; }
        public string DatatableColumnsJson
        {
            get
            {
                return JsonSerializer.Serialize(DatatableColumns);
            }
        }
        public string DataUrl { get; set; }
        public string SearchViewPath { get; set; }
        public string DataTableHeaderHtml { get; set; }
        public bool HideSearchFiltersButton { get; set; } = false;
        public bool DisableSearch { get; set; } = true;
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
}

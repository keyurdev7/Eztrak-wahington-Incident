using Pagination;
using ViewModels.Charts.Interfaces;
using ViewModels.DataTable;

namespace ViewModels.Charts.Shared
{
    public class ChartViewModel
    {
        public ChartViewModel()
        {
            SearchViewPath = "_Search";
        }
        public string Id { get; set; }
        public bool ShowTitle { get; set; } = true;
        public string Title { get; set; }
        public IChartBaseSearchModel Filters { get; set; }
        public string DataUrl { get; set; }
        public string ChartGenerationFunction { get; set; }
        public string SearchViewPath { get; set; }
        public bool DisableSearch { get; set; } = false;
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string CssClass { get; set; } = "am-chart";
        public bool SetLayout { get; set; } = false;
    }
}

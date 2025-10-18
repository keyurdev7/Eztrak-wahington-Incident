using ViewModels.Charts.Shared;
using ViewModels.Dashboard.interfaces;
using ViewModels.Report.PendingOrder;

namespace ViewModels.Dashboard
{
    public class DashboardFactory : IDashboardFactory
    {
        public ChartViewModel CreatePendingOrderChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "pending-order-chart",
                Title = "Pending Order",
                DataUrl = "/Home/GetPendingOrderChartData",
                SearchViewPath = "~/Views/Dashboard/PendingOrder/_Search.cshtml",
                ChartGenerationFunction = "GenerateAmPieChart",
                Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateWorkOrderChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "work-order-chart",
                Title = "Work Order",
                DataUrl = "/Home/GetWorkOrderChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateWorkOrderByRepairTypeChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "work-order-repair-type-chart",
                Title = "Work Order By Repair Type",
                DataUrl = "/Home/GetWorkOrderByRepairTypeChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new WorkOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateWorkOrderByAssetTypeChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "work-order-asset-type-chart",
                Title = "Work Order By Asset Type",
                DataUrl = "/Home/GetWorkOrderByAssetTypeChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new WorkOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateWorkOrderByManagerChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "work-order-by-manager-chart",
                Title = "Work Order By Manager",
                DataUrl = "/Home/GetWorkOrderByManagerChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateWorkOrderByTechnicianChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "work-order-by-technician-chart",
                Title = "Work Order By Technician",
                DataUrl = "/Home/GetWorkOrderByTechnicianChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateAssetByConditionChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "asset-by-condition-chart",
                Title = "Asset By Condition",
                DataUrl = "/Home/GetAssetsByConditionChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateAssetMaintenanceDueChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "asset-by-maintenance-chart",
                Title = "Asset By Maintenance",
                DataUrl = "/Home/GetAssetsMaintenanceDueChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateAssetReplacementDueChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "asset-by-replacement-chart",
                Title = "Asset By Replacement",
                DataUrl = "/Home/GetAssetsReplacementDueChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new PendingOrderChartSearchViewModel()
            };
        }
        public ChartViewModel CreateCostAccuracyChartViewModel()
        {
            return new ChartViewModel
            {
                Id = "cost-accuracy-chart",
                Title = "Cost Accuracy",
                DataUrl = "/Home/GetCostAccuracyChartData",
                DisableSearch = true,
                SearchViewPath = "",
                ChartGenerationFunction = "GenerateAmPieChart",
                //Filters = new PendingOrderChartSearchViewModel()
            };
        }
    }
}

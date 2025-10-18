using Models;
using ViewModels.CRUD;
using ViewModels.DataTable;
using ViewModels.Report.Common;
using ViewModels.Report.Factory.interfaces;
using ViewModels.Report.PendingOrder;
using ViewModels.Report.RawReport;
using ViewModels.Timesheet;

namespace ViewModels.Dashboard
{
    public class ReportFactory : IReportFactory
    {
        public ReportViewModel CreatePendingOrderReportViewModel()
        {
            var vm = new ReportViewModel();
            vm.Title = "Pending Orders";
            vm.Filters = new PendingOrderReportSearchViewModel();
            vm.DatatableColumns = GetPendingOrderReportColumns();
            vm.DisableSearch = false;
            vm.ControllerName = "Report";
            vm.DataUrl = $"/Report/GetPendingOrderReportData";
            vm.SearchViewPath = $"~/Views/Report/PendingOrder/_Search.cshtml";
            return vm;
        }
        public List<DataTableViewModel> GetPendingOrderReportColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Period",data = "Label"},
                new DataTableViewModel{title = "Client",data = "Client.Name"},
                new DataTableViewModel{title = "Representative",data = "AssignedTo.Name"},
                new DataTableViewModel{title = "Requests",data = "Requests"},
            };
        }

        public ReportCrudListViewModel CreateWorkOrderReportViewModel()
        {
            var filters = new WorkOrderSearchViewModel();
            var html = @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                        <span class=""custom-badge Complete m-1""> </span>
                        <span class=""stat-name"">Complete </span>
                    </div>
                    <div class=""m-2 d-flex"">
                        <span class=""custom-badge Working m-1""> </span>
                        <span class=""stat-name"">Working</span>
                    </div>
                   <div class=""m-2 d-flex"">
                        <span class=""custom-badge Open m-1""> </span>
                        <span class=""stat-name"">Open</span>
                    </div>
                    </div>";

            var titleHtml = @"<div class=""d-flex justify-content-start"">
                                <h3 class=""page-title text-site-primary m-md-0"">Work Order Report</h3>
                            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Work Order Report",
                Filters = filters,
                DatatableColumns = GetWorkOrderColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/WorkOrder/_Search.cshtml",
                DataUrl = "/Report/WorkOrderSearch",
                SearchBarHtml = html,
                TitleHtml = titleHtml,
                ShowDatatableButtons = true,
            };
        }
        public List<DataTableViewModel> GetWorkOrderColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Status",data = "Status",format="html",formatValue="status" , orderable=true},
                new DataTableViewModel{title = "ID #",data = "SystemGeneratedId", orderable=true},
                new DataTableViewModel{title = "Asset Id",data = "Asset.SystemGeneratedId", orderable=true},
                new DataTableViewModel{title = "Asset Type",data = "Asset.AssetType", orderable=true},
                new DataTableViewModel{title = "Description",data = "Asset.Description", orderable=true},
                new DataTableViewModel{title = "Street",data = "Asset.Street", orderable = true},
                new DataTableViewModel{title = "Task",data = "Task", orderable=true},
                new DataTableViewModel{title = "Type",data = "Type", orderable=true},
                new DataTableViewModel{title = "Urgency",data = "Urgency", orderable = true},
                new DataTableViewModel{title = "Manager",data = "Manager.Name", orderable = true},
                new DataTableViewModel{title = "Hours",data = "TaskType.Hours"},
                new DataTableViewModel{title = "Labor",data = "TaskType.Labor",className="dt-currency"},
                new DataTableViewModel{title = "Material",data = "TaskType.Material",className="dt-currency"},
                new DataTableViewModel{title = "Equipment",data = "TaskType.Equipment",className="dt-currency"},
                new DataTableViewModel{title = "Actual Average",data = "TaskType.Budget",className="dt-currency"},
            };
        }

        public ReportCrudListViewModel CreateMaintenanceReportViewModel()
        {
            var filters = new AssetSearchViewModel();
            var html = @"
                        <div class=""col d-flex justify-content-end"">
                            <div class=""p-2 d-flex"">
                                <span class=""custom-badge Good m-1""> </span>
                                <span class=""stat-name"">Good </span>
                            </div>
                            <div class=""m-2 d-flex"">
                                <span class=""custom-badge Fair m-1""> </span>
                                <span class=""stat-name"">Fair</span>
                            </div>
                            <div class=""m-2 d-flex"">
                                <span class=""custom-badge Poor m-1""> </span>
                                <span class=""stat-name"">Poor</span>
                            </div>
                            <div class=""m-2 d-flex"">
                                <span class=""custom-badge OutOfService m-1""> </span>
                                <span class=""stat-name"">Out Of Service</span>
                            </div>
                        </div>
                       ";
            html = "";

            var titleHtml = @"
                            <div class=""d-flex justify-content-start"">
                                <h3 class=""page-title text-site-primary m-md-0"">Maintenance Report</h3>
                            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Maintenance Report",
                Filters = filters,
                DatatableColumns = GetMaintenanceReportColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/Maintenance/_Search.cshtml",
                DataUrl = "/Report/MaintenanceSearch",
                SearchBarHtml = html,
                TitleHtml = titleHtml,
                ShowDatatableButtons = true,
                IsLayoutNull = true,
                HideTopSearchBar = true,
                HideTitle = true,
                ActionsList = new List<DataTableActionViewModel>()
                {
                    new DataTableActionViewModel() {Action="CreateWorkOrder",Title="Add",HideBasedOn="HideCreate",Href=$"/WorkOrder/CreateWorkOrder/{{Id}}"},
                }
            };
        }
        public List<DataTableViewModel> GetMaintenanceReportColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Condition",data = "Condition.Name",format="html",formatValue="status", orderable = true,filterId="condition-search-container",hasFilter=true},
                new DataTableViewModel{title = "Asset Type",data = "AssetType.Name", orderable = true,filterId="asset-type-search-container",hasFilter=true},
                new DataTableViewModel{title = "ID #",data = "SystemGeneratedId", orderable = true},
                //new DataTableViewModel{title = "Pole ID",data = "PoleId"},
                new DataTableViewModel{title = "Description",format="html",formatValue="tooltip",data = "Description", orderable = true },
                new DataTableViewModel{title = "MUTCD",data = "MUTCD.Name" , orderable = true, sortingColumn = "MUTCD.Code",filterId="mutcd-search-container",hasFilter=true},
                //new DataTableViewModel{title = "Class",data = "AssetClass"},
                new DataTableViewModel{title = "Maintenance Date",data = "FormattedNextMaintenanceDate", orderable = true, sortingColumn = "NextMaintenanceDate" },
                new DataTableViewModel{title = "Months Remaining",data = "FormattedMonthsRemaining" },
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }

        public ReportCrudListViewModel CreateMaterialCostReportViewModel()
        {
            var filters = new InventorySearchViewModel();
            var html = @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                        <span class=""custom-badge Complete m-1""> </span>
                        <span class=""stat-name"">Complete </span>
                    </div>
                    <div class=""m-2 d-flex"">
                        <span class=""custom-badge Working m-1""> </span>
                        <span class=""stat-name"">Working</span>
                    </div>
                   <div class=""m-2 d-flex"">
                        <span class=""custom-badge Open m-1""> </span>
                        <span class=""stat-name"">Open</span>
                    </div>
                    </div>
                   ";

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Material Cost Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Material Cost Report",
                Filters = filters,
                DatatableColumns = GetMaterialCostColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/MaterialCost/_Search.cshtml",
                DataUrl = "/Report/MaterialCostSearch",
                SearchBarHtml = html,
                TitleHtml = titleHtml,
                ShowDatatableButtons = true,
            };
        }
        public List<DataTableViewModel> GetMaterialCostColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "ID#",data = "ItemNo", orderable = true},
                new DataTableViewModel{title = "Category",data = "Category.Name", orderable = true},
                new DataTableViewModel{title = "Description",data = "Description", orderable = true},
                new DataTableViewModel{title = "MUTCD",data = "MUTCD.Name", orderable = true, sortingColumn = "MUTCD.Code",filterId="mutcd-search-container",hasFilter=true},
                new DataTableViewModel{title = "Manufacturer",data = "Manufacturer.Name", orderable = true,filterId="manufacturer-search-container",hasFilter=true},
                new DataTableViewModel{title = "OH Qty",data = "Quantity", orderable = true},
                new DataTableViewModel{title = "UOM",data = "UOM.Name", orderable = true},
                new DataTableViewModel{title = "Price",data = "ItemPrice", className = "dt-currency" ,orderable = true},
                new DataTableViewModel{title = "Total Value",data = "TotalValue", className = "dt-currency" ,orderable = true},
            };
        }

        public ReportCrudListViewModel CreateReplacementReportViewModel()
        {
            var filters = new AssetSearchViewModel();
            var html = @"
                    <div class=""col d-flex justify-content-end"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Good m-1""> </span>
                            <span class=""stat-name"">Good </span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Fair m-1""> </span>
                            <span class=""stat-name"">Fair</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Poor m-1""> </span>
                            <span class=""stat-name"">Poor</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge OutOfService m-1""> </span>
                            <span class=""stat-name"">Out Of Service</span>
                        </div>
                    </div>
                   ";

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Replacement Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Replacement Report",
                Filters = filters,
                DatatableColumns = GetReplacementReportColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/Replacement/_Search.cshtml",
                DataUrl = "/Report/ReplacementSearch",
                SearchBarHtml = html,
                TitleHtml = titleHtml,
                ShowDatatableButtons = true,
                IsLayoutNull = true,
                HideTopSearchBar = true,
                HideTitle = true,
                ActionsList = new List<DataTableActionViewModel>()
                {
                    new DataTableActionViewModel() {Action="CreateWorkOrder",Title="Add",HideBasedOn="HideCreate",Href=$"/WorkOrder/CreateWorkOrder/{{Id}}"},
                }
            };
        }
        public List<DataTableViewModel> GetReplacementReportColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Condition",data = "Condition.Name",format="html",formatValue="status", orderable = true,filterId="condition-search-container",hasFilter=true},
                new DataTableViewModel{title = "Asset Type",data = "AssetType.Name", orderable = true,filterId="asset-type-search-container",hasFilter=true},
                new DataTableViewModel{title = "ID #",data = "SystemGeneratedId", orderable = true},
                //new DataTableViewModel{title = "Pole ID",data = "PoleId"},
                new DataTableViewModel{title = "Description",format="html",formatValue="tooltip",data = "Description" , orderable = true},
                new DataTableViewModel{title = "MUTCD",data = "MUTCD.Name" , orderable = true, sortingColumn = "MUTCD.Code",filterId="mutcd-search-container",hasFilter=true},
                //new DataTableViewModel{title = "Class",data = "AssetClass"},
                new DataTableViewModel{title = "Replacement Date",data = "FormattedReplacementDate" , orderable = true , sortingColumn = "ReplacementDate"},
                new DataTableViewModel{title = "Months Remaining",data = "FormattedMonthsRemaining" },
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
        }

        public ReportCrudListViewModel CreateEquipmentCostReportViewModel()
        {
            var filters = new EquipmentSearchViewModel();
            var html = @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                            <span class=""custom-badge Complete m-1""> </span>
                            <span class=""stat-name"">Complete </span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge Working m-1""> </span>
                            <span class=""stat-name"">Working</span>
                        </div>
                       <div class=""m-2 d-flex"">
                            <span class=""custom-badge Open m-1""> </span>
                            <span class=""stat-name"">Open</span>
                        </div>
                    </div>
                   ";

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Equipment Cost Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Equipment Cost Report",
                Filters = filters,
                DatatableColumns = GetEquipmentCostColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/EquipmentCost/_Search.cshtml",
                DataUrl = "/Report/EquipmentCostSearch",
                SearchBarHtml = html,
                TitleHtml = titleHtml,
                ShowDatatableButtons = true,
            };
        }
        public List<DataTableViewModel> GetEquipmentCostColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "ID#",data = "ItemNo", orderable = true},
                new DataTableViewModel{title = "Category",data = "Category.Name", orderable = true},
                new DataTableViewModel{title = "Description",data = "Description", orderable = true},
                new DataTableViewModel{title = "Manufacturer",data = "Manufacturer.Name", orderable = true,filterId="manufacturer-search-container",hasFilter=true},
                new DataTableViewModel{title = "Hourly Rate",data = "HourlyRate", className = "dt-currency", orderable = true},
                new DataTableViewModel{title = "Quantity",data = "Quantity", orderable = true},
                new DataTableViewModel{title = "UOM",data = "UOM.Name", orderable = true},
                new DataTableViewModel{title = "Cost",data = "ItemPrice", className = "dt-currency" ,orderable = true},
                new DataTableViewModel{title = "Total Value",data = "TotalValue", className = "dt-currency" ,orderable = true},
            };
        }

        public ReportCrudListViewModel CreateTransactionReportViewModel()
        {
            var filters = new TransactionSearchViewModel();

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Transaction Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Transaction Report",
                Filters = filters,
                DatatableColumns = GetTransactionColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/Transaction/_Search.cshtml",
                DataUrl = "/Report/TransactionSearch",
                SearchBarHtml = "",
                TitleHtml = titleHtml,
                IsLayoutNull = true,
                HideTopSearchBar = true,
                HideTitle = true,
                ShowDatatableButtons = true,
            };
        }
        public List<DataTableViewModel> GetTransactionColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "PO #",data = "PONo"},
                new DataTableViewModel{title = "Inventory",data = "Type"},
                new DataTableViewModel{title = "Type",data = "TransactionType"},
                new DataTableViewModel{title = "Work Order #",data = "WorkOrderNumber"},
                new DataTableViewModel{title = "Funding Source",data = "FundingSource"},
                new DataTableViewModel{title = "Location",data = "Location",filterId="location-search-container",hasFilter=true},
                new DataTableViewModel{title = "Manufacturer",data = "Manufacturer"},
                new DataTableViewModel{title = "MUTCD",data = "MUTCD"},
                new DataTableViewModel{title = "Rate",data = "Rate", className="dt-currency"},
                new DataTableViewModel{title = "Supplier",data = "Supplier",filterId="supplier-search-container",hasFilter=true},
                new DataTableViewModel{title = "UOM",data = "UOM",filterId="uom-search-container",hasFilter=true},
                new DataTableViewModel{title = "Qty",data = "Quantity"},
                new DataTableViewModel{title = "User",data = "CreatedBy"},
                new DataTableViewModel{title = "Added",data = "FormattedCreatedOn"},
            };
        }

        public ReportCrudListViewModel CreateTimesheetReportViewModel()
        {
            var filters = new TimeSheetBreakdownReportSearchViewModel();

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Timesheet Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Timesheet Report",
                Filters = filters,
                DatatableColumns = GetTimesheetColumns(),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/Timesheet/_Search.cshtml",
                DataUrl = "/Report/TimesheetSearch",
                SearchBarHtml = "",
                TitleHtml = "",
                IsLayoutNull = true,
                HideTopSearchBar = true,
                HideTitle = true,
                ShowDatatableButtons = true,
            };
        }
        public List<DataTableViewModel> GetTimesheetColumns()
        {
            return new List<DataTableViewModel>()
            {
                //new DataTableViewModel{title = "PO #",data = "PONo"},
               new DataTableViewModel{title = "Date",data = "FormattedDate", sortingColumn="Date",filterId="date-range-search-container",hasFilter=true, orderable = true},
                new DataTableViewModel{title = "Technician",data = "Timesheet.Technician.Name",filterId="technician-search-container",hasFilter=true},
                //new DataTableViewModel{title = "Location Site",data = "LocationSite"},
                //new DataTableViewModel{title = "Invoice #",data = "Timesheet.InvoiceNo", sortingColumn="Timesheet.InvoiceNo", orderable = true},
                new DataTableViewModel{title = "Work Order #",data = "Timesheet.WorkOrder.SystemGeneratedId",filterId="work-order-search-container",hasFilter=true},
                new DataTableViewModel{title = "Approver",data = "Timesheet.Approver.Name"},
                //new DataTableViewModel{title = "Work Order Number",data = "Timesheet.Contract.JobNumber"},
                //new DataTableViewModel{title = "Department",data = "Timesheet.Contract.Department"},
                new DataTableViewModel{title = "Craft",data = "Timesheet.CraftSkill.Name",filterId="craft-search-container",hasFilter=true},
                //new DataTableViewModel{title = "TS Ref Status",data = "FormattedTSRefStatus", sortingColumn = "TSRefStatus", orderable=true},
                //new DataTableViewModel{title = "TS Ref Number",data = "TSRefNumber", orderable = true},
                //new DataTableViewModel{title = "Payment Indicator",data = "PaymentIndicator", orderable = true},
                new DataTableViewModel{title = "ST Hrs",data = "RegularHours", orderable = true},
                new DataTableViewModel{title = "OT Hrs",data = "OvertimeHours", orderable = true},
                new DataTableViewModel{title = "DT Hrs",data = "DoubleTimeHours", orderable = true},
                new DataTableViewModel{title = "ST Rate",data = "Timesheet.CraftSkill.STRate",className="dt-currency"},
                new DataTableViewModel{title = "OT Rate",data = "Timesheet.CraftSkill.OTRate",className="dt-currency"},
                new DataTableViewModel{title = "DT Rate",data = "Timesheet.CraftSkill.DTRate",className="dt-currency"},
                //new DataTableViewModel{title = "Per Diem",data = "Timesheet.PerDiem",className="dt-currency", orderable = true},
                //new DataTableViewModel{title = "Equipment",data = "Timesheet.Equipment",className="dt-currency", orderable = true},
                //new DataTableViewModel{title = "Other",data = "Timesheet.Other",className="dt-currency", orderable = true},
                new DataTableViewModel{title = "Total Cost",data = "TotalFormatted",className="dt-currency"},
                //new DataTableViewModel{title = "Total Received Cost",data = "TotalReceivedCostFormatted"},
                //new DataTableViewModel{title = "Balance Due",data = "BalanceDueFormatted"},
            };
        }

        public ReportCrudListViewModel CreateWorkOrderRawReportViewModel(WorkOrderRawReportColumns additionalColumns)
        {
            var filters = new WorkOrderSearchViewModel();

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Work Order Raw Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Work Order Raw Report",
                Filters = filters,
                DatatableColumns = GetWorkOrderRawColumns(additionalColumns),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/WorkOrderRaw/_Search.cshtml",
                DataUrl = "/Report/WorkOrderRawSearch",
                SearchBarHtml = "",
                TitleHtml = titleHtml,
                ShowDatatableButtons = true

            };

        }
        public List<DataTableViewModel> GetWorkOrderRawColumns(WorkOrderRawReportColumns additionalColumns)
        {

            var columns = new List<DataTableViewModel>()
            {
               new DataTableViewModel{title = "Source",data = "FormattedSourceStatus", sortingColumn="SourceStatus", orderable = true},
               new DataTableViewModel{title = "Status",data = "FormattedStatus", sortingColumn="Status", orderable = true},
               new DataTableViewModel{title = "Date Created",data = "FormattedCreatedOn", sortingColumn="Status", orderable = true},
               new DataTableViewModel{title = "Requestor Name",data = "RequestorName", orderable = true},
               new DataTableViewModel{title = "Phone",data = "Phone", orderable = true},
               new DataTableViewModel{title = "Address",data = "Address", orderable = true},
               new DataTableViewModel{title = "City",data = "City", orderable = true},
               new DataTableViewModel{title = "State",data = "State", orderable = true},
               new DataTableViewModel{title = "Zip Code",data = "ZipCode", orderable = true},
               new DataTableViewModel{title = "Email",data = "Email", orderable = true},
               new DataTableViewModel{title = "Email Subject",data = "EmailSubject", orderable = true},
               new DataTableViewModel{title = "Type of Problem",data = "TypeOfProblem", orderable = true},
               new DataTableViewModel{title = "Street",data = "Intersection", orderable = true},
               new DataTableViewModel{title = "Description",data = "Description", orderable = true},
               new DataTableViewModel{title = "Work Order #",data = "SystemGeneratedId", orderable = true},
               new DataTableViewModel{title = "Manager",data = "Manager", orderable = true},
               new DataTableViewModel{title = "Urgency",data = "FormattedUrgency", orderable = true, sortingColumn = "Urgency"},
               new DataTableViewModel{title = "Type",data = "FormattedType", orderable = true, sortingColumn = "Type"},
               new DataTableViewModel{title = "Due Date",data = "FormattedDueDate", orderable = true, sortingColumn = "DueDate"},
               new DataTableViewModel{title = "Task",data = "FormattedTask", orderable = true, sortingColumn = "Task"},
               new DataTableViewModel{title = "Asset Id",data = "AssetId", orderable = true},
               new DataTableViewModel{title = "Asset Type",data = "AssetType", orderable = true},
               new DataTableViewModel{title = "Approval Date",data = "FormattedApprovalDate", orderable = true, sortingColumn = "ApprovalDate"},
               new DataTableViewModel{title = "Total Hours",data = "TotalHours", orderable = true},
               new DataTableViewModel{title = "Labor $",data = "LabourCost", orderable = true, className = "dt-currency"},
               new DataTableViewModel{title = "Material $",data = "MaterialCost", orderable = true, className = "dt-currency"},
               new DataTableViewModel{title = "Equipment $",data = "EquipmentCost", orderable = true, className = "dt-currency"},
            };

            if (additionalColumns.MaterialColumns != null && additionalColumns.MaterialColumns.Count > 0)
            {
                foreach (var mc in additionalColumns.MaterialColumns)
                {
                    columns.Add(new DataTableViewModel { title = $"{mc.Name} $", data = $"Materials[{mc.Id}].Cost", format = "dictionary", className = "mw-100 dt-currency" });
                }
            }

            if (additionalColumns.EquipmentColumns != null && additionalColumns.EquipmentColumns.Count > 0)
            {
                foreach (var eq in additionalColumns.EquipmentColumns)
                {
                    columns.Add(new DataTableViewModel { title = $"{eq.Name} $", data = $"Equipments[{eq.Id}].Cost", format = "dictionary", className = "mw-100 dt-currency" });
                }
            }

            if (additionalColumns.TechnicianColumns != null && additionalColumns.TechnicianColumns.Count > 0)
            {
                foreach (var t in additionalColumns.TechnicianColumns)
                {
                    columns.Add(new DataTableViewModel { title = $"{t.Name} $", data = $"Technicians[{t.Id}].Cost", format = "dictionary", className = "mw-80 dt-currency" });
                    columns.Add(new DataTableViewModel { title = $"Craft Skill $", data = $"Technicians[{t.Id}].Craft", format = "dictionary", className = "mw-80" });
                }
            }

            return columns;
        }

        public ReportCrudListViewModel CreateAssetRawReportViewModel(List<string> additionalColumns)
        {
            var filters = new AssetSearchViewModel();

            var titleHtml = @"
            <div class=""d-flex justify-content-start"">
                <h3 class=""page-title text-site-primary m-md-0"">Asset Raw Report</h3>
            </div>";

            return new ReportCrudListViewModel
            {
                Title = "Asset Raw Report",
                Filters = filters,
                DatatableColumns = GetAssetRawColumns(additionalColumns),
                DisableSearch = false,
                HideCreateButton = true,
                ShowSearchSaveButton = true,
                ControllerName = "Report",
                SearchViewPath = "~/Views/Report/AssetRaw/_Search.cshtml",
                DataUrl = "/Report/AssetRawSearch",
                SearchBarHtml = "",
                TitleHtml = titleHtml,
                ShowDatatableButtons = true
            };
        }
        public List<DataTableViewModel> GetAssetRawColumns(List<string> additionalColumns)
        {
            var columns = new List<DataTableViewModel>()
            {
               new DataTableViewModel{title = "Id",data = "SystemGeneratedId", orderable = true},
               new DataTableViewModel{title = "Asset Type",data = "AssetTypeName", orderable = true},
               new DataTableViewModel{title = "Description",data = "Description", orderable = true},
               new DataTableViewModel{title = "Value",data = "Value", orderable = true, className="dt-currency"},
               new DataTableViewModel{title = "MUTCD",data = "MUTCDCode", orderable = true}
            };

            foreach (var c in additionalColumns)
            {
                columns.Add(new DataTableViewModel { title = $"{c} $", data = $"Associations['{c}']", format = "dictionary", className = "mw-100" });
            }
            //new DataTableViewModel { title = "Sign Type", data = "SignType", orderable = true },
            //   new DataTableViewModel { title = "Post Type", data = "PostType", orderable = true },
            //   new DataTableViewModel { title = "Post Location Type", data = "PostLocationType", orderable = true },
            //   new DataTableViewModel { title = "Mount Type", data = "MountType", orderable = true },
            //   new DataTableViewModel { title = "Hardware", data = "Hardware", orderable = true },
            //   new DataTableViewModel { title = "Dimension", data = "Dimension", orderable = true },



            columns.AddRange(new List<DataTableViewModel> {
               new DataTableViewModel{title = "Date Added",data = "FormattedDateAdded", orderable = true, sortingColumn = "DateAdded"},
               new DataTableViewModel{title = "Last Maint. Date",data = "FormattedLastMaintenanceDate"},
               new DataTableViewModel{title = "Maint. Date",data = "FormattedNextMaintenanceDate", orderable = true, sortingColumn = "NextMaintenanceDate"},
               new DataTableViewModel{title = "Last Replacement Date",data = "FormattedLastReplacementDate"},
               new DataTableViewModel{title = "Replacement Date",data = "FormattedReplacementDate", orderable = true, sortingColumn = "ReplacementDate"},
               new DataTableViewModel{title = "Last Removal Date",data = "FormattedLastRemovalDate"},
               new DataTableViewModel{title = "Last Repair Date",data = "FormattedLastRepairDate"}
            });

            return columns;
        }

    }
}

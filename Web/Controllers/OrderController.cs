using System.Net;
using AutoMapper;
using Centangle.Common.ResponseHelpers.Models;
using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Common;
using ViewModels;
using ViewModels.CRUD;
using ViewModels.DataTable;
using Web.Controllers.Shared;

namespace Web.Controllers
{
    public class OrderController : CrudBaseController<OrderModifyViewModel, OrderModifyViewModel, OrderDetailViewModel, OrderDetailViewModel, OrderBriefViewModel, OrderSearchViewModel>
    {
        private readonly IOrderService<OrderModifyViewModel, OrderModifyViewModel, OrderDetailViewModel> _service;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService<OrderModifyViewModel, OrderModifyViewModel, OrderDetailViewModel> service, ILogger<OrderController> logger, IMapper mapper) : base(service, logger, mapper, "Order", "Reservation", false)
        {
            _service = service;
            _logger = logger;
        }

        public override List<DataTableViewModel> GetColumns()
        {
            return new List<DataTableViewModel>()
            {
                new DataTableViewModel{title = "Status",data = "Status",format="html",formatValue="status", orderable = true },
                new DataTableViewModel{title = "Work Order Number",data = "WorkOrder.SystemGeneratedId"},
                new DataTableViewModel{title = "Date",data = "FormattedCreatedOn", orderable = true, sortingColumn = "CreatedOn"},
                //new DataTableViewModel{title = "Urgency",data = "WorkOrder.FormattedUrgency"},
                new DataTableViewModel{title = "Total Cost",data = "TotalCost",className="dt-currency" , orderable = true},
                new DataTableViewModel{title = "Description",data = "Notes", format="html", formatValue = "tooltip", orderable = true},
                //new DataTableViewModel{title = "Location",data = "WorkOrder.Street"},
                new DataTableViewModel{title = "Requestor",data = "Requestor.Name", orderable = true, sortingColumn = "Requestor.FirstName"},
                //new DataTableViewModel{title = "Type",data = "Type"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}
            };
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                    //new DataTableActionViewModel() {Action="Detail",Title="Detail",Href=$"/{_controllerName}/Detail/{{Id}}"},
                    new DataTableActionViewModel() {Action="Issue",Title="Issue",Href=$"/Order/Detail/{{Id}}", Class="issue-order", Attr=new List<string>{"Id" } },
                    new DataTableActionViewModel() {Action="Delete",Title="Delete",Href=$"/Order/Delete/{{Id}}",DatatableHrefType=DatatableHrefType.Link},
            };
        }


        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var viewModel = await base.OverrideCrudListVM(vm);
            var html = @"
                    <div class=""col d-flex justify-content-end"" style=""margin-top: -5px;"">
                        <div class=""p-2 d-flex"">
                        <span class=""custom-badge Complete m-1""> </span>
                        <span class=""stat-name"">Complete </span>
                    </div>
                    <div class=""m-2 d-flex"">
                        <span class=""custom-badge Issued m-1""> </span>
                        <span class=""stat-name"">Partially Issued</span>
                    </div>
                   <div class=""m-2 d-flex"">
                        <span class=""custom-badge Open m-1""> </span>
                        <span class=""stat-name"">Open</span>
                    </div>
                    </div>
                   ";
            vm.SearchBarHtml = html;
            viewModel.IsLayoutNull = true;
            viewModel.HideTopSearchBar = true;
            viewModel.HideTitle = true;
            viewModel.LoadDatatableScript = false;
            viewModel.HideCreateButton = true;
            return viewModel;
        }
        [HttpPost]
        public async override Task<ActionResult> Create(OrderModifyViewModel model)
        {
            try
            {
                if (model.WorkOrder.Id > 0 && model.OrderItems.Count > 0 && model.OrderItems.All(x => x.Quantity > 0))
                {
                    var response = await _service.Create(model);
                    long id = 0;
                    if (response.Status == HttpStatusCode.OK)
                    {
                        var parsedResponse = response as RepositoryResponseWithModel<long>;
                        id = parsedResponse?.ReturnModel ?? 0;
                    }
                    if (model.Type == OrderTypeCatalog.Inventory) HttpContext.Session.Remove("Cart");
                    else HttpContext.Session.Remove("EquipmentCart");
                    return Json(new { Status = true });
                }
                else
                {
                    return Json(new { Status = false, Message = "To create an order, you must select at least 1 Inventory or Equipment!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order Create method threw an exception, Message: {ex.Message}");
            }
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> IssueInventoryItem(long orderItemId, long inventoryId)
        {
            try
            {
                var ordersResponse = await _service.GetInventoryToIssue(orderItemId, inventoryId);
                var orders = (ordersResponse as RepositoryResponseWithModel<IssueInventoryItemViewModel>)?.ReturnModel ?? new();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order Create method threw an exception, Message: {ex.Message}");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IssueInventoryItem(IssueInventoryItemViewModel model)
        {
            var response = await _service.IssueInventoryItem(model);
            if (response.Status == HttpStatusCode.OK)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> IssueEquipmentItem(long orderItemId, long equipmentId)
        {
            try
            {
                var ordersResponse = await _service.GetEquipmentToIssue(orderItemId, equipmentId);
                var orders = (ordersResponse as RepositoryResponseWithModel<IssueEquipmentItemViewModel>)?.ReturnModel ?? new();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order Create method threw an exception, Message: {ex.Message}");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IssueEquipmentItem(IssueEquipmentItemViewModel model)
        {
            var response = await _service.IssueEquipmentItem(model);
            if (response.Status == HttpStatusCode.OK)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(long id, OrderTypeCatalog type)
        {
            var response = await _service.Submit(id, type);
            if (response.Status == HttpStatusCode.OK)
            {
                return Json(true);
            }
            return Json(false);
        }
    }
}


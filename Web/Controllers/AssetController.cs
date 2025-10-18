using AutoMapper;
using Repositories.Common;
using ViewModels.DataTable;
using ViewModels;
using Web.Controllers.Shared;
using Centangle.Common.ResponseHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories.Services.AttachmentService.Interface;
using ViewModels.CRUD;
using ViewModels.Asset;
using ViewModels.Shared.Notes;
using ViewModels.Shared;
using Pagination;
using Enums;
using System.Net;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Web.Controllers
{
    public class AssetController : CrudBaseController<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel, AssetDetailViewModel, AssetBriefViewModel, AssetSearchViewModel>
    {
        private readonly IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> _service;
        private readonly IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeModifyViewModel> _assetTypeService;
        private readonly ILogger<AssetController> _logger;
        private readonly IMapper _mapper;
        private readonly IAttachment _attachment;
        private readonly IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnOptionDetailViewModel> _dynamicColumnService;
        private readonly IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> _conditionService;
        private readonly IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> _workOrderService;

        public AssetController(IAssetService<AssetModifyViewModel, AssetModifyViewModel, AssetDetailViewModel> service
            , IAssetTypeService<AssetTypeModifyViewModel, AssetTypeModifyViewModel, AssetTypeModifyViewModel> assetTypeService
            , ILogger<AssetController> logger
            , IMapper mapper
            , IAttachment attachment
            , IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnOptionDetailViewModel> dynamicColumnService
            , IConditionService<ConditionModifyViewModel, ConditionModifyViewModel, ConditionDetailViewModel> conditionService
            , IWorkOrderService<WorkOrderModifyViewModel, WorkOrderModifyViewModel, WorkOrderDetailViewModel> workOrderService

            ) : base(service, logger, mapper, "Asset", "Assets", true, false, false)
        {
            _service = service;
            _assetTypeService = assetTypeService;
            _logger = logger;
            _mapper = mapper;
            _attachment = attachment;
            _dynamicColumnService = dynamicColumnService;
            _conditionService = conditionService;
            _workOrderService = workOrderService;
        }
        public override async Task<ActionResult> Index()
        {
            var response = await _assetTypeService.GetAll<AssetTypeDetailViewModel>(new AssetTypeSearchViewModel() { DisablePagination = true });
            var assetTypesResponse = response as IRepositoryResponseWithModel<PaginatedResultModel<AssetTypeDetailViewModel>>;
            var assetTypes = assetTypesResponse?.ReturnModel.Items ?? new List<AssetTypeDetailViewModel>();
            //var assetTree = await _assetTypeService.GetHirearchy();
            ViewBag.AssetTypes = assetTypes;
            //ViewBag.AssetTree = assetTree;
            return await base.Index();
        }


        public override List<DataTableViewModel> GetColumns()
        {
            var columns = new List<DataTableViewModel>()
            {
                //new DataTableViewModel{title = "", data = "Id",format="expand",className="action dt-control expand exclude-form-export"},
                new DataTableViewModel{title = "Condition",data = "Condition.Name", orderable = true,filterId="condition-search-container",hasFilter=true, format="html",formatValue="status", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "ConditionId","ConditionId","Condition.Id", "hidden-asset-create-form",false,"Id",new List<(string,string)>{("Select2SelectedId","Condition.Id"),("Select2SelectedValue","Condition.Name")})  },
                new DataTableViewModel{title = "ID #",data = "SystemGeneratedId", orderable = true },
                new DataTableViewModel{title = "Asset Type",data = "AssetType.Name",orderable = true,filterId="asset-type-search-container", hasFilter = true, isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "AssetTypeId","AssetTypeId","AssetType.Id", "hidden-asset-create-form",false,"Id",new List<(string,string)>{("Select2SelectedId","AssetType.Id"),("Select2SelectedValue", "AssetType.Name") })  },
                //new DataTableViewModel{title = "Asset ID",data = "SystemGeneratedId", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "AssetId","AssetId","AssetId", "hidden-asset-create-form",false) },
                //new DataTableViewModel{title = "Pole ID",data = "PoleId", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "PoleId","PoleId","PoleId", "hidden-asset-create-form",false) },
                new DataTableViewModel{title = "Description",format="html",orderable = true,formatValue="tooltip",data = "Description", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "Description","Description","Description", "hidden-asset-create-form", true) },
                //new DataTableViewModel{title = "MUTCD",data = "MUTCD.Name",orderable = true,filterId="mutcd-search-id",hasFilter=true, sortingColumn = "MUTCD.Code", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "MUTCDId","MUTCDId","MUTCD.Id", "hidden-asset-create-form",false,"Id",new List<(string,string)>{("Select2SelectedId","MUTCD.Id"),("Select2SelectedValue", "MUTCD.Name")}) },
                //new DataTableViewModel{title = "Class",data = "AssetClass", isEditable = true, editableColumnDetail = new EditableCellDetails("Asset", "AssetClass","AssetClass","AssetClass", "hidden-asset-create-form",false) },
                //new DataTableViewModel{title = "Manufacturer",data = "Manufacturer.Name"},
                //new DataTableViewModel{title = "Value",data = "Value",className="dt-currency"},
                //new DataTableViewModel{title = "Maint. $",data = "MaintenanceCost",className="dt-currency"},
                new DataTableViewModel{title = "Street",data = "Intersection" },
                //new DataTableViewModel{title = "Installed",data = "FormattedInstalledDate", orderable = true, sortingColumn = "InstalledDate" },
                //new DataTableViewModel{title = "Replace",data = "FormattedReplacementDate", orderable = true, sortingColumn = "ReplacementDate" },
                //new DataTableViewModel{title = "Next Maint.",data = "FormattedNextMaintenanceDate", orderable = true, sortingColumn = "NextMaintenanceDate" },
                new DataTableViewModel{title = "Images",data = "ImageUrls", format = "html", formatValue="multiple-images", className = "grid-images image-thumbnail action"},
                new DataTableViewModel{title = "Action",data = null,className="action text-right exclude-form-export"}

            };
            return columns;
        }

        protected override async Task<CrudListViewModel> OverrideCrudListVM(CrudListViewModel vm)
        {
            var response = await _assetTypeService.GetAll<AssetTypeDetailViewModel>(new AssetTypeSearchViewModel() { DisablePagination = true });
            var assetTypesResponse = response as IRepositoryResponseWithModel<PaginatedResultModel<AssetTypeDetailViewModel>>;
            var assetTypes = assetTypesResponse?.ReturnModel.Items ?? new List<AssetTypeDetailViewModel>();
            //var assetTree = await _assetTypeService.GetHirearchy();
            //var dataTableColumns = AppendDynamicColumns(assetTypes, assetTree, vm.DatatableColumns);
            ViewBag.AssetTypes = assetTypes;
            //ViewBag.AssetTree = assetTree;
            //ViewBag.AssetsDatatableColumns = dataTableColumns;
            var html = "";
            html += @"
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
                            <span class=""stat-name"">Needs Replacement</span>
                        </div>
                        <div class=""m-2 d-flex"">
                            <span class=""custom-badge OutOfService m-1""> </span>
                            <span class=""stat-name"">Out Of Service</span>
                        </div>
                    </div>
                   ";
            html = "";
            vm.TableViewPath = @"~/Views/Asset/_Table.cshtml";
            vm.SearchBarHtml = html;
            vm.IsLayoutNull = true;
            // vm.TitleHtml = @"
            // <div class=""d-flex justify-content-start align-items-center"">
            //     <h2 style=""margin: 0;"">Assets </h2>
            //     <a class=""btn orange ms-4 me-2"" style=""max-height:35px !important;min-height:35px !important;"" target=""_blank"" href=""/Map"">
            //        Map
            //     </a>
            // </div>";
            vm.HideTitle = true;
            vm.HideCreateButton = false;
            vm.HideTopSearchBar = true;
            vm.LoadDefaultDatatableScript = false;
            vm.LoadDatatableScript = false;
            vm.RowClickDefaultAction = "Update";
            return vm;
        }

        protected override void SetDatatableActions<T>(DatatablePaginatedResultModel<T> result)
        {
            result.ActionsList = new List<DataTableActionViewModel>()
            {
                new DataTableActionViewModel() {Tooltip="Create Work Order",Action="CreateWorkOrder",Title="Add",Href=$"/WorkOrder/CreateWorkOrder/{{Id}}"},
                //new DataTableActionViewModel() { Action = "History", Title = "History", Href = $"/Asset/AssetWorkHistory/{{Id}}"},
                new DataTableActionViewModel() {Tooltip="Work Order History", Action = "History", Title = "History", Href = $"/WorkOrders/Index/?activeTab=order-grid&assetId={{SystemGeneratedId}}",DatatableHrefType=DatatableHrefType.Link },
                //new DataTableActionViewModel() {Tooltip="Work Order Approval History", Action = "Approved History", Title = "History", Href = $"/Asset/_AssetApprovedWorkHistory/{{SystemGeneratedId}}"},
                //new DataTableActionViewModel() {Tooltip="Notes",Action="GetNotes",Title="Notes",Class="@HasNotesClass",Href=$"/Asset/GetNotes/{{Id}}"},
            };
        }

        public async override Task<ActionResult> Create()
        {

            var model = new AssetModifyViewModel();
            if (Request.Query.ContainsKey("lat"))
            {
                model.Latitude = double.Parse(Request.Query["lat"]);
            }

            if (Request.Query.ContainsKey("long"))
            {
                model.Longitude = double.Parse(Request.Query["long"]);
            }
            var dynamicColumns = await _dynamicColumnService.GetDynamicColumns(DynamicColumnEntityType.Asset, -1);
            model.DynamicColumns = dynamicColumns;
            var updateVm = GetUpdateViewModel("Create", model);
            updateVm = await OverrideUpdateViewModel(updateVm);
            return UpdateView(updateVm);
        }

        public override Task<ActionResult> Create(AssetModifyViewModel model)
        {
            var keysToRemove = ModelState.Keys.Where(key => key.StartsWith("AssetAssociation")).ToList();
            foreach (var key in keysToRemove)
            {
                ModelState.Remove(key);
            }
            return base.Create(model);
        }

        public async Task<ActionResult> GetNotes(long id)
        {
            try
            {
                var notes = await getAssetNotes(id);
                ViewBag.AssetId = id;
                return View("_Notes", notes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset Notes method threw an exception, Message: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> GetNotesListPartialView(long id)
        {
            try
            {
                var notes = await getAssetNotes(id);
                return View("~/Views/Shared/Notes/_NotesListPartialView.cshtml", notes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset Notes method threw an exception, Message: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> SaveNotes(AssetNotesViewModel model)
        {
            var response = await _service.SaveNotes(model);
            return Json(response.Status == System.Net.HttpStatusCode.OK);
        }

        public async Task<ActionResult> DeleteFile(long id)
        {
            bool flag = false;
            var message = "";
            var attachmentList = new List<long> { id };
            var response = await _attachment.Delete(attachmentList);
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                flag = true;
                message = "Image deleted successfully";
            }
            else { message = "Something went wrong. Please try again later."; }

            var data = new
            {
                success = flag,
                message = message
            };
            return Json(data);
        }

        public async Task<PartialViewResult> GetExpandedData(long id)
        {
            var items = await _service.GetAssetsSubLevels(new AssetSearchViewModel { Id = id });
            ViewBag.AssetId = id;
            return PartialView("_AssetSubLevel", items);
        }

        public async Task<IActionResult> Maps()
        {

            var response = await _service.GetAssetsForMap();
            if (response.Status == System.Net.HttpStatusCode.OK)
            {
                var parsedModel = response as RepositoryResponseWithModel<AssetsMapViewModel>;
                var responseModel = parsedModel?.ReturnModel;
                return View(responseModel);
            }
            return View(new AssetsMapViewModel());
        }

        public override async Task<ActionResult> Update(int id)
        {
            try
            {
                var response = await _service.GetById(id);
                AssetModifyViewModel model = null;
                if (response.Status == HttpStatusCode.OK)
                {
                    var parsedModel = response as RepositoryResponseWithModel<AssetDetailViewModel>;
                    model = _mapper.Map<AssetModifyViewModel>(parsedModel.ReturnModel);
                }
                if (model != null)
                {
                    var conditionResponse = await _conditionService.GetAll<ConditionDetailViewModel>(new ConditionSearchViewModel() { DisablePagination = true });
                    var conditions = (conditionResponse as RepositoryResponseWithModel<PaginatedResultModel<ConditionDetailViewModel>>).ReturnModel.Items;
                    var crudUpdateModel = GetUpdateViewModel("Update", model);
                    crudUpdateModel.UpdateViewPath = "_UpdateAssetTab";
                    ViewBag.AssetId = model.Id;
                    var notes = await getAssetNotes(model.Id);
                    var assetApprovalWorkHistory = await GetAssetApprovedWorkOrderData(model.Id);
                    var updateVM = new AssetTabsModifyViewModel()
                    {
                        CrudUpdateViewModel = crudUpdateModel,
                        Notes = notes,
                        ApprovalWorkOrderHistory = assetApprovalWorkHistory,
                        Conditions = conditions
                    };
                    return View("_Update", updateVM);
                }
                else
                {

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> GetAssetTypeHtml(long assetTypeId, long? assetId, bool showAll = false)
        {
            var assetTypeLevelsResponse = await _service.GetAssetTypeLevels(assetTypeId, assetId, showAll);
            var assetTypeLevels = (assetTypeLevelsResponse as RepositoryResponseWithModel<List<AssetTypeLevel1DetailViewModel>>).ReturnModel;
            return View("_AssetTypeLevelRadioButtons", assetTypeLevels);
        }

        public async Task<ActionResult> _GetAttachmentView(long Id)
        {
            try
            {
                var attachments = await _service.GetAssetAttachments(Id);
                return View("~/Views/Shared/GalleryPartialView/_GetGalleryView.cshtml", attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset _GetGalleryView method threw an exception, Message: {ex.Message}");
                return null;
            }

        }

        public async Task<string> _GetAttachmentUrl(long Id)
        {
            try
            {
                var attachmentUrl = await _service.GetAttachmentUrl(Id);
                return attachmentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Asset _GetAttachmentUrl method threw an exception, Message: {ex.Message}");
                return null;
            }
        }

        public override ActionResult DataTableIndexView(CrudListViewModel vm)
        {
            return View("~/Views/Asset/_Index.cshtml", vm);
        }

        public virtual async Task<bool> IsAssetIdUnique(long id, string assetId)
        {
            return await _service.IsAssetIdUnique(id, assetId);
        }

        private Dictionary<long, List<DataTableViewModel>> AppendDynamicColumns(List<AssetTypeDetailViewModel> assetTypes, List<AssetTypeTreeViewModel> assetTree, List<DataTableViewModel> defaultColumns)
        {
            Dictionary<long, List<DataTableViewModel>> assetColumnsDictionary = new Dictionary<long, List<DataTableViewModel>>();
            foreach (var assetType in assetTypes)
            {
                var assetColumns = defaultColumns.ToList(); // Make a shallow copy of defaultColumns
                //// Find the corresponding asset type in assetTree
                //var filteredAssetType = assetTree.FirstOrDefault(item => item.Id == assetType.Id);

                //// Check if the asset type is found in assetTree
                //if (filteredAssetType != null)
                //{
                //    if (filteredAssetType.AssetTypeLevel1 != null && filteredAssetType.AssetTypeLevel1.Any())
                //    {
                //        int index = assetColumns.Count > 1 ? assetColumns.Count - 2 : 0;
                //        for (int i = 0; i < filteredAssetType.AssetTypeLevel1.Count; i++)
                //        {
                //            var subType = filteredAssetType.AssetTypeLevel1[i];
                //            assetColumns.Insert(index, new DataTableViewModel
                //            {
                //                title = subType.Name,
                //                data = $"AssetsAssociationTypeSubLevels.{i}.AssetTypeLevel2.Name",
                //                orderable = false,
                //                isEditable = true,
                //                editableColumnDetail = new EditableCellDetails(
                //                    "AssetAssociation",
                //                    "AssetTypeLevel2Id",
                //                    subType.Name,
                //                    subType.Name,
                //                    "hidden-asset-create-form",
                //                    false,
                //                    $"AssetsAssociationTypeSubLevels[{i}].Id"
                //                )
                //            });
                //            index++;

                //        }
                //    }
                //}
                assetColumnsDictionary.Add(assetType.Id, assetColumns);
            }
            return assetColumnsDictionary;
        }

        public async Task<JsonResult> GetMapData()
        {
            var assetsResponse = await _service.GetAll<AssetDetailViewModel>(new AssetSearchViewModel() { ForMapData = true, DisablePagination = true });
            var resposne = assetsResponse as IRepositoryResponseWithModel<PaginatedResultModel<AssetDetailViewModel>>;
            var mapData = _mapper.Map<List<AssetMapViewModel>>(resposne.ReturnModel.Items);
            return Json(mapData);
        }

        public async Task<ActionResult> AssetWorkHistory(long id)
        {
            var result = new List<WorkOrderDetailViewModel>();
            var filter = new WorkOrderSearchViewModel()
            {
                Id = id
            };
            var historyResponse = await _workOrderService.GetAll<WorkOrderDetailViewModel>(filter);
            if (historyResponse.Status == HttpStatusCode.OK)
            {
                var history = (historyResponse as RepositoryResponseWithModel<PaginatedResultModel<WorkOrderDetailViewModel>>)?.ReturnModel;
                result = history?.Items ?? new();
            }
            return View("_AssetWorkHistory", result);
        }

        public async Task<ActionResult> _AssetApprovedWorkHistory(long id)
        {
            List<WorkOrderDetailViewModel> result = await GetAssetApprovedWorkOrderData(id);
            return View("_AssetApprovedWorkHistory", result);
        }



        private async Task<List<INotesViewModel>> getAssetNotes(long id)
        {
            var response = await _service.GetNotesByAssetId(id);
            var model = new List<AssetNotesViewModel>();
            if (response.Status == HttpStatusCode.OK)
            {
                var responseModel = response as RepositoryResponseWithModel<List<AssetNotesViewModel>>;
                model = responseModel.ReturnModel;
            }
            List<INotesViewModel> notesViewModels = model.Cast<INotesViewModel>().ToList();
            return notesViewModels;
        }
        private async Task<List<WorkOrderDetailViewModel>> GetAssetApprovedWorkOrderData(long id)
        {
            var result = new List<WorkOrderDetailViewModel>();
            var filter = new WorkOrderSearchViewModel()
            {
                Id = id,
                Status = WOStatusCatalog.Approved
            };
            var historyResponse = await _workOrderService.GetAll<WorkOrderDetailViewModel>(filter);
            if (historyResponse.Status == HttpStatusCode.OK)
            {
                var history = (historyResponse as RepositoryResponseWithModel<PaginatedResultModel<WorkOrderDetailViewModel>>)?.ReturnModel;
                result = history?.Items ?? new();
            }

            return result;
        }
    }
}
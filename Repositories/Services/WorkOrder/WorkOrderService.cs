using Models.Common.Interfaces;
using ViewModels.Shared;
using Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Centangle.Common.ResponseHelpers.Models;
using DataLibrary;
using Pagination;
using ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Repositories.Services.AttachmentService.Interface;
using Enums;
using ViewModels.Shared.Notification;
using Microsoft.AspNetCore.Http;
using Repositories.Shared.NotificationServices.Interface;
using Microsoft.AspNetCore.Hosting;
using Helpers.Extensions;
using Centangle.Common.ResponseHelpers;
using Helpers.File;
using ViewModels.WorkOrderTechnician;
using ViewModels.WorkOrderTasks;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.Shared.Notes;
using System.Linq.Expressions;
using ViewModels.WorkOrder;

namespace Repositories.Common
{
    public class WorkOrderService<CreateViewModel, UpdateViewModel, DetailViewModel> : BaseService<WorkOrder, CreateViewModel, UpdateViewModel, DetailViewModel>,
        IWorkOrderService<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, IDynamicColumns, new()
        where CreateViewModel : class, IBaseCrudViewModel, IDynamicColumns, IWorkOrderModifyViewModel, IIdentitifier, new()
        where UpdateViewModel : WorkOrderModifyViewModel, IDynamicColumns, IWorkOrderModifyViewModel, IIdentitifier, new()
    {
        private readonly ModelStateDictionary _modelState;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<WorkOrderService<CreateViewModel, UpdateViewModel, DetailViewModel>> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IAttachment _attachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> _inventoryService;
        private readonly IFileHelper _fileHelper;
        private readonly IUserInfoService _userInfo;
        private readonly ITaskTypeService<TaskTypeModifyViewModel, TaskTypeModifyViewModel, TaskTypeDetailViewModel> _taskTypeService;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> _equipmentTransactionService;
        private readonly IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnModifyViewModel> _dynamicColumnService;
        private readonly ITimesheetService _timesheetService;

        public WorkOrderService
            (
                ApplicationDbContext db,
                ILogger<WorkOrderService<CreateViewModel, UpdateViewModel, DetailViewModel>> logger,
                IMapper mapper,
                IRepositoryResponse response,
                IActionContextAccessor actionContext,
                IAttachment attachmentService,
                IHostingEnvironment hostingEnvironment,
                INotificationService notificationService,
                IHttpContextAccessor httpContextAccessor,
                IInventoryService<InventoryModifyViewModel, InventoryModifyViewModel, InventoryDetailViewModel> inventoryService,
                IFileHelper fileHelper,
                IUserInfoService userInfo,
                ITaskTypeService<TaskTypeModifyViewModel, TaskTypeModifyViewModel, TaskTypeDetailViewModel> taskTypeService,
                ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService,
                IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> equipmentTransactionService,
                IDynamicColumnService<DynamicColumnModifyViewModel, DynamicColumnModifyViewModel, DynamicColumnModifyViewModel> dynamicColumnService
                , ITimesheetService timesheetService
            ) : base(db, logger, mapper, response)
        {
            _modelState = actionContext.ActionContext.ModelState;
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _response = response;
            _attachmentService = attachmentService;
            _hostingEnvironment = hostingEnvironment;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _inventoryService = inventoryService;
            _fileHelper = fileHelper;
            _userInfo = userInfo;
            _taskTypeService = taskTypeService;
            _transactionService = transactionService;
            _equipmentTransactionService = equipmentTransactionService;
            _dynamicColumnService = dynamicColumnService;
            this._timesheetService = timesheetService;
        }

        public async override Task<Expression<Func<WorkOrder, bool>>> SetQueryFilter(IBaseSearchModel filters)
        {
            var searchFilters = filters as WorkOrderSearchViewModel;

            DateTime? FromDate = searchFilters?.FromDate;
            DateTime? ToDate = searchFilters?.ToDate;
            if (FromDate == null || FromDate == DateTime.MinValue)
            {
                //FromDate = DateTime.Today.AddDays(-15);
            }
            if (ToDate == null || ToDate == DateTime.MinValue)
            {
                //ToDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            }

            var userRole = _userInfo.LoggedInUserRole();
            var isUserTechnician = userRole == RolesCatalog.Technician.ToString();
            var isUserManager = userRole == RolesCatalog.Manager.ToString();
            var loggedInUserId = long.Parse(_userInfo.LoggedInUserId());
            string searchKeyword = searchFilters?.Search?.value?.ToLower();
            return w =>
                        (
                            (
                                string.IsNullOrEmpty(searchKeyword)
                                ||
                                w.SystemGeneratedId.ToLower().Contains(searchKeyword)
                                ||
                                w.Title.ToLower().Contains(searchKeyword)
                                ||
                                w.Description.ToLower().Contains(searchKeyword)
                                ||
                                (w.Asset != null && w.Asset.SystemGeneratedId.ToLower().Contains(searchKeyword))
                                ||
                                w.Manager.FirstName.ToLower().Contains(searchKeyword)
                            )
                             &&
                            (searchFilters.Id == null || w.Asset.Id == searchFilters.Id)
                            &&
                            (searchFilters.NotStatus == null || w.Status != searchFilters.NotStatus)
                            &&
                            (searchFilters.Status == null || w.Status == searchFilters.Status)
                            &&
                            (searchFilters.AssetId == null || w.Asset.SystemGeneratedId == searchFilters.AssetId)
                            &&
                            (searchFilters.Manager.Id == 0 || w.Manager.Id == searchFilters.Manager.Id)
                            &&
                            (searchFilters.AssetType.Id == null || w.Asset.AssetType.Id == searchFilters.AssetType.Id)
                            &&
                            (searchFilters.Task == null || w.Task == searchFilters.Task)
                            &&
                            (searchFilters.Urgency == null || w.Urgency == searchFilters.Urgency)
                            &&
                            (isUserManager == false || w.ManagerId == loggedInUserId)
                            &&
                            (isUserTechnician == false || w.Technicians.Any(x => x.TechnicianId == loggedInUserId))
                            &&
                                (FromDate == null || w.CreatedOn >= FromDate)
                                &&
                                (ToDate == null || w.CreatedOn <= ToDate)
                        );
        }

        public async override Task<IRepositoryResponse> Create(CreateViewModel model)
        {
            var viewModel = model as WorkOrderModifyViewModel;
            model.Replace.Id = model.Replace.Id != 0 && model.Task == TaskCatalog.Replace ? model.Replace.Id : null;
            model.Repair.Id = model.Repair.Id != 0 && model.Task == TaskCatalog.Repair ? model.Repair.Id : null;
            var totalWorkOrdersCount = await _db.WorkOrder.IgnoreQueryFilters().CountAsync();
            model.SystemGeneratedId = "WOS-" + (totalWorkOrdersCount + 1).ToString("D4");
            if (model.WorkOrderMaterials != null && model.WorkOrderMaterials.Count > 0)
            {
                model.TotalMaterialCost = await GetMaterialCost(model.WorkOrderMaterials);
            }
            model.Status = WOStatusCatalog.Open;
            if (viewModel.File != null)
            {
                viewModel.DefaultImageUrl = _fileHelper.Save(viewModel);
            }
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (model.Asset?.Id > 0)
                {
                    model.Intersection = model.Asset.Street;
                    model.AssetType = new AssetTypeBriefViewModel
                    {
                        Id = model.Asset.AssetTypeId
                    };
                }
                var response = await base.Create(model);
                long id = 0;
                if (response.Status == System.Net.HttpStatusCode.OK)
                {
                    var parsedResponse = response as RepositoryResponseWithModel<long>;
                    id = parsedResponse?.ReturnModel ?? 0;
                    model.Id = id;
                    if (model.Images.Count > 0)
                    {
                        var attachments = new List<AttachmentVM>();
                        foreach (var image in model.Images)
                        {
                            attachments.Add(new AttachmentVM() { File = image });
                        }
                        attachments.ForEach(x => x.EntityId = id);
                        attachments.ForEach(x => x.EntityType = Enums.AttachmentEntityType.WorkOrders);
                        await _attachmentService.CreateMultiple(attachments);
                    }

                    if (model.WorkOrderLabors.Count > 0)
                    {
                        await SetWorkOrderLabors(model.WorkOrderLabors, id);
                    }

                    if (model.WorkOrderMaterials.Count > 0)
                    {
                        await SetWorkOrderMaterials(model.WorkOrderMaterials, id);
                    }
                    if (model.WorkOrderTechnicians.Count > 0)
                    {
                        await SetWorkOrderTechnicians(model.WorkOrderTechnicians, id);
                    }
                    if (model.StreetServiceRequestId != null && model.StreetServiceRequestId > 0)
                    {
                        var serviceRequest = await _db.StreetServiceRequests.FindAsync(model.StreetServiceRequestId);
                        if (serviceRequest != null)
                        {
                            serviceRequest.Status = SSRStatus.WOCreated;
                            await _db.SaveChangesAsync();
                        }
                    }
                    await _dynamicColumnService.UpdateValues(model);
                    var manager = await _db.Users.Where(x => x.Id == model.Manager.Id).FirstOrDefaultAsync();
                    if (model.Asset.SystemGeneratedId != null)
                        await SendNotificationToManager(manager.FirstName + " " + manager.LastName, manager.Email, id, model.Asset.SystemGeneratedId);
                    await transaction.CommitAsync();
                    return response;

                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Create() for Work Order threw the following exception");
            }
            return Response.BadRequestResponse(_response);
        }

        public async override Task<IRepositoryResponse> Update(UpdateViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var viewModel = model as WorkOrderModifyViewModel;
                if (viewModel.File != null)
                {
                    viewModel.DefaultImageUrl = _fileHelper.Save(viewModel);
                }
                if (model.WorkOrderMaterials != null && model.WorkOrderMaterials.Count > 0)
                {
                    model.TotalMaterialCost = await GetMaterialCost(model.WorkOrderMaterials);
                }

                var record = await _db.Set<WorkOrder>().FindAsync(model?.Id);
                model.SystemGeneratedId = record.SystemGeneratedId;
                SetUpdateValues(model, record);
                if (record != null)
                {
                    var dbModel = _mapper.Map(model, record);
                    await _db.SaveChangesAsync();
                }

                await SaveImages(model.Images, record.Id);

                if (model?.WorkOrderLabors?.Count > 0)
                {
                    await SetWorkOrderLabors(model.WorkOrderLabors, model.Id);
                }

                if (model?.WorkOrderMaterials?.Count > 0)
                {
                    await SetWorkOrderMaterials(model.WorkOrderMaterials, model.Id);
                }

                if (model?.WorkOrderEquipments?.Count > 0)
                {
                    await SetWorkOrderEquipments(model.WorkOrderEquipments, model.Id);
                }

                if (model?.WorkOrderTechnicians?.Count > 0)
                {
                    await SetWorkOrderTechnicians(model.WorkOrderTechnicians, model.Id);
                }
                await _dynamicColumnService.UpdateValues(model);
                await transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Update() for Work Order threw the following exception");
                return Response.BadRequestResponse(_response);
            }

        }

        private void SetUpdateValues(UpdateViewModel model, WorkOrder record)
        {
            if (model.Asset.Id == 0)
            {
                model.Asset.Id = null;
            }
            if (model?.AssetType?.Id == 0)
            {
                model.AssetType.Id = null;
            }
            model.ActualHours = record.ActualHours;
            model.ActualCost = record.ActualCost;
            model.MaterialCost = record.MaterialCost;
            model.LabourCost = record.LabourCost;
            model.EquipmentCost = record.EquipmentCost;
            model.TotalCost = record.TotalCost;
            model.TotalHours = record.TotalHours;

        }

        public async Task<IRepositoryResponse> UploadImages(WorkOrderAddImageViewModel model)
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var workOrder = await _db.WorkOrder.FindAsync(model.Id);
                if (workOrder != null)
                {
                    await SaveImages(model.Images, model.Id);
                    await transaction.CommitAsync();
                    return _response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"UploadImages() for Work Order threw the following exception");
                return Response.BadRequestResponse(_response);
            }

        }

        public async Task<IRepositoryResponse> UpdateStatus(WorkOrderModifyStatusViewModel model)
        {
            var _transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var record = await _db.Set<WorkOrder>().FindAsync(model?.Id);
                if (record != null)
                {
                    record.Status = model.Status;
                    if (model.Status == WOStatusCatalog.Approved)
                    {
                        var asset = await _db.Assets.Where(x => x.Id == record.AssetId).FirstOrDefaultAsync();
                        record.ApprovalDate = DateTime.UtcNow;
                        if (asset != null)
                        {
                            if (record.Task == TaskCatalog.Maintenance)
                            {
                                asset.NextMaintenanceDate = DateTime.UtcNow.AddYears((int)asset.NextMaintenanceYear);
                            }
                            if (record.Task == TaskCatalog.Replace)
                            {
                                asset.ReplacementDate = DateTime.UtcNow.AddYears((int)asset.ReplacementYear);
                            }
                            if (record.Task == TaskCatalog.Remove)
                            {
                                var outOfService = await _db.Conditions.Where(x => x.Name == "Out Of Service ").FirstOrDefaultAsync();
                                if (outOfService != null)
                                {
                                    asset.ConditionId = outOfService.Id;
                                }
                            }
                        }
                        record.ApprovalDate = DateTime.UtcNow;

                    }
                    await _db.SaveChangesAsync();
                }
                WorkOrderComment woc = new WorkOrderComment()
                {
                    WorkOrderId = model.Id ?? 0,
                    Comment = model.Comment,
                    Status = model.Status
                };
                _db.WorkOrderComments.Add(woc);
                await _db.SaveChangesAsync();

                if (model.Images.Count > 0)
                {
                    var attachments = new List<AttachmentVM>();
                    foreach (var image in model.Images)
                    {
                        attachments.Add(new AttachmentVM() { File = image });
                    }
                    attachments.ForEach(x => x.EntityId = woc.Id);
                    attachments.ForEach(x => x.EntityType = Enums.AttachmentEntityType.WorkOrdersStatus);
                    await _attachmentService.CreateMultiple(attachments);
                }
                await _transaction.CommitAsync();
                var response = new RepositoryResponseWithModel<long> { ReturnModel = record.Id };
                return response;
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError(ex, $"UpdateStatus() for Work Order threw the following exception");
                return Response.BadRequestResponse(_response);
            }

        }

        public async override Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                var searchFilters = search as WorkOrderSearchViewModel;

                DateTime? FromDate = searchFilters?.FromDate;
                DateTime? ToDate = searchFilters?.ToDate;
                if (FromDate == null || FromDate == DateTime.MinValue)
                {
                    FromDate = DateTime.Today.AddDays(-15);
                }
                if (ToDate == null || ToDate == DateTime.MinValue)
                {
                    ToDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                }

                var userRole = _userInfo.LoggedInUserRole();
                Enum.TryParse(userRole, out RolesCatalog loggedInUserRole);
                var isUserTechnician = userRole == RolesCatalog.Technician.ToString();
                var isUserManager = userRole == RolesCatalog.Manager.ToString();
                var loggedInUserId = long.Parse(_userInfo.LoggedInUserId());
                var filters = await SetQueryFilter(search);
                var workOrdersQueryable = (from w in _db.WorkOrder.Where(filters)
                                          .Include(x => x.AssetType)
                                          .Include(x => x.Asset).ThenInclude(x => x.AssetType)
                                          .Include(x => x.Technicians)
                                          .Include(x => x.TaskType)
                                           join replace in _db.Replaces on w.ReplaceId equals replace.Id into wreplace
                                           from replace in wreplace.DefaultIfEmpty()
                                           join repair in _db.Repairs on w.RepairId equals repair.Id into wrepair
                                           from repair in wrepair.DefaultIfEmpty()
                                           join m in _db.Users on w.CreatedBy equals m.Id
                                           select new WorkOrderDetailViewModel
                                           {
                                               Id = w.Id,
                                               LoggedInUserRole = loggedInUserRole,
                                               Asset = new ViewModels.WorkOrder.WOAssetViewModel
                                               {
                                                   Id = w.Asset != null ? w.Asset.Id : 0,
                                                   SystemGeneratedId = w.Asset != null ? w.Asset.SystemGeneratedId : "-",
                                                   AssetType = w.Asset != null ? w.Asset.AssetType.Name : "",
                                                   Street = w.Asset != null ? w.Asset.Intersection : "-",
                                                   Latitude = w.Asset != null ? w.Asset.Latitude : 0,
                                                   Longitude = w.Asset != null ? w.Asset.Longitude : 0,
                                               },
                                               Description = w.Asset != null ? w.Asset.Description : w.Description,
                                               Title = w.Title,
                                               Manager = new ViewModels.Manager.ManagerBriefViewModel
                                               {
                                                   Id = m.Id,
                                                   Name = m.FirstName + " " + m.LastName,
                                               },
                                               Repair = new RepairBriefViewModel
                                               {
                                                   Id = repair != null ? repair.Id : 0,
                                                   Name = repair != null ? repair.Name : ""
                                               },
                                               Replace = new ReplaceBriefViewModel
                                               {
                                                   Id = replace != null ? replace.Id : 0,
                                                   Name = replace != null ? replace.Name : ""
                                               },
                                               TaskType = new TaskTypeBriefViewModel
                                               {
                                                   Id = w.TaskType != null ? w.TaskType.Id : 0,
                                                   Code = w.TaskType != null ? w.TaskType.Code : "-",
                                                   BudgetHours = w.TaskType != null ? w.TaskType.BudgetHours : 0,
                                                   Labor = w.TaskType != null ? w.TaskType.Labor : 0,
                                                   Material = w.TaskType != null ? w.TaskType.Material : 0,
                                                   Equipment = w.TaskType != null ? w.TaskType.Equipment : 0,
                                                   BudgetCost = w.TaskType != null ? w.TaskType.BudgetCost : 0
                                               },
                                               AssetTypeName = w.AssetType != null ? w.AssetType.Name : "",
                                               Intersection = w.Intersection,
                                               Status = w.Status,
                                               Task = w.Task,
                                               Type = w.Type,
                                               Urgency = w.Urgency,
                                               TotalCost = w.TotalCost,
                                               TotalHours = w.TotalHours,
                                               ActualCost = w.ActualCost,
                                               ActualHours = w.ActualHours,
                                               MaterialCost = w.MaterialCost,
                                               EquipmentCost = w.EquipmentCost,
                                               LabourCost = w.LabourCost,
                                               SystemGeneratedId = w.SystemGeneratedId,
                                               ApprovalDate = w.ApprovalDate,
                                               CreatedOn = w.CreatedOn,
                                               DueDate = w.DueDate ?? DateTime.MinValue
                                           }).AsQueryable();

                var attachments = await GetWorkOrderAttachments(search);
                var notes = await _db.WorkOrderNotes.GroupBy(x => x.WorkOrderId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count > 0).ToListAsync();
                var result = await workOrdersQueryable.Paginate(search);

                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<WorkOrderDetailViewModel>();
                    paginatedResult.Items = _mapper.Map<List<WorkOrderDetailViewModel>>(result.Items);
                    foreach (var item in paginatedResult.Items)
                    {
                        if (item is IHasNotes && item is INullableIdentitifier)
                        {
                            (item as IHasNotes).HasNotes = notes.Any(x => x.Id == (item as INullableIdentitifier).Id);
                        }
                        item.ImagesList = attachments.Where(x => x.EntityId == item.Id).ToList();
                    }
                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<WorkOrderDetailViewModel>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(WorkOrder).FullName} in GetAll()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for {typeof(WorkOrder).FullName} threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        public async override Task<IRepositoryResponse> GetById(long id)
        {
            try
            {
                var workOrder = await _db.WorkOrder
                                        .Include(x => x.AssetType)
                                        .Include(x => x.Asset).ThenInclude(x => x.AssetType)
                                        .Include(x => x.TaskType)
                                        .Include(x => x.Replace)
                                        .Include(x => x.Repair)
                                        .Join(_db.Users, w => w.CreatedBy, m => m.Id, (w, m) => new { w, m })
                                        .Where(x => x.w.Id == id)
                                        .Select(x => new WorkOrderDetailViewModel
                                        {
                                            Id = x.w.Id,
                                            Asset = new WOAssetViewModel
                                            {
                                                Id = x.w.Asset != null ? x.w.Asset.Id : 0,
                                                SystemGeneratedId = x.w.Asset != null ? x.w.Asset.SystemGeneratedId : "-",
                                                AssetType = x.w.Asset != null ? x.w.Asset.AssetType.Name : "-",
                                                AssetTypeId = x.w.Asset != null ? x.w.Asset.AssetType.Id : 0,
                                                Description = x.w.Asset != null ? x.w.Asset.Description : x.w.Description,
                                                Street = x.w.Asset != null ? x.w.Asset.Intersection : "-",
                                                Latitude = x.w.Asset != null ? x.w.Asset.Latitude : 0,
                                                Longitude = x.w.Asset != null ? x.w.Asset.Longitude : 0
                                            },
                                            Description = x.w.Description,
                                            Title = x.w.Title,
                                            DefaultImageUrl = x.w.DefaultImageUrl,
                                            Manager = new ViewModels.Manager.ManagerBriefViewModel
                                            {
                                                Id = x.m.Id,
                                                Name = x.m.FirstName + " " + x.m.LastName,
                                                Telephone = x.m.PhoneNumber,
                                                Email = x.m.Email
                                            },
                                            Repair = new RepairBriefViewModel
                                            {
                                                Id = x.w.Repair != null ? x.w.Repair.Id : 0,
                                                Name = x.w.Repair != null ? x.w.Repair.Name : ""
                                            },
                                            Replace = new ReplaceBriefViewModel
                                            {
                                                Id = x.w.Replace != null ? x.w.Replace.Id : 0,
                                                Name = x.w.Replace != null ? x.w.Replace.Name : ""
                                            },
                                            TaskType = new TaskTypeBriefViewModel
                                            {
                                                Id = x.w.TaskType != null ? x.w.TaskType.Id : 0,
                                                Title = x.w.TaskType != null ? x.w.TaskType.Title : "",
                                                Code = x.w.TaskType != null ? x.w.TaskType.Code : "",
                                                BudgetCost = x.w.TaskType != null ? x.w.TaskType.BudgetCost : 0,
                                                BudgetHours = x.w.TaskType != null ? x.w.TaskType.BudgetHours : 0,
                                            },
                                            AssetType = new AssetTypeBriefViewModel(false, "")
                                            {
                                                Id = x.w.AssetType != null ? x.w.AssetType.Id : 0,
                                                Name = x.w.AssetType != null ? x.w.AssetType.Name : "",
                                            },
                                            AssetTypeName = x.w.AssetType != null ? x.w.AssetType.Name : "",
                                            Intersection = x.w.Intersection,
                                            Status = x.w.Status,
                                            Task = x.w.Task,
                                            Type = x.w.Type,
                                            Urgency = x.w.Urgency,
                                            SystemGeneratedId = x.w.SystemGeneratedId,
                                            TotalCost = x.w.TotalCost,
                                            TotalHours = x.w.TotalHours,
                                            DueDate = x.w.DueDate,
                                            ApprovalDate = x.w.ApprovalDate,
                                            CreatedOn = x.w.CreatedOn,
                                            UpdatedOn = x.w.UpdatedOn
                                        })
                                        .FirstOrDefaultAsync();

                if (workOrder != null)
                {
                    workOrder.ImagesList = await GetWorkOrderAttachments(workOrder.Id ?? 0);
                    workOrder.DynamicColumns = await _dynamicColumnService.GetDynamicColumns(DynamicColumnEntityType.WorkOrder, workOrder.Id ?? 0);
                    workOrder.Comments = await GetWorkOrderComments(workOrder.Id ?? 0);
                    //workOrder.WorkOrderLabors = await GetWorkOrderLabors(workOrder.Id ?? 0);
                    //workOrder.WorkOrderMaterials = await GetWorkOrderMaterials(workOrder.Id ?? 0);
                    //workOrder.WorkOrderEquipments = await GetWorkOrderEquipments(workOrder.Id ?? 0);
                    workOrder.WorkOrderTechnicians = await GetWorkOrderTechnicians(workOrder.Id ?? 0);
                    workOrder.CostPerformance.TaskWorkSteps = await _taskTypeService.GetTaskSteps((long)workOrder.TaskType.Id);
                    workOrder.CostPerformance.TaskLabors = await _taskTypeService.GetTaskLabors((long)workOrder.TaskType.Id);
                    workOrder.CostPerformance.TaskEquipments = await _taskTypeService.GetTaskEquipments((long)workOrder.TaskType.Id);
                    workOrder.CostPerformance.TaskMaterials = await _taskTypeService.GetTaskMaterials((long)workOrder.TaskType.Id);
                    workOrder.CostPerformance.EquipmentTransactions = await _equipmentTransactionService.GetWorkOrderTransactions(workOrder.SystemGeneratedId);
                    workOrder.CostPerformance.MaterialTransactions = await _transactionService.GetWorkOrderTransactions(workOrder.SystemGeneratedId);
                    await GetWorkOrderTimesheets(workOrder);

                    workOrder.CostPerformance.SetMaterialCostPerformance();
                    workOrder.CostPerformance.SetEquipmentCostPerformance();
                    workOrder.CostPerformance.SetLaborCostPerformance();
                    //await GetActualEquipmentCosts(id, workOrder);
                    //await GetActualMaterialCosts(id, workOrder);
                    var response = new RepositoryResponseWithModel<WorkOrderDetailViewModel> { ReturnModel = workOrder };
                    return response;
                }
                _logger.LogWarning($"No record found for id:{id} for {typeof(WorkOrder).FullName} in GetById()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(WorkOrder).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<string> GetAttachmentUrl(long id)
        {
            try
            {
                var attachmentUrl = await _db.Attachments.Where(x => x.EntityId == id).Select(x => x.Url).FirstOrDefaultAsync();
                return attachmentUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<AttachmentVM>> GetWorkOrderAttachments(long id)
        {
            var attachments = await _db.Attachments.Where(x => x.EntityId == id && x.EntityType == Enums.AttachmentEntityType.WorkOrders).Select(x => new AttachmentVM
            {
                Id = x.Id,
                EntityId = (long)x.EntityId,
                EntityType = (Enums.AttachmentEntityType)x.EntityType,
                Url = x.Url,
                Name = x.Name,
                Type = x.Type,
                CreatedOn = x.CreatedOn
            }).ToListAsync();

            return attachments;
        }

        public async Task<IRepositoryResponse> GetNotesByWorkOrderId(long id)
        {
            try
            {
                var notes = await (from n in _db.WorkOrderNotes.Include(x => x.WorkOrder)
                                   join u in _db.Users on n.CreatedBy equals u.Id
                                   where (n.WorkOrderId == id)
                                   select new WorkOrderNotesViewModel
                                   {
                                       Id = n.Id,
                                       WorkOrderId = n.WorkOrderId,
                                       Description = n.Description,
                                       FileUrl = n.FileUrl,
                                       CreatedOn = n.CreatedOn,
                                       CreatedBy = u.FirstName + " " + u.LastName,
                                   }).ToListAsync();
                var response = new RepositoryResponseWithModel<List<WorkOrderNotesViewModel>> { ReturnModel = notes };
                return response;
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> SaveNotes(WorkOrderNotesViewModel model)
        {
            try
            {
                model.FileUrl = _fileHelper.Save(model);
                var mappedNotes = _mapper.Map<WorkOrderNotes>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(WorkOrder).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task SendNotificationToManager(string name, string email, long workOrderId, string assetId)
        {
            //var request = _httpContextAccessor.HttpContext.Request;
            //var baseUrl = $"{request.Scheme}://{request.Host}";
            //var link = $"{baseUrl}/WorkOrder/Detail?id={workOrderId}";
            await SendMail(name, email, workOrderId, assetId);
        }

        public async Task<bool> CreateTasks(WorkOrderTasksModifyViewModel model)
        {
            try
            {
                var mappedNotes = _mapper.Map<WorkOrderTasks>(model);
                await _db.AddAsync(mappedNotes);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<WorkOrderTasksIndexViewModel> GetWorkOrderTasks(long id)
        {
            try
            {
                var workOrderTasks = await _db.WorkOrderTasks.Where(x => x.WorkOrderId == id).ToListAsync();
                var completedTasks = workOrderTasks.Where(x => x.Status == WOTaskStatusCatalog.Completed).ToList();
                var unCompletedTasks = workOrderTasks.Where(x => x.Status == WOTaskStatusCatalog.Pending).ToList();
                var completedMappedList = _mapper.Map<List<WorkOrderTasksDetailViewModel>>(completedTasks);
                var unCompletedMappedList = _mapper.Map<List<WorkOrderTasksDetailViewModel>>(unCompletedTasks);
                WorkOrderTasksIndexViewModel workOrderTaskIndex = new();
                workOrderTaskIndex.CompletedTasks = completedMappedList;
                workOrderTaskIndex.UnCompletedTasks = unCompletedMappedList;
                return workOrderTaskIndex;

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<long> SetWorkOrderTaskStatus(long id, WOTaskStatusCatalog status)
        {
            try
            {
                var checkList = await _db.WorkOrderTasks.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (checkList != null)
                {
                    if (status != WOTaskStatusCatalog.Deleted)
                    {
                        checkList.Status = status;
                    }
                    else
                    {
                        checkList.IsDeleted = true;
                    }
                    _db.SaveChanges();
                    return checkList.Id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SetCheckListStatus method for CheckList threw an exception.");
            }
            return -1;
        }

        #region private functions

        private async Task<List<AttachmentVM>> GetWorkOrderAttachments(IBaseSearchModel search)
        {
            var searchFilters = search as WorkOrderSearchViewModel;
            var filters = await SetQueryFilter(searchFilters);
            var attachmentsQueryable = (from att in _db.Attachments
                                        join wo in _db.WorkOrder.Where(filters)
                                          .Include(x => x.Asset).ThenInclude(x => x.AssetType)
                                          on new { WorkOrderId = att.EntityId ?? 0, EntityType = (att.EntityType ?? AttachmentEntityType.WorkOrders) } equals new { WorkOrderId = wo.Id, EntityType = AttachmentEntityType.WorkOrders }
                                        select new AttachmentVM
                                        {
                                            Id = att.Id,
                                            Name = att.Name,
                                            Url = att.Url,
                                            EntityId = (long)att.EntityId,
                                            EntityType = (AttachmentEntityType)att.EntityType
                                        }).AsQueryable();

            return await attachmentsQueryable.ToListAsync();
        }

        private async Task<bool> SendMail(string managerName, string managerEmail, long workOrderId, string assetId)
        {
            try
            {
                if (managerEmail != null && !string.IsNullOrEmpty(assetId))
                {
                    string subject = "Assignment of Work Order";
                    string bodyForMail = GetBodyForMail(managerName, assetId);

                    var mailRequest = new MailRequestViewModel(managerEmail, subject, bodyForMail, workOrderId, NotificationEntityType.WorkOrderCreated, NotificationType.Email);

                    await _notificationService.MappNotification(mailRequest);
                    _logger.LogInformation("Notification saved Successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
            return true;
        }

        private string GetBodyForMail(string managerName, string assetId)
        {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Templates", "WorkOrderAssignmentEmailTemplate.html");
            if (File.Exists(filePath))
            {
                var str = File.ReadAllText(filePath);
                return str.Replace("<%MANAGER_NAME%>", managerName)
                    .Replace("<%ASSETID%>", assetId.ToString());
            }
            else
            {
                return string.Empty;
            }
        }

        private async Task SetWorkOrderMaterials(List<WorkOrderMaterialModifyViewModel> workOrderMaterials, long id)
        {
            try
            {
                var dbRecords = await _db.WorkOrderMaterials.Where(x => x.WorkOrderId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedWorkOrderMaterials = _mapper.Map<List<WorkOrderMaterial>>(workOrderMaterials);
                mappedWorkOrderMaterials.ForEach(x => x.WorkOrderId = id);
                await _db.AddRangeAsync(mappedWorkOrderMaterials);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        private async Task SetWorkOrderEquipments(List<WorkOrderEquipmentModifyViewModel> workOrderEquipments, long id)
        {
            try
            {
                var dbRecords = await _db.WorkOrderEquipments.Where(x => x.WorkOrderId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedWorkOrderEquipments = _mapper.Map<List<WorkOrderEquipment>>(workOrderEquipments);
                mappedWorkOrderEquipments.ForEach(x => x.WorkOrderId = id);
                await _db.AddRangeAsync(mappedWorkOrderEquipments);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        private async Task SetWorkOrderLabors(List<WorkOrderLaborModifyViewModel> workOrderLabors, long id)
        {
            try
            {
                var dbRecords = await _db.WorkOrderLabors.Where(x => x.WorkOrderId == id).ToListAsync();
                dbRecords.ForEach(x => x.IsDeleted = true);

                var mappedWorkOrderLabors = _mapper.Map<List<WorkOrderLabor>>(workOrderLabors);
                mappedWorkOrderLabors.ForEach(x => x.WorkOrderId = id);
                await _db.AddRangeAsync(mappedWorkOrderLabors);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

        }

        private async Task SetWorkOrderTechnicians(List<WorkOrderTechnicianModifyViewModel> workOrderTechnicians, long id)
        {
            try
            {
                var dbRecords = await _db.WorkOrderTechnicians.Where(x => x.WorkOrderId == id).ToListAsync();

                var mappedWorkOrderTechnicians = _mapper.Map<List<WorkOrderTechnician>>(workOrderTechnicians);
                mappedWorkOrderTechnicians.ForEach(x => x.WorkOrderId = id);

                var deletedRecords = dbRecords.Except(mappedWorkOrderTechnicians, new GenericCompare<WorkOrderTechnician>()).ToList();
                deletedRecords.ForEach(x => x.IsDeleted = true);

                var updatedRecords = dbRecords.Intersect(mappedWorkOrderTechnicians, new GenericCompare<WorkOrderTechnician>()).ToList();
                foreach (var oldsubActivity in updatedRecords)
                {
                    var subActivityVM = mappedWorkOrderTechnicians.Where(x => x.Id == oldsubActivity.Id).FirstOrDefault();

                    oldsubActivity.Id = subActivityVM.Id;
                    oldsubActivity.WorkOrderId = subActivityVM.WorkOrderId;
                    oldsubActivity.CraftSkillId = subActivityVM.CraftSkillId;
                    oldsubActivity.TechnicianId = subActivityVM.TechnicianId;
                }
                var addedRecords = mappedWorkOrderTechnicians.Where(x => x.Id < 1).ToList();
                await _db.AddRangeAsync(addedRecords);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }

        //private async Task<List<WorkOrderMaterialModifyViewModel>> GetWorkOrderMaterials(long id)
        //{
        //    try
        //    {
        //        var workOrderMaterials = await _db.WorkOrderMaterials
        //                                          .Include(x => x.Inventory)
        //                                          .Where(x => x.WorkOrderId == id)
        //                                          .Select(x => new WorkOrderMaterialModifyViewModel
        //                                          {
        //                                              Id = x.Id,
        //                                              Inventory = new InventoryBriefViewModel
        //                                              {
        //                                                  Id = x.Inventory.Id,
        //                                                  SystemGeneratedId = x.Inventory.SystemGeneratedId,
        //                                              },
        //                                              Quantity = x.Quantity
        //                                          })
        //                                         .ToListAsync();
        //        return workOrderMaterials;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new();
        //    }
        //}

        //private async Task<List<WorkOrderEquipmentModifyViewModel>> GetWorkOrderEquipments(long id)
        //{
        //    try
        //    {
        //        var workOrderEquipments = await _db.WorkOrderEquipments
        //                                          .Include(x => x.Equipment)
        //                                          .Where(x => x.WorkOrderId == id)
        //                                          .Select(x => new WorkOrderEquipmentModifyViewModel
        //                                          {
        //                                              Id = x.Id,
        //                                              Equipment = new EquipmentDetailViewModel
        //                                              {
        //                                                  Id = x.Equipment.Id,
        //                                                  ItemNo = x.Equipment.ItemNo,
        //                                                  Description = x.Equipment.Description
        //                                              },
        //                                              Quantity = x.Quantity,
        //                                              Hours = x.Hours
        //                                          })
        //                                         .ToListAsync();
        //        return workOrderEquipments;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new();
        //    }
        //}

        private async Task<List<WorkOrderTechnicianModifyViewModel>> GetWorkOrderTechnicians(long id)
        {
            try
            {
                var workOrderTechnicians = await _db.WorkOrderTechnicians
                                                  .Include(x => x.Technician)
                                                  .Where(x => x.WorkOrderId == id)
                                                  .Select(x => new WorkOrderTechnicianModifyViewModel
                                                  {
                                                      Id = x.Id,
                                                      Technician = new TechnicianBriefViewModel
                                                      {
                                                          Id = x.Technician.Id,
                                                          Name = x.Technician.FirstName + " " + x.Technician.LastName,
                                                      },
                                                      CraftSkill = new CraftSkillBriefViewModel
                                                      {
                                                          Id = x.CraftSkill.Id,
                                                          Name = x.CraftSkill.Name
                                                      }
                                                  })
                                                 .ToListAsync();
                return workOrderTechnicians;
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        //private async Task<List<WorkOrderLaborModifyViewModel>> GetWorkOrderLabors(long id)
        //{
        //    try
        //    {
        //        var workOrderLabors = await _db.WorkOrderLabors
        //                                            .Include(x => x.CraftSkill)
        //                                            .Where(x => x.WorkOrderId == id)
        //                                            .Select(x => new WorkOrderLaborModifyViewModel
        //                                            {
        //                                                Id = x.Id,
        //                                                MN = x.MN,
        //                                                DU = x.DU,
        //                                                Craft = new CraftSkillBriefViewModel
        //                                                {
        //                                                    Id = x.CraftId,
        //                                                    Name = x.CraftSkill.Name,
        //                                                    OTRate = x.CraftSkill.OTRate,
        //                                                    STRate = x.CraftSkill.STRate,
        //                                                    DTRate = x.CraftSkill.DTRate
        //                                                },
        //                                                //Estimate = x.Estimate,
        //                                                LaborEstimate = x.Estimate,
        //                                                Rate = x.Rate,
        //                                                LaborRate = x.Rate,
        //                                                Type = x.Type,
        //                                            })
        //                                            .ToListAsync();
        //        return workOrderLabors;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new();
        //    }

        //}

        private async Task<double> GetMaterialCost(List<WorkOrderMaterialModifyViewModel> inventories)
        {
            var inventoryAverageCosts = await _inventoryService.GetInventoryAverageCost(inventories.Select(x => (long)x.Inventory.Id).ToList());
            double totalCost = 0;
            foreach (var inventory in inventoryAverageCosts)
            {
                var invQuantity = inventories.Where(x => x.Inventory.Id == inventory.InventoryId).Select(x => x.Quantity).FirstOrDefault();
                totalCost += (invQuantity * inventory.AverageCost);
            }
            return totalCost;
        }

        private async Task SaveImages(List<IFormFile> attachments, long workOrderId)
        {
            if (attachments.Count > 0)
            {
                var attachmentList = new List<AttachmentVM>();
                foreach (var image in attachments)
                {
                    attachmentList.Add(new AttachmentVM() { File = image });
                }
                attachmentList.ForEach(x => x.EntityId = workOrderId);
                attachmentList.ForEach(x => x.EntityType = Enums.AttachmentEntityType.WorkOrders);
                await _attachmentService.CreateMultiple(attachmentList);
            }
        }

        //private async Task GetActualEquipmentCosts(long id, WorkOrderDetailViewModel? workOrder)
        //{
        //    //get actual Equipments
        //    //get orders for workOrder
        //    var workOrderEquipmentTransactions = await (
        //    from o in _db.Orders.Where(x => x.WorkOrderId == id)
        //    join oi in _db.OrderItems on o.Id equals oi.OrderId
        //    join eq in _db.Equipments on oi.EquipmentId equals eq.Id
        //    join et in _db.EquipmentTransactions.Where(x => x.TransactionType == EquipmentTransactionTypeCatalog.Order) on oi.Id equals et.EntityDetailId
        //    select new
        //    {
        //        EquipmentId = eq.Id,
        //        EquipmentName = eq.Description,
        //        SystemGeneratedId = eq.SystemGeneratedId,
        //        Cost = et.Quantity * et.ItemPrice,
        //    })
        //    .GroupBy(x => new { x.EquipmentId })
        //    .Select(x => new
        //    {
        //        EquipmentId = x.Key.EquipmentId,
        //        EquipmentName = x.Max(y => y.EquipmentName),
        //        SystemGeneratedId = x.Max(y=>y.SystemGeneratedId),
        //        Cost = x.Sum(s => s.Cost)
        //    })
        //    .ToListAsync();
        //    foreach (var transaction in workOrderEquipmentTransactions)
        //    {
        //        var matchingEquipment = workOrder.CostPerformance.TaskEquipments.FirstOrDefault(x => x.Equipment.Id == transaction.EquipmentId);
        //        if (matchingEquipment != null)
        //        {
        //            //matchingEquipment.ActualCost = Math.Abs(transaction.TotalCost);
        //        }
        //        else //UnPlanned
        //        {
        //            workOrder.CostPerformance.TaskEquipments.Add(new TaskEquipmentViewModel
        //            {
        //                Equipment = new EquipmentBriefViewModel
        //                {
        //                    Id = transaction.EquipmentId,
        //                    //SystemGeneratedId = transaction.SystemGeneratedId,
        //                    Description = transaction.EquipmentName
        //                },
        //                //ActualCost = Math.Abs(transaction.Cost)
        //            });
        //        }
        //    }
        //}

        //private async Task GetActualMaterialCosts(long id, WorkOrderDetailViewModel? workOrder)
        //{
        //    //get actual Materials
        //    //get orders for workOrder
        //    var workOrderMaterialTransactions = await (
        //    from o in _db.Orders.Where(x => x.WorkOrderId == id)
        //    join oi in _db.OrderItems on o.Id equals oi.OrderId
        //    join eq in _db.Inventories on oi.InventoryId equals eq.Id
        //    join et in _db.Transactions.Where(x => x.TransactionType == TransactionTypeCatalog.Order) on oi.Id equals et.EntityDetailId
        //    select new
        //    {
        //        MaterialId = eq.Id,
        //        MaterialName = eq.Description,
        //        //SystemGeneratedId = eq.SystemGeneratedId,
        //        Cost = et.Quantity * et.ItemPrice,
        //    })
        //    .GroupBy(x => new { x.MaterialId, x.MaterialName, x.SystemGeneratedId })
        //    .Select(x => new
        //    {
        //        MaterialId = x.Key.MaterialId,
        //        MaterialName = x.Key.MaterialName,
        //        SystemGeneratedId = x.Key.SystemGeneratedId,
        //        Cost = x.Sum(s => s.Cost)
        //    })
        //    .ToListAsync();
        //    foreach (var item in workOrder.CostPerformance.TaskMaterials)
        //    {
        //        var transaction = workOrderMaterialTransactions.Where(x => x.MaterialId == item.Material.Id).FirstOrDefault();
        //        //item.ActualCost = Math.Abs(transaction?.Cost ?? 0);
        //    }
        //    var estimatedMaterialIds = workOrder.CostPerformance.TaskMaterials.Select(x => x.Material.Id).ToList();
        //    var laterAddedActualMaterial = workOrderMaterialTransactions.Where(x => !estimatedMaterialIds.Contains(x.MaterialId)).ToList();
        //    foreach (var item in laterAddedActualMaterial)
        //    {
        //        workOrder.CostPerformance.TaskMaterials.Add(new TaskMaterialViewModel
        //        {
        //            Material = new InventoryBriefViewModel
        //            {
        //                Id = item.MaterialId,
        //                //SystemGeneratedId = item.SystemGeneratedId,
        //                Description = item.MaterialName
        //            },
        //            Cost = Math.Abs(item.Cost)
        //        });
        //    }
        //}

        private async Task GetWorkOrderTimesheets(WorkOrderDetailViewModel? workOrder)
        {
            try
            {
                var timeSheets = await _db.Timesheets
                               .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                               .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.CraftSkill)
                               .Include(x => x.TimesheetBreakdowns)
                               .Where(x => x.WorkOrderId == workOrder.Id).ToListAsync();
                foreach (var t in timeSheets)
                {
                    workOrder.CostPerformance.TimeSheets.Add(await _timesheetService.MapTimesheetVM(t));
                }
            }
            catch (Exception ex)
            {

            }

        }

        private async Task<List<WorkOrderCommentViewModel>> GetWorkOrderComments(long id)
        {
            var comments = await (from woc in _db.WorkOrderComments
                                  join u in _db.Users on woc.CreatedBy equals u.Id into ul
                                  from u in ul.DefaultIfEmpty()
                                  where woc.WorkOrderId == id
                                  select new WorkOrderCommentViewModel
                                  {
                                      Id = woc.Id,
                                      Comment = woc.Comment ?? "",
                                      Status = woc.Status,
                                      CreatedDate = woc.CreatedOn,
                                      CreatedBy = u == null ? "" : (u.FirstName + " " + u.LastName),

                                  }).ToListAsync();
            var commentsId = comments.Select(x => x.Id).ToList();
            var commentAttachments = await _db.Attachments
                .Where(x => commentsId.Contains(x.EntityId) && x.EntityType == Enums.AttachmentEntityType.WorkOrdersStatus)
                .Select(x => new AttachmentVM
                {
                    Id = x.Id,
                    EntityId = (long)x.EntityId,
                    EntityType = (Enums.AttachmentEntityType)x.EntityType,
                    Url = x.Url,
                    Name = x.Name,
                    Type = x.Type,
                    CreatedOn = x.CreatedOn
                }).ToListAsync();
            foreach (var comment in comments)
            {
                comment.ImagesList = commentAttachments.Where(x => x.EntityId == comment.Id).ToList();
            }
            return comments;
        }
        #endregion private functions

    }
}


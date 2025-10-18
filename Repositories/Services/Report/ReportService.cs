using DataLibrary;
using Helpers.Reports;
using Repositories.Services.Report.Common.interfaces;
using Repositories.Services.Report.Interface;
using Centangle.Common.ResponseHelpers.Models;
using Helpers.Extensions;
using Pagination;
using AutoMapper;
using Centangle.Common.ResponseHelpers;
using ViewModels.Report.PendingOrder;
using ViewModels.Report;
using ViewModels;
using Enums;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Shared.UserInfoServices.Interface;
using Microsoft.Extensions.Logging;
using ViewModels.Charts;
using ViewModels.Report.AssetsByCondition;
using ViewModels.Charts.Interfaces;
using Repositories.Common;
using ViewModels.Timesheet;
using ViewModels.Manager;
using ViewModels.Technician;
using ViewModels.Dashboard.Common.Card;
using Web.Extensions;
using ViewModels.Report.RawReport;
using ViewModels.WorkOrderTechnician;
using ViewModels.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using ViewModels.Asset;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace Repositories.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IReportServiceQueries _reportServiceQueries;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRepositoryResponse _response;
        private readonly IUserInfoService _userInfo;
        private readonly ILogger<ReportService> _logger;
        private readonly ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> _transactionService;
        private readonly IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> _equipmentTransactionService;

        public ReportService(
            IReportServiceQueries reportServiceQueries
            , ApplicationDbContext db
            , IMapper mapper
            , IRepositoryResponse response
            , IUserInfoService userInfo
            , ILogger<ReportService> logger
            , ITransactionService<TransactionModifyViewModel, TransactionModifyViewModel, TransactionDetailViewModel> transactionService
            , IEquipmentTransactionService<EquipmentTransactionModifyViewModel, EquipmentTransactionModifyViewModel, EquipmentTransactionDetailViewModel> equipmentTransactionService
          )
        {
            _reportServiceQueries = reportServiceQueries;
            _db = db;
            _mapper = mapper;
            _response = response;
            this._userInfo = userInfo;
            this._logger = logger;
            this._transactionService = transactionService;
            this._equipmentTransactionService = equipmentTransactionService;
        }

        public async Task<ChartResponseViewModel> GetAssetsByConditionChartData(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var responseQueryable = _db.Assets.Include(x => x.Condition)
                            .Select(x => new
                            {
                                AssetId = x.Id,
                                ConditionId = x.ConditionId,
                                ConditionName = x.Condition != null ? x.Condition.Name : "None"
                            })
                            .GroupBy(x => new { x.ConditionId, x.ConditionName })
                            .Select(x => new ChartViewModel
                            {
                                Category = x.Key.ConditionName,
                                Value = x.Count()
                            });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetWorkOrderByManagerChartData(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var responseQueryable = _db.WorkOrder.Include(x => x.Manager)

                            .Select(x => new { AssetId = x.Id, ManagerId = x.ManagerId, ManagerName = x.Manager != null ? x.Manager.FirstName + " " + x.Manager.LastName : "None" })
                            .GroupBy(x => new { x.ManagerId, x.ManagerName })
                            .Select(x => new ChartViewModel
                            {
                                Category = x.Key.ManagerName,
                                Value = x.Count()
                            });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetWorkOrderByTechnicianChartData(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var responseQueryable = (from wo in _db.WorkOrder
                                         join wot in _db.WorkOrderTechnicians.Include(x => x.Technician) on wo.Id equals wot.WorkOrderId
                                         select new
                                         {
                                             Id = wo.Id,
                                             TechnicianId = wot.TechnicianId,
                                             Technician = wot.Technician.FirstName + " " + wot.Technician.LastName
                                         })

                            .GroupBy(x => new { x.TechnicianId, x.Technician })
                            .Select(x => new ChartViewModel
                            {
                                Category = x.Key.Technician,
                                Value = x.Count()
                            });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetWorkOrderByAssetTypeChartData(WorkOrderChartSearchViewModel search)
        {
            try
            {

                var responseQueryable = _db.WorkOrder.Include(x => x.AssetType)
                           .Select(x => new
                           {
                               Id = x.Id,
                               AssetTypeId = x.AssetType != null ? x.AssetTypeId : 0,
                               AssetType = x.AssetType != null ? x.AssetType.Name : "None"
                           })
                           .GroupBy(x => new { x.AssetTypeId, x.AssetType })
                           .Select(x => new ChartViewModel
                           {
                               Category = x.Key.AssetType,
                               Value = x.Count()
                           });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetWorkOrderByRepairTypeChartData(WorkOrderChartSearchViewModel search)
        {
            try
            {
                var responseQueryable = _db.WorkOrder.Include(x => x.TaskType)
                            .Select(x => new
                            {
                                Id = x.Id,
                                TaskTypeId = x.TaskTypeId != null ? x.TaskTypeId : 0,
                                TaskType = x.TaskTypeId != null ? x.TaskType.Title : "None"
                            })
                            .GroupBy(x => new { x.TaskTypeId, x.TaskType })
                            .Select(x => new ChartViewModel
                            {
                                Category = x.Key.TaskType,
                                Value = x.Count()
                            });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetWorkOrderChartData(WorkOrderChartSearchViewModel search)
        {
            try
            {
                var responseQueryable = _db.WorkOrder

                            .Select(x => new { AssetId = x.Id, Status = x.Status })
                            .GroupBy(x => x.Status)
                            .Select(x => new ChartViewModel
                            {
                                Category = x.Key.GetDisplayName(),
                                Value = x.Count()
                            });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<IRepositoryResponse> GetWorkOrderReportData(IBaseSearchModel filters)
        {
            try
            {
                var search = filters as WorkOrderSearchViewModel;
                DateTime? FromDate = search?.FromDate;
                DateTime? ToDate = search?.ToDate;
                if (FromDate == null || FromDate == DateTime.MinValue)
                {
                    FromDate = DateTime.Today.AddDays(-15);
                }
                if (ToDate == null || ToDate == DateTime.MinValue)
                {
                    ToDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                }

                var userRole = _userInfo.LoggedInUserRole();
                var isUserTechnician = userRole == RolesCatalog.Technician.ToString();
                var isUserManager = userRole == RolesCatalog.Manager.ToString();
                var loggedInUserId = long.Parse(_userInfo.LoggedInUserId());
                var workOrderQueryable = (from w in _db.WorkOrder
                                          .Include(x => x.Asset).ThenInclude(x => x.AssetType)
                                          .Include(x => x.Technicians)
                                          .Include(x => x.TaskType)
                                          join replace in _db.Replaces on w.ReplaceId equals replace.Id into wreplace
                                          from replace in wreplace.DefaultIfEmpty()
                                          join repair in _db.Repairs on w.RepairId equals repair.Id into wrepair
                                          from repair in wrepair.DefaultIfEmpty()
                                          join m in _db.Users on w.CreatedBy equals m.Id
                                          where
                                          (
                                              (
                                                  string.IsNullOrEmpty(search.Search.value)
                                                  ||
                                                  w.Asset.SystemGeneratedId.ToLower().Contains(search.Search.value.ToLower())
                                                  ||
                                                  w.SystemGeneratedId.ToLower().Contains(search.Search.value.ToLower())
                                                  ||
                                                  w.Manager.FirstName.ToLower().Contains(search.Search.value.ToLower())
                                              )
                                              &&
                                              (search.AssetId == null || w.Asset.SystemGeneratedId == search.AssetId)
                                              &&
                                              (search.Manager.Id == 0 || w.Manager.Id == search.Manager.Id)
                                               &&
                                              (search.AssetType.Id == null || w.Asset.AssetType.Id == search.AssetType.Id)
                                              &&
                                              (search.Task == null || w.Task == search.Task)
                                              &&
                                              (search.Urgency == null || w.Urgency == search.Urgency)
                                              &&
                                              (isUserManager == false || w.ManagerId == loggedInUserId)
                                              &&
                                              (isUserTechnician == false || w.Technicians.Any(x => x.TechnicianId == loggedInUserId))
                                               &&
                                                (w.CreatedOn >= FromDate)
                                                &&
                                                (w.CreatedOn <= ToDate)
                                          )
                                          select new WorkOrderDetailViewModel
                                          {
                                              Id = w.Id,
                                              Asset = new ViewModels.WorkOrder.WOAssetViewModel
                                              {
                                                  Id = w.Asset != null ? w.Asset.Id : 0,
                                                  SystemGeneratedId = w.Asset != null ? w.Asset.SystemGeneratedId : "-",
                                                  AssetType = w.Asset != null ? w.Asset.AssetType.Name : "-",
                                                  Description = w.Asset != null ? w.Asset.Description : "-",
                                                  Street = w.Asset != null ? w.Asset.Intersection : "-",
                                              },

                                              Manager = new ViewModels.Manager.ManagerBriefViewModel
                                              {
                                                  Id = m.Id,
                                                  Name = m.FirstName + " " + m.LastName,
                                              },
                                              Repair = new RepairBriefViewModel
                                              {
                                                  Id = repair != null ? repair.Id : 0,
                                                  Name = repair != null ? repair.Name : "-"
                                              },
                                              Replace = new ReplaceBriefViewModel
                                              {
                                                  Id = replace != null ? replace.Id : 0,
                                                  Name = replace != null ? replace.Name : "-"
                                              },
                                              TaskType = w.TaskType != null ? new TaskTypeBriefViewModel
                                              {
                                                  Id = w.TaskType.Id,
                                                  Code = w.TaskType.Code,
                                                  BudgetHours = w.TaskType.BudgetHours,
                                                  Labor = w.TaskType.Labor,
                                                  Material = w.TaskType.Material,
                                                  Equipment = w.TaskType.Equipment,
                                              } : new(),
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
                                              SystemGeneratedId = w.SystemGeneratedId
                                          }).AsQueryable();

                var result = await workOrderQueryable.Paginate(search);
                if (result != null)
                {
                    var paginatedResult = new PaginatedResultModel<WorkOrderDetailViewModel>();
                    paginatedResult.Items = _mapper.Map<List<WorkOrderDetailViewModel>>(result.Items);
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

        public async Task<IRepositoryResponse> GetPendingOrderReportData(PendingOrderReportSearchViewModel search)
        {
            try
            {
                var PendingOrderQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderReportViewModel>(search);
                var paginatedData = await GetPaginatedResult(search, PendingOrderQueryable);
                var paginatedDataModel = paginatedData as RepositoryResponseWithModel<PaginatedResultModel<PendingOrderReportViewModel>>;
                paginatedDataModel.ReturnModel.Items.SetLabelAndIdentifier(search.TimePeriodGroupingType, true);
                return paginatedData;
            }
            catch (Exception ex)
            {
            }
            return new RepositoryResponse() { Status = System.Net.HttpStatusCode.BadRequest };
        }

        public async Task<CardViewModel> GetAverageOrderCompletionTime(PendingOrderReportSearchViewModel search)
        {
            var response = new CardViewModel("Average Order Completion Time", "#666666", "average-order.svg");
            try
            {
                var matchingWorkOrders = _db.WorkOrder.Where(x => x.ApprovalDate != null && x.ApprovalDate != new DateTime());
                double averageCompletionTime = 0;
                if (matchingWorkOrders.Any())
                {
                    averageCompletionTime = (double)await matchingWorkOrders
                        .Select(x => EF.Functions.DateDiffDay(x.CreatedOn, x.ApprovalDate))
                        .AverageAsync();
                }
                response.Value = $"{averageCompletionTime.RoundDouble(2)} days" ?? "-";
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        #region Asset

        public async Task<ChartResponseViewModel> GetAssetsMaintenanceDueChartData(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var fromDate = DateTime.Now;
                var toDate = fromDate.AddDays(90);
                var responseQueryable = _db.Assets
                  .Where(x => x.NextMaintenanceDate > fromDate && x.NextMaintenanceDate <= toDate)
                  .Select(x => new
                  {
                      AssetId = x.Id,
                      ReplacementCategory = x.NextMaintenanceDate < fromDate ? "Overdue" :
                                            x.NextMaintenanceDate <= fromDate.AddDays(30) ? "Due within 30 days" :
                                            x.NextMaintenanceDate <= fromDate.AddDays(90) ? "Due within 31-90 days" : "Unknown"
                  })
                  .GroupBy(x => x.ReplacementCategory)
                  .Select(g => new ChartViewModel
                  {
                      Category = g.Key,
                      Value = g.Count()
                  });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        public async Task<ChartResponseViewModel> GetAssetsReplacementDueChartData(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var fromDate = DateTime.Now;
                var toDate = fromDate.AddDays(90);
                var responseQueryable = _db.Assets
                  .Where(x => x.ReplacementDate > fromDate && x.ReplacementDate <= toDate)
                  .Select(x => new
                  {
                      AssetId = x.Id,
                      ReplacementCategory = x.ReplacementDate < fromDate ? "Overdue" :
                                            x.ReplacementDate <= fromDate.AddDays(30) ? "Due within 30 days" :
                                            x.ReplacementDate <= fromDate.AddDays(90) ? "Due within 31-90 days" : "Unknown"
                  })
                  .GroupBy(x => x.ReplacementCategory)
                  .Select(g => new ChartViewModel
                  {
                      Category = g.Key,
                      Value = g.Count()
                  });
                var response = await responseQueryable.ToListAsync();
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }


        #endregion Asset

        #region CostTracking

        public async Task<CardViewModel> GetAverageLaborCost(WorkOrderByManagerChartSearchViewModel search)
        {
            var response = new CardViewModel("Average Labor Cost", "#666666", "labor.svg");
            try
            {
                var averageCosts = await _db.WorkOrder.Select(x => x.LabourCost).AverageAsync();

                response.Value = averageCosts.RoundDouble(2).ToString("C") ?? "";
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        public async Task<CardViewModel> GetAverageEquipmentCost(WorkOrderByManagerChartSearchViewModel search)
        {
            var response = new CardViewModel("Average Equipment Cost", "#666666", "average-assestts.svg");
            try
            {
                var averageCosts = await _db.WorkOrder.Select(x => x.EquipmentCost).AverageAsync();

                response.Value = averageCosts.RoundDouble(2).ToString("C") ?? "";
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        public async Task<CardViewModel> GetAverageMaterialCost(WorkOrderByManagerChartSearchViewModel search)
        {
            var response = new CardViewModel("Average Asset Cost", "#666666", "average-equipment.svg");
            try
            {
                var averageCosts = await _db.WorkOrder.Select(x => x.MaterialCost).AverageAsync();

                response.Value = averageCosts.RoundDouble(2).ToString("C") ?? "";
            }
            catch (Exception ex)
            {
            }
            return response;
        }

        public async Task<ChartResponseViewModel> GetCostAccuracy(WorkOrderByManagerChartSearchViewModel search)
        {
            try
            {
                var averageCosts = await _db.WorkOrder.Include(x => x.TaskType)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    BudgetAmount = g.Average(x => (x.TaskType != null ? x.TaskType.BudgetCost : 0)),
                    FinalAmount = g.Average(x => x.ActualCost),
                })
                .FirstOrDefaultAsync();
                var response = new List<ChartViewModel> {
                    new ChartViewModel
                    {
                        Category = "Budget Amount",
                        Value = averageCosts?.BudgetAmount.RoundDouble(2)??0d
                    },
                    new ChartViewModel
                    {
                        Category = "Final Amount",
                        Value = averageCosts?.FinalAmount.RoundDouble(2)??0d
                    }
                };
                return SetResponseModel(response);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }

        #endregion CostTracking

        #region RawReport

        public async Task<WorkOrderRawReportColumns> GetWorkOrderRawReportColumnCount()
        {
            var equipmentColumns = await (from eq in _db.Equipments
                                          select new BaseMinimalVM
                                          {
                                              Id = eq.Id,
                                              Name = eq.Description ?? "-"
                                          }).ToListAsync();

            var materialColumns = await (from inv in _db.Inventories
                                         select new BaseMinimalVM
                                         {
                                             Id = inv.Id,
                                             Name = inv.Description
                                         }).ToListAsync();

            var technicianColumns = await (from wo in _db.WorkOrder
                                           join o in _db.WorkOrderTechnicians on wo.Id equals o.WorkOrderId
                                           group o.Technician by o.TechnicianId into g
                                           select new BaseMinimalVM
                                           {
                                               Id = g.Key,
                                               Name = g.Max(x => x.FirstName) + " " + g.Max(x => x.LastName)
                                           }).ToListAsync();

            var result = new WorkOrderRawReportColumns
            {
                MaterialColumns = materialColumns != null ? materialColumns : new(),
                EquipmentColumns = equipmentColumns != null ? equipmentColumns : new(),
                TechnicianColumns = technicianColumns != null ? technicianColumns : new()
            };

            return result;
        }

        public async Task<IRepositoryResponse> GetWorkOrderRawReportData(IBaseSearchModel filters)
        {
            try
            {
                var search = filters as WorkOrderSearchViewModel;
                IQueryable<WorkOrderRawReportViewModel> workOrderQueryable = GetWorkOrderRawReportQueryable(search);

                var result = await workOrderQueryable.Paginate(search);
                if (result != null)
                {
                    var materials = await (from o in _db.Orders
                                           join oi in _db.OrderItems on o.Id equals oi.OrderId
                                           join inv in _db.Inventories on oi.InventoryId equals inv.Id
                                           join t in _db.Transactions.Where(x => x.TransactionType == TransactionTypeCatalog.Order)
                                            on oi.Id equals t.EntityDetailId
                                           select new
                                           {
                                               WorkOrderId = o.WorkOrderId,
                                               InventoryId = inv.Id,
                                               InventoryName = inv.Description,
                                               Cost = t.ItemPrice * t.Quantity
                                           })
                                           .GroupBy(x => new { x.WorkOrderId, x.InventoryId, x.InventoryName })
                                           .Select(x => new WorkOrderInventoryRawData(
                                                x.Key.WorkOrderId,
                                                x.Key.InventoryName,
                                                x.Key.InventoryId,
                                                -1 * x.Sum(s => s.Cost)
                                           )).ToListAsync();

                    var equipments = await (from o in _db.Orders
                                            join oi in _db.OrderItems on o.Id equals oi.OrderId
                                            join eq in _db.Equipments on oi.EquipmentId equals eq.Id
                                            join t in _db.EquipmentTransactions.Where(x => x.TransactionType == EquipmentTransactionTypeCatalog.Return)
                                             on oi.Id equals t.EntityDetailId
                                            select new
                                            {
                                                WorkOrderId = o.WorkOrderId,
                                                EquipmentId = eq.Id,
                                                EquipmentName = eq.Description,
                                                Cost = t.Hours * t.Quantity * t.HourlyRate
                                            })
                                           .GroupBy(x => new { x.WorkOrderId, x.EquipmentId, x.EquipmentName })
                                           .Select(x => new WorkOrderEquipmentRawData(
                                                x.Key.WorkOrderId,
                                                x.Key.EquipmentName,
                                                x.Key.EquipmentId,
                                                x.Sum(s => s.Cost)
                                           )).ToListAsync();
                    var technicians = await (from wo in _db.WorkOrder
                                             join wot in _db.WorkOrderTechnicians.Include(x => x.Technician).Include(x => x.CraftSkill) on wo.Id equals wot.WorkOrderId
                                             join ts in _db.Timesheets on wot.Id equals ts.WorkOrderTechnicianId
                                             group new { wot.Technician, wot.CraftSkill, ts } by new { techId = wot.Technician.Id, woId = wo.Id } into wotg
                                             select new WorkOrderTechnicianRawData(
                                                 wotg.Key.woId,
                                                 wotg.Key.techId,
                                                 wotg.Max(x => x.Technician.FirstName) + " " + wotg.Max(x => x.Technician.LastName),
                                                 wotg.Max(x => x.CraftSkill.Name),
                                                 wotg.Sum(s => s.ts.TotalCost)
                                             )).ToListAsync();

                    foreach (var r in result.Items)
                    {
                        var woEquipments = equipments.Where(x => x.WorkOrderId == r.Id).ToList();
                        if (woEquipments != null && woEquipments.Count > 0)
                        {
                            r.Equipments = woEquipments.ToDictionary(
                               wo => wo.EquipmentId,
                               wo => wo
                            );
                        }
                        var woMaterials = materials.Where(x => x.WorkOrderId == r.Id).ToList();
                        if (woMaterials != null && woMaterials.Count > 0)
                        {
                            r.Materials = woMaterials.ToDictionary(
                               wo => wo.InventoryId,
                               wo => wo
                            );
                        }
                        var woTechnician = technicians.Where(x => x.WorkOrderId == r.Id).ToList();
                        if (woTechnician != null && woTechnician.Count > 0)
                        {
                            r.Technicians = woTechnician.ToDictionary(
                               wo => wo.TechnicianId,
                               wo => wo
                            );
                        }
                    }


                    var response = new RepositoryResponseWithModel<PaginatedResultModel<WorkOrderRawReportViewModel>> { ReturnModel = result };
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

        private IQueryable<WorkOrderRawReportViewModel> GetWorkOrderRawReportQueryable(WorkOrderSearchViewModel? search)
        {
            DateTime? FromDate = search?.FromDate;
            DateTime? ToDate = search?.ToDate;
            if (FromDate == null || FromDate == DateTime.MinValue)
            {
                FromDate = DateTime.Today.AddDays(-15);
            }
            if (ToDate == null || ToDate == DateTime.MinValue)
            {
                ToDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            }

            return (from w in _db.WorkOrder
                                      .Include(x => x.Asset).ThenInclude(x => x.AssetType)
                                      .Include(x => x.Technicians).ThenInclude(x => x.CraftSkill)
                                      .Include(x => x.Technicians).ThenInclude(x => x.Technician)
                                      .Include(x => x.TaskType)
                                      .Include(x => x.StreetServiceRequest)
                    join replace in _db.Replaces on w.ReplaceId equals replace.Id into wreplace
                    from replace in wreplace.DefaultIfEmpty()
                    join repair in _db.Repairs on w.RepairId equals repair.Id into wrepair
                    from repair in wrepair.DefaultIfEmpty()
                    join m in _db.Users on w.CreatedBy equals m.Id
                    where
                    (
                      (
                          string.IsNullOrEmpty(search.Search.value)
                          ||
                          w.Asset.SystemGeneratedId.ToLower().Contains(search.Search.value.ToLower())
                          ||
                          w.SystemGeneratedId.ToLower().Contains(search.Search.value.ToLower())
                          ||
                          w.Manager.FirstName.ToLower().Contains(search.Search.value.ToLower())
                      )
                      &&
                      (search.AssetId == null || w.Asset.SystemGeneratedId == search.AssetId)
                      &&
                      (search.Manager.Id == 0 || w.Manager.Id == search.Manager.Id)
                      &&
                      (search.AssetType.Id == null || w.Asset.AssetType.Id == search.AssetType.Id)
                      &&
                      (search.Task == null || w.Task == search.Task)
                      &&
                      (search.Urgency == null || w.Urgency == search.Urgency)
                      &&
                      (w.CreatedOn >= FromDate)
                      &&
                      (w.CreatedOn <= ToDate)
                    )
                    select new WorkOrderRawReportViewModel
                    {
                        Id = w.Id,
                        Status = w.Status,
                        SourceStatus = w.Asset == null ? WOSourceStatusCatalog.Public : WOSourceStatusCatalog.Internal,
                        CreatedOn = w.CreatedOn,
                        RequestorName = m.FirstName + " " + m.LastName,
                        Phone = w.StreetServiceRequest != null ? w.StreetServiceRequest.PhoneNumber : "-",
                        Address = w.StreetServiceRequest != null ? w.StreetServiceRequest.StreetAddress : "-",
                        City = w.StreetServiceRequest != null ? w.StreetServiceRequest.City : "-",
                        State = w.StreetServiceRequest != null ? w.StreetServiceRequest.State : "-",
                        ZipCode = w.StreetServiceRequest != null ? w.StreetServiceRequest.Zip : "-",
                        Email = w.StreetServiceRequest != null ? w.StreetServiceRequest.Email : "-",
                        EmailSubject = w.StreetServiceRequest != null ? w.StreetServiceRequest.Subject : "-",
                        TypeOfProblem = w.StreetServiceRequest != null ? w.StreetServiceRequest.Description : "-",
                        Intersection = w.Intersection,
                        Description = w.Description,
                        SystemGeneratedId = w.SystemGeneratedId,
                        Manager = m.FirstName + " " + m.LastName,
                        Urgency = w.Urgency,
                        Type = w.Type,
                        DueDate = w.DueDate,
                        Task = w.Task,
                        AssetId = w.Asset != null ? w.Asset.SystemGeneratedId : "-",
                        AssetType = w.AssetType != null ? w.AssetType.Name : "-",
                        ApprovalDate = w.ApprovalDate,
                        TotalHours = w.TotalHours,
                        TotalCost = w.TotalCost,
                        LabourCost = w.LabourCost,
                        MaterialCost = w.MaterialCost,
                        EquipmentCost = w.EquipmentCost,
                    }).AsQueryable();
        }

        public async Task<List<string>> GetAssetRawReportColumns()
        {
            var columns = await _db.AssetTypeLevels1.Select(x => x.Name).ToListAsync();
            return columns ?? new();
        }

        public async Task<IRepositoryResponse> GetAssetsRawReport(AssetSearchViewModel search)
        {
            try
            {
                var workOrders = await _db.WorkOrder
                    .Select(x => new
                    {
                        Task = x.Task,
                        ApprovalDate = x.ApprovalDate,
                        AssetId = x.AssetId
                    }).ToListAsync();

                var maintenanceTypeWO = workOrders.Where(x => x.Task == TaskCatalog.Maintenance).OrderByDescending(x => x.ApprovalDate).ToList();
                var repairTypeWO = workOrders.Where(x => x.Task == TaskCatalog.Repair).OrderByDescending(x => x.ApprovalDate).ToList();
                var replaceTypeWO = workOrders.Where(x => x.Task == TaskCatalog.Replace).OrderByDescending(x => x.ApprovalDate).ToList();
                var removeTypeWO = workOrders.Where(x => x.Task == TaskCatalog.Remove).OrderByDescending(x => x.ApprovalDate).ToList();

                var defaultDate = DateTime.MinValue;
                var assetsQueryable = (from a in _db.Assets
                                       join at in _db.AssetTypes on a.AssetTypeId equals at.Id
                                       join mutc in _db.MUTCDs on a.MUTCDId equals mutc.Id into mutcl
                                       from mutc in mutcl.DefaultIfEmpty()
                                       where (
                                                (
                                                    string.IsNullOrEmpty(search.Search.value)
                                                    ||
                                                    a.AssetType.Name.ToLower().Contains(search.Search.value.ToLower())
                                                    ||
                                                    a.MUTCD.Code.ToLower().Contains(search.Search.value.ToLower())
                                                    ||
                                                    a.SystemGeneratedId.ToLower().Contains(search.Search.value.ToLower())
                                                )
                                                &&
                                                (search.AssetType.Id == null || a.AssetTypeId == search.AssetType.Id)
                                                &&
                                                (search.MUTCD.Id == null || a.MUTCDId == search.MUTCD.Id)
                                       )
                                       select new AssetRawReportViewModel
                                       {
                                           Id = a.Id,
                                           SystemGeneratedId = a.SystemGeneratedId,
                                           AssetTypeName = at.Name,
                                           Description = a.Description ?? "-",
                                           MUTCDCode = mutc == null ? "" : mutc.Code,
                                           Value = a.Value ?? 0,
                                           ReplacementDate = a.ReplacementDate ?? defaultDate,
                                           NextMaintenanceDate = a.NextMaintenanceDate ?? defaultDate,
                                           DateAdded = a.CreatedOn,
                                       }).AsQueryable();
                var result = await assetsQueryable.Paginate(search);
                if (result != null)
                {
                    var assetTypeValues = await _db.AssetAssociations.Include(x => x.AssetTypeLevel1).Include(x => x.AssetTypeLevel2)
                        .Select(x => new AssetAssociationRawReportViewModel(
                            x.Id,
                            x.AssetId,
                            x.AssetTypeLevel1Id,
                            x.AssetTypeLevel1.Name,
                            x.AssetTypeLevel2Id,
                            x.AssetTypeLevel2.Name
                        ))
                        .ToListAsync();
                    foreach (var r in result.Items)
                    {
                        r.LastMaintenanceDate = maintenanceTypeWO.Where(x => x.AssetId == r.Id)
                                                .Skip(1)
                                                .Select(x => x.ApprovalDate)
                                                .FirstOrDefault() ?? defaultDate;
                        r.LastRepairDate = repairTypeWO.Where(x => x.AssetId == r.Id)
                                                .Skip(1)
                                                .Select(x => x.ApprovalDate)
                                                .FirstOrDefault() ?? defaultDate;
                        r.LastRemoveDate = removeTypeWO.Where(x => x.AssetId == r.Id)
                                                .Skip(1)
                                                .Select(x => x.ApprovalDate)
                                                .FirstOrDefault() ?? defaultDate;
                        r.LastReplacementDate = replaceTypeWO.Where(x => x.AssetId == r.Id)
                                                .Skip(1)
                                                .Select(x => x.ApprovalDate)
                                                .FirstOrDefault() ?? defaultDate;
                        var currentAssetTypeValues = assetTypeValues.Where(x => x.AssetId == r.Id).ToList();
                        if (currentAssetTypeValues != null && currentAssetTypeValues.Count > 0)
                        {
                            foreach (var ta in currentAssetTypeValues)
                            {
                                r.Associations.Add(ta.AssetLevel1Name, ta.AssetLevel2Name);
                            }
                            //SetAssetAssociations(r, currentAssetTypeValues, ta);
                        }
                    }
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<AssetRawReportViewModel>> { ReturnModel = result };
                    return response;
                }
                _logger.LogWarning($"No record found for {typeof(Asset).FullName} in GetAssetsRawReport()");
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                return Response.BadRequestResponse(_response);
            }
        }

        #endregion RawReport

        public async Task<IRepositoryResponse> GetTransactionReport(TransactionSearchViewModel model)
        {
            try
            {
                PaginatedResultModel<TransactionReportViewModel> result = new();

                var transactions = new List<TransactionReportViewModel>();
                //Work Order, Funding Source, Item, Manufacturer, MUTCD, UOM, PO No, Rate, Quantity, Location, Supplier, Source, material details, equipment details
                var inventoryTransactions = await (from t in _db.Transactions
                                                   join src in _db.Sources on t.SourceId equals src.Id into srcl
                                                   from src in srcl.DefaultIfEmpty()
                                                   join mt in _db.Inventories on t.InventoryId equals mt.Id
                                                   join mf in _db.Manufacturers on mt.ManufacturerId equals mf.Id
                                                   join uom in _db.UOMs on mt.UOMId equals uom.Id
                                                   join mutcd in _db.MUTCDs on mt.MUTCDId equals mutcd.Id
                                                   join sup in _db.Suppliers on t.SupplierId equals sup.Id
                                                   join o in _db.Orders on t.EntityId equals o.Id
                                                   join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                                   join loc in _db.Locations on t.LocationId equals loc.Id
                                                   join u in _db.Users on t.CreatedBy equals u.Id
                                                   where (
                                                               (
                                                                   string.IsNullOrEmpty(model.Search.value)
                                                                   ||
                                                                   sup.Name.ToLower().Contains(model.Search.value.ToLower())
                                                                   ||
                                                                   wo.SystemGeneratedId.ToLower().Contains(model.Search.value.ToLower())
                                                               )
                                                               &&
                                                               (model.Supplier.Id == null || sup.Id == model.Supplier.Id)
                                                               &&
                                                               (model.UOM.Id == null || uom.Id == model.UOM.Id)
                                                               &&
                                                               (model.Location.Id == null || loc.Id == model.Location.Id)
                                                      )
                                                   select new TransactionReportViewModel
                                                   {
                                                       Id = t.Id,
                                                       TransactionType = t.TransactionType.GetDisplayName(),
                                                       PONo = t.LotNO,
                                                       Quantity = Math.Abs(t.Quantity),
                                                       CreatedBy = u.FirstName + " " + u.LastName,
                                                       WorkOrderNumber = wo.SystemGeneratedId,
                                                       FundingSource = src != null ? src.Name : "-",
                                                       Material = mt.Description,
                                                       ItemNo = mt.ItemNo,
                                                       ItemPrice = t.ItemPrice,
                                                       Location = loc.Name,
                                                       Manufacturer = mf.Name,
                                                       MUTCD = mutcd.Description,
                                                       Rate = Math.Abs(t.Quantity * t.ItemPrice),
                                                       Supplier = sup.Name,
                                                       //Source = src.Name,
                                                       UOM = uom.Name,
                                                       //PurchaseDate = 
                                                       CreatedOn = t.CreatedOn,
                                                       Type = "Material"
                                                   })
                              .OrderByDescending(x => x.CreatedOn)
                              .ToListAsync();

                var equipmentTransactions = await (from t in _db.EquipmentTransactions
                                                   join eq in _db.Equipments on t.EquipmentId equals eq.Id
                                                   join mf in _db.Manufacturers on eq.ManufacturerId equals mf.Id
                                                   join uom in _db.UOMs on eq.UOMId equals uom.Id
                                                   join sup in _db.Suppliers on t.SupplierId equals sup.Id
                                                   join o in _db.Orders on t.EntityId equals o.Id
                                                   join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                                   join loc in _db.Locations on t.LocationId equals loc.Id
                                                   join u in _db.Users on t.CreatedBy equals u.Id
                                                   where (
                                                              (
                                                                  string.IsNullOrEmpty(model.Search.value)
                                                                  ||
                                                                  sup.Name.ToLower().Contains(model.Search.value.ToLower())
                                                                  ||
                                                                  wo.SystemGeneratedId.ToLower().Contains(model.Search.value.ToLower())
                                                              )
                                                              &&
                                                              (model.Supplier.Id == null || sup.Id == model.Supplier.Id)
                                                               &&
                                                              (model.UOM.Id == null || uom.Id == model.UOM.Id)
                                                              &&
                                                               (model.Location.Id == null || loc.Id == model.Location.Id)
                                                     )
                                                   select new TransactionReportViewModel
                                                   {
                                                       Id = t.Id,
                                                       TransactionType = t.TransactionType.GetDisplayName(),
                                                       PONo = t.PoNo,
                                                       Quantity = Math.Abs(t.Quantity),
                                                       CreatedBy = u.FirstName + " " + u.LastName,
                                                       WorkOrderNumber = wo.SystemGeneratedId,
                                                       FundingSource = "-",
                                                       Material = eq.Description,
                                                       ItemNo = eq.ItemNo,
                                                       ItemPrice = t.ItemPrice,
                                                       Location = loc.Name,
                                                       Manufacturer = mf.Name,
                                                       MUTCD = "-",
                                                       Rate = Math.Abs(t.Quantity * t.HourlyRate * t.Hours),
                                                       Supplier = sup.Name,
                                                       //Source = src.Name,
                                                       UOM = uom.Name,
                                                       //PurchaseDate = 
                                                       CreatedOn = t.CreatedOn,
                                                       Type = "Equipment"
                                                   })
                           .OrderByDescending(x => x.CreatedOn)
                           .ToListAsync();

                transactions.AddRange(inventoryTransactions);
                transactions.AddRange(equipmentTransactions);

                if (transactions != null && transactions?.Count > 0)
                {
                    result.Items = transactions.OrderBy(x => x.CreatedOn).ToList();
                }

                var response = new RepositoryResponseWithModel<PaginatedResultModel<TransactionReportViewModel>> { ReturnModel = result };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Inventory).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<IRepositoryResponse> GetTimesheetReport(IBaseSearchModel search)
        {
            try
            {
                var searchFilter = search as TimeSheetBreakdownReportSearchViewModel;
                DateTime? FromDate = searchFilter?.From;
                DateTime? ToDate = searchFilter?.To;
                if (FromDate == null || FromDate == DateTime.MinValue)
                {
                    FromDate = DateTime.Today.AddDays(-15);
                }
                if (ToDate == null || ToDate == DateTime.MinValue)
                {
                    ToDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                }

                var timesheetBreakdownsQueryable = _db.TimesheetBreakdowns
                                     .Include(x => x.Timesheet).ThenInclude(x => x.WorkOrderTechnician).ThenInclude(x => x.WorkOrder)
                                     .Include(x => x.Timesheet).ThenInclude(x => x.WorkOrderTechnician).ThenInclude(x => x.CraftSkill)
                                     .Include(x => x.Timesheet).ThenInclude(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                                     .Include(x => x.Timesheet).ThenInclude(x => x.Approver)
                                     .Where(x =>
                                            (
                                              string.IsNullOrEmpty(searchFilter.Search.value)
                                              || x.Timesheet.WorkOrderTechnician.Technician.FirstName.ToLower().Contains(searchFilter.Search.value.ToLower())
                                              || x.Timesheet.WorkOrderTechnician.WorkOrder.SystemGeneratedId.ToLower().Contains(searchFilter.Search.value.ToLower())
                                              || x.Timesheet.Approver.FirstName.ToLower().Contains(searchFilter.Search.value.ToLower())
                                              || x.Timesheet.Approver.LastName.ToLower().Contains(searchFilter.Search.value.ToLower())
                                            //|| x.Timesheet.InvoiceNo.ToLower().Contains(searchFilter.Search.value.ToLower())
                                            //|| x.Timesheet.WorkOrderTechnician.Contract.Department.ToLower().Contains(searchFilter.Search.value.ToLower())
                                            )
                                            &&
                                            (searchFilter.Approver.Id == null || searchFilter.Approver.Id == 0 || x.Timesheet.Approver.Id == searchFilter.Approver.Id)
                                            &&
                                            (searchFilter.Technician.Id == 0 || x.Timesheet.WorkOrderTechnician.Technician.Id == searchFilter.Technician.Id)
                                            &&
                                            (searchFilter.Craft.Id == null || searchFilter.Craft.Id == 0 || x.Timesheet.WorkOrderTechnician.CraftSkillId == searchFilter.Craft.Id)
                                            &&
                                            (searchFilter.WorkOrder.Id == null || searchFilter.WorkOrder.Id == 0 || x.Timesheet.WorkOrderTechnician.WorkOrderId == searchFilter.WorkOrder.Id)
                                            &&
                                            (searchFilter.Day == null || x.Date.Day == searchFilter.Day)
                                            &&
                                            (x.Date >= FromDate)
                                            &&
                                            (x.Date <= ToDate)
                                          )
                                     .OrderByDescending(x => x.Date).AsQueryable();
                var timesheetBreakdowns = await timesheetBreakdownsQueryable.Paginate(search);

                if (timesheetBreakdowns != null)
                {
                    List<TimesheetBreakdownDetailForReportViewModel> timesheetBreakdownList = new List<TimesheetBreakdownDetailForReportViewModel>();
                    foreach (var item in timesheetBreakdowns.Items.ToList())
                    {
                        var mappedTimesheetBreakdownModel = _mapper.Map<TimesheetBreakdownDetailForReportViewModel>(item);
                        mappedTimesheetBreakdownModel.Timesheet.Technician = new TechnicianDetailViewModel { FirstName = item.Timesheet?.WorkOrderTechnician.Technician.FirstName, LastName = item.Timesheet?.WorkOrderTechnician.Technician.LastName };
                        mappedTimesheetBreakdownModel.Timesheet.Approver = item.Timesheet?.Approver != null ?
                                    new ManagerDetailViewModel { FirstName = item.Timesheet?.Approver.FirstName ?? "", LastName = item.Timesheet?.Approver.LastName ?? "" } : new();
                        mappedTimesheetBreakdownModel.Timesheet.WorkOrder.SystemGeneratedId = item.Timesheet?.WorkOrderTechnician.WorkOrder.SystemGeneratedId;
                        //mappedTimesheetBreakdownModel.Timesheet.Contract.JobNumber = item.Timesheet.WorkOrderTechnician.ContractPOItem.JobNumber;
                        //mappedTimesheetBreakdownModel.Timesheet.Contract.Department = item.Timesheet.WorkOrderTechnician.Contract.Department;
                        mappedTimesheetBreakdownModel.Timesheet.CraftSkill.Id = item.Timesheet?.WorkOrderTechnician.CraftSkillId;
                        mappedTimesheetBreakdownModel.Timesheet.CraftSkill.Name = item.Timesheet?.WorkOrderTechnician.CraftSkill?.Name ?? "";
                        mappedTimesheetBreakdownModel.Timesheet.CraftSkill.STRate = item.Timesheet?.WorkOrderTechnician.CraftSkill?.STRate ?? 0;
                        mappedTimesheetBreakdownModel.Timesheet.CraftSkill.OTRate = item.Timesheet?.WorkOrderTechnician.CraftSkill?.OTRate ?? 0;
                        mappedTimesheetBreakdownModel.Timesheet.CraftSkill.DTRate = item.Timesheet?.WorkOrderTechnician.CraftSkill?.DTRate ?? 0;
                        //var employeeCite = item.Timesheet.WorkOrderTechnician.Employee.City + " / " + item.Timesheet.WorkOrderTechnician.Employee.State;
                        //var customerCite = item.Timesheet.WorkOrderTechnician.Contract.Customer.PhysicalAddressCity + " / " + item.Timesheet.WorkOrderTechnician.Contract.Customer.PhysicalAddressState;
                        //mappedTimesheetBreakdownModel.LocationSite = item.IsOnSite ? customerCite : employeeCite;
                        mappedTimesheetBreakdownModel.RegularHours = item.RegularHours;
                        mappedTimesheetBreakdownModel.OvertimeHours = item.OvertimeHours;
                        mappedTimesheetBreakdownModel.DoubleTimeHours = item.DoubleTimeHours;
                        //mappedTimesheetBreakdownModel.TSRefNumber = item.TSRefNumber;
                        //mappedTimesheetBreakdownModel.PaymentIndicator = item.PaymentIndicator;
                        //mappedTimesheetBreakdownModel.TSRefStatus = item.TSRefStatus;
                        //mappedTimesheetBreakdownModel.Timesheet.PerDiem = (item.Timesheet.DailyPerDiem * (item.IncludePerDiem == true ? 1 : 0)).RoundFloat();
                        //mappedTimesheetBreakdownModel.Timesheet.Equipment = (item.Timesheet.Equipment / 7).RoundFloat();
                        //mappedTimesheetBreakdownModel.Timesheet.Other = (item.Timesheet.Other / 7).RoundFloat();
                        //mappedTimesheetBreakdownModel.Timesheet.InvoiceNo = item.Timesheet.InvoiceNo;
                        //mappedTimesheetBreakdownModel.POLineItem = item.Timesheet.WorkOrderTechnician.ContractPOItem?.POLineItem ?? "";
                        timesheetBreakdownList.Add(mappedTimesheetBreakdownModel);
                    }

                    var paginatedResult = new PaginatedResultModel<TimesheetBreakdownDetailForReportViewModel>();
                    paginatedResult.Items = timesheetBreakdownList;
                    paginatedResult._meta = timesheetBreakdowns._meta;
                    paginatedResult._links = timesheetBreakdowns._links;

                    var response = new RepositoryResponseWithModel<PaginatedResultModel<TimesheetBreakdownDetailForReportViewModel>>();
                    response.ReturnModel = paginatedResult;
                    return response;
                }
                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetById() for {typeof(Inventory).FullName} threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        #region[Helpers]

        private static void SetAssetAssociations(AssetRawReportViewModel? r, List<AssetAssociationRawReportViewModel> currentAssetTypeValues, string ta)
        {
            switch (ta)
            {
                case "Sign Type":
                    r.SignType = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                case "Post Type":
                    r.PostType = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                case "Post Location Type":
                    r.PostLocationType = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                case "Mount Type":
                    r.MountType = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                case "Hardware":
                    r.Hardware = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                case "Dimension":
                    r.Dimension = GetValueForLevel2(r, currentAssetTypeValues, ta);
                    break;
                default:
                    break;
            }

            GetValueForLevel2(r, currentAssetTypeValues, ta);
        }

        private static string GetValueForLevel2(AssetRawReportViewModel? r, List<AssetAssociationRawReportViewModel> currentAssetTypeValues, string ta)
        {
            if (currentAssetTypeValues.Any(x => x.AssetLevel1Name == ta))
            {
                return currentAssetTypeValues.Where(x => x.AssetLevel1Name == ta).Select(x => x.AssetLevel2Name).FirstOrDefault() ?? "-";
            }
            return "-";
        }

        protected async Task<IRepositoryResponse> GetPaginatedResult<M>(IBaseSearchModel search, IQueryable<M> query)
        {
            var result = await query.Paginate(search);
            if (result != null)
            {
                var paginatedResult = new PaginatedResultModel<M>();
                paginatedResult.Items = result.Items;
                paginatedResult._meta = result._meta;
                paginatedResult._links = result._links;
                var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                return response;
            }
            return Response.NotFoundResponse(_response);
        }

        private ChartResponseViewModel SetResponseModel<T>(List<T> data) where T : IChartDataViewModel
        {
            List<IChartDataViewModel> convertedData = data.Cast<IChartDataViewModel>().ToList();
            return new ChartResponseViewModel { IsSuccess = true, Data = convertedData };
        }

        #endregion

    }

}

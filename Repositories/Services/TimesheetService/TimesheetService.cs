using AutoMapper;
using DataLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Repositories.Common;
using ViewModels.Timesheet;
using ViewModels.Employee;
using ViewModels.Timesheet.Interfaces;
using Enums;
using Centangle.Common.ResponseHelpers.Models;
using ViewModels.Manager;
using ViewModels;
using ViewModels.Technician;
using Helpers.Extensions;
using Pagination;
using Centangle.Common.ResponseHelpers;
using ClosedXML.Excel;
using System.Linq.Dynamic.Core;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.WorkOrder;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Repositories
{
    public class TimesheetService : BaseService<Timesheet, TimesheetModifyViewModel, TimesheetModifyViewModel, TimesheetDetailViewModel>, ITimesheetService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TimesheetService> _logger;
        private readonly IMapper _mapper;
        private readonly ITimesheetLimit _timesheetLimit;
        private readonly IRepositoryResponse _response;
        private readonly IUserInfoService _userInfoService;

        public TimesheetService(
            ApplicationDbContext db
            , ILogger<TimesheetService> logger
            , IMapper mapper
            , ITimesheetLimit timesheetLimit
            , IRepositoryResponse response
            , IUserInfoService userInfoService
            //, IExcelReader reader
            ) : base(db, logger, mapper, response)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _timesheetLimit = timesheetLimit;
            _response = response;
            _userInfoService = userInfoService;
        }
        public async Task<TimesheetProjectsViewModel> GetWorkOrdersByTechnicianId(long id)
        {
            try
            {
                var workOrders = await _db.WorkOrderTechnicians
                                   .Include(x => x.WorkOrder).ThenInclude(x => x.AssetType)
                                   .Include(x => x.CraftSkill)
                                   .Where(x =>
                                   x.TechnicianId == id
                                   && x.WorkOrder.ActiveStatus == ActiveStatus.Active
                                   && x.ActiveStatus == ActiveStatus.Active
                                   ).ToListAsync();

                if (workOrders != null)
                {
                    TimesheetProjectsViewModel timesheetProjects = new()
                    {
                        TechnicianId = id
                    };

                    var groupedData = workOrders.GroupBy(x => x.WorkOrderId).ToList();
                    foreach (var workOrder in groupedData)
                    {
                        WorkOrderViewModel workOrderViewModel = new()
                        {
                            WorkOrder = new TimesheetWorkOrderDetailAPIViewModel
                            {
                                Id = workOrder.Key,
                                SystemGeneratedId = workOrder.Max(x => x.WorkOrder.SystemGeneratedId),
                                AssetType = new ViewModels.Shared.BaseMinimalVM
                                {
                                    Id = workOrder.Max(x => x.WorkOrder?.AssetType?.Id) ?? 0,
                                    Name = workOrder.Max(x => x.WorkOrder?.AssetType?.Name) ?? "-"
                                },
                                Type = workOrder.Max(x => x.WorkOrder.Type),
                                Task = workOrder.Max(x => x.WorkOrder.Task),
                                DueDate = workOrder.Max(x => x.WorkOrder.DueDate)
                            },
                            Craft = workOrder.Select(x => new TechnicianWorkOrderCraft
                            {
                                Id = x.CraftSkill.Id,
                                Name = x.CraftSkill.Name,
                                WorkOrderTechnician = workOrder.Where(m => m.WorkOrderId == workOrder.Key && m.CraftSkillId == x.CraftSkillId).Select(x => x.Id).FirstOrDefault()
                            }).ToList()
                        };
                        timesheetProjects.TechniciansWorkOrders.Add(workOrderViewModel);
                    }
                    return timesheetProjects;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetProjectsByEmployeeId method for Approval threw an exception.");
            }
            return null;
        }

        public async Task<long> ModifyApiTimeSheetBreakdowns(TimesheetBreakdownUpdateViewModel model)
        {
            try
            {
                var timesheetBreakdowns = await _db.TimesheetBreakdowns.Where(x => x.TimesheetId == model.TimeSheetId).ToListAsync();
                if (timesheetBreakdowns != null && timesheetBreakdowns.Count > 0)
                {
                    await SetDisableDaysDefaultSettings(model.TimeSheetId, model.TimesheetBreakdowns.ToList<ITimesheetBreakdownModel>());
                    foreach (var timesheet in timesheetBreakdowns)
                    {
                        var timesheetVM = model.TimesheetBreakdowns.Where(x => x.Id == timesheet.Id).FirstOrDefault();
                        if (timesheetVM != null)
                        {
                            var timesheetHoursBreakdown = _timesheetLimit.GetHoursBreakdown(timesheetVM.Day, timesheetVM.TotalHours);
                            timesheet.Id = timesheetVM.Id;
                            timesheet.Date = timesheetVM.Date;
                            timesheet.Day = timesheetVM.Day;
                            var doubletimeHrs = timesheetHoursBreakdown.DTHours;
                            var overtimeHrs = timesheetHoursBreakdown.OTHours;
                            var regularHours = timesheetHoursBreakdown.STHours;
                            timesheet.RegularHours = regularHours;
                            timesheet.DoubleTimeHours = doubletimeHrs;
                            timesheet.OvertimeHours = overtimeHrs;
                            timesheet.TimesheetId = model.TimeSheetId;
                            timesheet.IsOnSite = timesheetVM.IsOnSite;
                        }
                    }

                    await _db.SaveChangesAsync();
                    return model.TimeSheetId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ApproveTimesheets method for Approval threw an exception.");
            }
            return 0;
        }

        public async Task<TimeSheetWithBreakdownViewModel> GetTimeSheetBreakdowns(long id)
        {
            try
            {
                var result = await _db.Timesheets
                                                .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                                                .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.CraftSkill)
                                                .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.WorkOrder)
                                                .Include(x => x.TimesheetBreakdowns)
                                                .Include(x => x.Approver)
                                                .Where(x => x.Id == id && x.IsDeleted == false)
                                                .IgnoreQueryFilters()
                                                .FirstOrDefaultAsync();
                if (result != null)
                {
                    var mappedModel = _mapper.Map<TimeSheetWithBreakdownViewModel>(result);
                    mappedModel.Approver = _mapper.Map<ManagerDetailViewModel>(result.Approver);
                    mappedModel.WorkOrder = _mapper.Map<WorkOrderDetailViewModel>(result.WorkOrderTechnician.WorkOrder);
                    mappedModel.Craft = _mapper.Map<CraftSkillDetailViewModel>(result.WorkOrderTechnician.CraftSkill);
                    mappedModel.Technician = _mapper.Map<TechnicianDetailViewModel>(result.WorkOrderTechnician.Technician);
                    mappedModel.TimesheetBreakdowns = _mapper.Map<List<TimesheetBreakdownDetailViewModel>>(result.TimesheetBreakdowns);
                    //mappedModel.TimesheetBreakdowns.ForEach(x => x.DailyPerDiem = mappedModel.Employee.PerDiem);
                    await SetDisableDaysDefaultSettings(result.Id, mappedModel.TimesheetBreakdowns.ToList<ITimesheetBreakdownModel>());
                    await GetTimesheetUpdatedBy(result, mappedModel);
                    return mappedModel;
                }
            }
            catch (Exception ex)
            {

            }
            return new TimeSheetWithBreakdownViewModel();
        }

        public override async Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel search)
        {
            try
            {
                DateTime? date = null;
                var searchFilter = search as ApprovalSearchViewModel;
                if (searchFilter?.WeekEnding != null)
                {
                    DayOfWeek? dayOfWeek = searchFilter.WeekEnding?.DayOfWeek;
                    if (dayOfWeek != DayOfWeek.Sunday)
                    {
                        searchFilter.WeekEnding = await SetWeekending.SetWeekEnding(searchFilter.WeekEnding ?? DateTime.Now);
                    }
                    date = searchFilter.WeekEnding?.Date ?? DateTime.Now;
                }

                var result = await _db.Timesheets
                                        .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                                        .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.WorkOrder)
                                        .Include(x => x.TimesheetBreakdowns)
                                        .Where
                                            (x =>
                                                (date == null || x.WeekEnding.Date == date)
                                                &&
                                                (string.IsNullOrEmpty(searchFilter.PayPeriodRange) || x.TimesheetBreakdowns.Any(t => t.Date >= searchFilter.PayPeriodFromDate) && x.TimesheetBreakdowns.Any(t => t.Date <= searchFilter.PayPeriodToDate))
                                                &&
                                                (searchFilter.TimesheetId == null || searchFilter.TimesheetId == 0 || x.Id == searchFilter.TimesheetId)
                                                &&
                                                (searchFilter.Technician.Id == 0 || x.WorkOrderTechnician.TechnicianId == searchFilter.Technician.Id)
                                                &&
                                                (searchFilter.WorkOrder.Id == null || x.WorkOrderTechnician.WorkOrderId == searchFilter.WorkOrder.Id)
                                                 &&
                                                (searchFilter.ApproveStatus == null || x.ApproveStatus == searchFilter.ApproveStatus)
                                                &&
                                                (
                                                    string.IsNullOrEmpty(searchFilter.Search.value)
                                                    ||
                                                    x.WorkOrderTechnician.Technician.FirstName.ToLower().Contains(searchFilter.Search.value.ToLower())
                                                )
                                            )
                                            .Select(x => new TimesheetDetailViewModel
                                            {
                                                Id = x.Id,
                                                ActiveStatus = x.ActiveStatus,
                                                ApproveStatus = x.ApproveStatus,
                                                WorkOrderTechnicianId = x.WorkOrderTechnician.Id,
                                                WeekEnding = x.WeekEnding,
                                                Approver = x.Approver != null ? new BaseBriefVM { Id = x.Approver.Id, Name = x.Approver.FirstName } : new(),
                                                Technician = new EmployeeBriefViewModel
                                                {
                                                    Id = x.WorkOrderTechnician.Technician.Id,
                                                    Name = x.WorkOrderTechnician.Technician.FirstName ?? "" + " " + x.WorkOrderTechnician.Technician.LastName ?? "",
                                                },
                                                WorkOrder = new BaseBriefVM { Id = x.WorkOrderTechnician.WorkOrder.Id, Name = x.WorkOrderTechnician.WorkOrder.SystemGeneratedId },
                                                STRate = x.STRate,
                                                OTRate = x.OTRate,
                                                DTRate = x.DTRate,
                                                Miles = x.Miles,
                                                Airfare = x.Airfare,
                                                Other = x.Other,
                                                Material = x.Material,
                                                TotalPerDiem = x.PerDiem,
                                                DailyPerDiem = x.DailyPerDiem,
                                                PaymentIndicator = x.PaymentIndicator,
                                                TimesheetBreakdowns = x.TimesheetBreakdowns.Select(t => new TimesheetBreakdownDetailViewModel
                                                {
                                                    Id = t.Id,
                                                    Date = t.Date,
                                                    Day = t.Day,
                                                    //DailyPerDiem = t.DailyPerDiem,
                                                    DoubleTimeHours = t.DoubleTimeHours,
                                                    OvertimeHours = t.OvertimeHours,
                                                    RegularHours = t.RegularHours,
                                                    TimesheetId = t.TimesheetId,
                                                    //PaymentIndicator = t.PaymentIndicator
                                                }).ToList()

                                            })
                                            .Paginate(search);

                if (result != null)
                {
                    result.Items.ForEach(x => x.ProcessBreakdowns(x));
                    var paginatedResult = new PaginatedResultModel<M>();

                    paginatedResult.Items = result.Items as List<M>;

                    paginatedResult._meta = result._meta;
                    paginatedResult._links = result._links;
                    var response = new RepositoryResponseWithModel<PaginatedResultModel<M>> { ReturnModel = paginatedResult };
                    return response;
                }
                _logger.LogWarning($"No record found for Timesheet in GetAll()");
                return Response.NotFoundResponse(_response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAll() method for Approval threw an exception.");
                return Response.BadRequestResponse(_response);
            }
        }

        private async Task GetTimesheetUpdatedBy(Timesheet model, TimeSheetWithBreakdownViewModel viewModel)
        {
            if (model.UpdatedBy > 0)
            {
                var userName = await _db.Users.Where(x => x.Id == model.UpdatedBy).Select(x => x.UserName).FirstOrDefaultAsync();
                viewModel.LastUpdatedBy = userName;
                viewModel.LastUpdatedOn = model.UpdatedOn;
            }
        }

        private async Task SetDisableDaysDefaultSettings(long timeSheetId, List<ITimesheetBreakdownModel> timesheetBreakdowns)
        {
            var timeSheetModel = await _db.Timesheets.Include(x => x.WorkOrderTechnician).Where(x => x.Id == timeSheetId).FirstOrDefaultAsync();
            if (timeSheetModel != null)
            {
                var otherTimeSheetsForWeekQueryable = _db.Timesheets.Include(x => x.WorkOrderTechnician)
            .ThenInclude(x => x.Technician)
            .Where(x => x.WeekEnding == timeSheetModel.WeekEnding
             && x.WorkOrderTechnician.TechnicianId == timeSheetModel.WorkOrderTechnician.TechnicianId
             && x.WorkOrderTechnician.IsDeleted == false
             && x.Id != timeSheetModel.Id)
            .SelectMany(x => x.TimesheetBreakdowns).AsQueryable();

                var otherTimeSheetsForWeek = await otherTimeSheetsForWeekQueryable.ToListAsync();

                foreach (var timeSheetBreakDown in timesheetBreakdowns)
                {
                    var alreadyAddedDay = otherTimeSheetsForWeek?.Where(x => x.Day == timeSheetBreakDown.Day && (x.RegularHours > 0 || x.OvertimeHours > 0 || x.DoubleTimeHours > 0)).FirstOrDefault();
                    if (alreadyAddedDay != null)
                    {
                        timeSheetBreakDown.DisableDayEntry = false;// JASON Suggested to remove this feature
                    }
                    //if (otherTimeSheetsForWeek.Any(x => x.Day == timeSheetBreakDown.Day && x.IncludePerDiem))
                    //{
                    //    timeSheetBreakDown.DisableDayPerDiemEntry = true;
                    //}
                }
            }

        }

        private async Task<List<TimesheetBreakdown>?> SetTimesheetBreakdowns(List<TimesheetBreakdown>? timesheetBreakdowns, DateTime weekEnding)
        {
            try
            {

                for (int i = 6; i >= 0; i--)
                {
                    TimesheetBreakdown timesheetBreakdown = new()
                    {
                        Date = weekEnding.AddDays(-i),
                        Day = weekEnding.AddDays(-i).DayOfWeek,
                        IncludePerDiem = false,
                        RegularHours = 0,
                        OvertimeHours = 0,
                        DoubleTimeHours = 0,
                        PaymentIndicator = null,
                        TSRefStatus = null,


                    };
                    timesheetBreakdowns.Add(timesheetBreakdown);
                }
                return timesheetBreakdowns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Create method of Approval ");
            }
            return null;
        }

        public async Task ApproveTimesheets(List<long> ids, bool Status)
        {
            try
            {
                var timeSheets = await _db.Timesheets.Where(x => ids.Contains(x.Id)).ToListAsync();
                if (timeSheets != null && timeSheets.Count > 0)
                {
                    if (Status)
                    {
                        timeSheets.ForEach(x => x.ApproveStatus = TimesheetApproveStatus.Approved);
                    }
                    else
                    {
                        timeSheets.ForEach(x => x.ApproveStatus = TimesheetApproveStatus.Rejected);
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ApproveTimesheets method for Approval threw an exception.");
            }
        }

        public async Task<List<long>> GetApprovedTimesheetIds()
        {
            try
            {
                var timeSheetIds = await _db.Timesheets.Where(x => x.ApproveStatus == TimesheetApproveStatus.Approved).Select(x => x.Id).ToListAsync();
                if (timeSheetIds != null && timeSheetIds.Count > 0)
                {
                    return timeSheetIds;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ApproveTimesheets method for Approval threw an exception.");
            }
            return new List<long>();
        }

        public async Task UpdateTimeSheetBreakdowns(TimeSheetWithBreakdownViewModel model)
        {
            try
            {
                var timesheet = await _db.Timesheets.Include(x => x.TimesheetBreakdowns).Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (timesheet != null)
                {
                    await SetDisableDaysDefaultSettings(timesheet.Id, model.TimesheetBreakdowns.ToList<ITimesheetBreakdownModel>());
                    var mappedModel = _mapper.Map(model, timesheet);
                    if (timesheet.ApproveStatus == TimesheetApproveStatus.Approved)
                    {
                        timesheet.TotalCost = model.TotalCost;
                        var workOrder = await _db.WorkOrder.Where(x => x.Id == timesheet.WorkOrderId).IgnoreAutoIncludes().FirstOrDefaultAsync();
                        if (workOrder != null)
                        {
                            var totalHours = model.TimesheetBreakdowns.Sum(x => x.RegularHours) + model.TimesheetBreakdowns.Sum(x => x.DoubleTimeHours) + model.TimesheetBreakdowns.Sum(x => x.OvertimeHours);
                            workOrder.ActualHours += totalHours;
                            workOrder.LabourCost += model.TotalCost;
                            workOrder.ActualCost += model.TotalCost;
                            timesheet.ApproverId = long.Parse(_userInfoService.LoggedInUserId());
                        }
                    }
                    mappedModel.TimesheetBreakdowns?.ForEach(x => x.CreatedOn = timesheet.CreatedOn);
                    //mappedModel.PerDiem = model.TimesheetBreakdowns.Where(x => x.IncludePerDiem == true).Sum(x => x.DailyPerDiem);
                    _db.Entry(mappedModel).State = EntityState.Modified;
                    await _db.SaveChangesAsync(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ApproveTimesheets method for Approval threw an exception.");
            }
        }

        public async Task<TimesheetBriefViewModel> GetTimesheet(TimesheetEmployeeSearchViewModel model)
        {
            try
            {
                Timesheet? timesheet = (await GetExistingTimesheet(model));
                if (timesheet != null)
                {
                    var mappedTimesheet = await MapTimesheetVM(timesheet);
                    return mappedTimesheet;
                }
                else
                {
                    var workOrderTechnician = await _db.WorkOrderTechnicians.Include(x => x.WorkOrder).Include(x => x.CraftSkill)
                        .Where(x => x.Id == model.WorkOrderTechnicianId).FirstOrDefaultAsync();
                    if (workOrderTechnician != null)
                    {
                        TimesheetModifyViewModel timesheetModifyViewModel = new()
                        {
                            WeekEnding = model.WeekEnding,
                            CraftId = workOrderTechnician.CraftSkillId,
                            WorkOrderTechnicianId = model.WorkOrderTechnicianId,
                            WorkOrder = new WorkOrderBriefViewModel { Id = workOrderTechnician.Id },
                            Technician = new TechnicianBriefViewModel { Id = workOrderTechnician.TechnicianId, Name = "" },
                        };
                        timesheetModifyViewModel.Approver = null;
                        var timeSheetId = await CreateTimeSheet(timesheetModifyViewModel);
                        if (timeSheetId > 0)
                        {
                            var timesheetDBModel = await GetExistingTimesheet(model);
                            if (timesheetDBModel != null)
                            {
                                var mappedTimesheet = await MapTimesheetVM(timesheetDBModel);
                                return mappedTimesheet;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetTimesheet method for Approval threw an exception.");
            }
            return null;
        }

        public async Task<List<TimesheetBriefViewModel>> GetAllExistingTimesheetForWeek(long technicianId, DateTime date)
        {
            try
            {
                List<TimesheetBriefViewModel> timeSheets = new List<TimesheetBriefViewModel>();
                DayOfWeek dayOfWeek = date.DayOfWeek;
                if (dayOfWeek != DayOfWeek.Sunday)
                {
                    date = await SetWeekending.SetWeekEnding(date);
                }
                var timesheetsDb = await _db.Timesheets
                                   .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                                   .Include(x => x.TimesheetBreakdowns)
                                   .Where(x =>
                                            (technicianId == 0 || x.WorkOrderTechnician.TechnicianId == technicianId)
                                            &&
                                            (x.WeekEnding.Date.Date == date.Date)
                                         ).ToListAsync();
                foreach (var timesheet in timesheetsDb)
                {
                    var mappedTimesheet = await MapTimesheetVM(timesheet);
                    timeSheets.Add(mappedTimesheet);
                }
                return timeSheets;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async override Task<IRepositoryResponse> Delete(long id)
        {

            try
            {
                if (await CanDelete(id) == false)
                {
                    return UnAuthorizedResponse();
                }

                var dbModel = await _db.Timesheets.Include(x => x.TimesheetBreakdowns).Where(x => x.Id == id).FirstOrDefaultAsync();
                if (dbModel != null)
                {
                    dbModel.IsDeleted = true;
                    dbModel.TimesheetBreakdowns.ForEach(x => x.IsDeleted = true);
                    await _db.SaveChangesAsync();
                }
                _logger.LogWarning($"No record found for id:{id} for Timesheet in Delete()");

                return Response.NotFoundResponse(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete() for Timesheet threw the following exception");
                return Response.BadRequestResponse(_response);
            }
        }

        public async Task<XLWorkbook> DownloadExcel(List<TimeSheetExcelVM> list)
        {
            try
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("TimeSheet");
                var columnHeaders = new List<string>
                {
                    "Date",
                    "Employee",
                    "Customer",
                    "PO Number",
                    "PO Line Item",
                    "Transfer To PO Line Item",
                    "Craft",
                    "TS Ref Status",
                    "TS Ref Number",
                    "Payment Indicator",
                    "ST Hours",
                    "OT Hours",
                    "DT Hours",
                    "ST Rate",
                    "OT Rate",
                    "DT Rate",
                    "Per Diem",
                    "Equipment",
                    "Other",
                    "Total"

                };
                AddColumnHeaders(worksheet, columnHeaders);
                ChangeBackgroudColor(worksheet, 1, XLColor.FromArgb(0, 0, 0));
                AddDataRows(worksheet, list);

                return workbook;
            }
            catch (Exception ex)
            {
                // handle exception
                return null;
            }
        }

        public async Task<TimesheetBreakdownResForModalVM> UpdateModalHeaderValues(List<TimesheetBreakdownForModalVM> model)
        {
            var response = new TimesheetBreakdownResForModalVM();
            foreach (var item in model)
            {
                var timesheetHoursBreakdown = _timesheetLimit.GetHoursBreakdown(item.Day, item.TotalHours);
                response.TotalSTByWeek += timesheetHoursBreakdown.STHours;
                response.TotalOTByWeek += timesheetHoursBreakdown.OTHours;
                response.TotalDTByWeek += timesheetHoursBreakdown.DTHours;
            }
            response.TotalHoursByWeek = response.TotalSTByWeek + response.TotalOTByWeek + response.TotalDTByWeek;
            return response;
        }

        public async Task<long> ModifyTimesheet(TimesheetWebUpdateViewModel model)
        {
            var timesheet = await _db.Timesheets.Where(x => x.Id == model.TimeSheetId).FirstOrDefaultAsync();
            if (timesheet != null)
            {
                timesheet.Note = model.Note;
                await _db.SaveChangesAsync();
                var mappedModel = _mapper.Map<TimesheetBreakdownUpdateViewModel>(model);
                var timeSheetId = await ModifyApiTimeSheetBreakdowns(mappedModel);
                if (timeSheetId > 0)
                {
                    return timeSheetId;
                }
            }
            return 0;
        }

        public async Task<PaginatedResultModel<T>> GetPayPeriodDates<T>(PayPeriodSearchViewModel searchVM)
        {
            try
            {
                var currentDate = DateTime.Now;
                var weekEndingDate = await SetWeekending.SetWeekEnding(currentDate);

                List<PayPeriodBriefViewModel> list = new();


                for (int i = 1; i <= 10; i++)
                {
                    DateTime fromDate = weekEndingDate.AddDays(-13); // Adjusted to subtract 13 days
                    PayPeriodBriefViewModel model = new()
                    {
                        Id = i,
                        Select2Text = $"{fromDate.ToShortDateString()} - {weekEndingDate.ToShortDateString()}"
                    };
                    list.Add(model);

                    weekEndingDate = weekEndingDate.AddDays(-14); // Adding 2 weeks (14 days)
                }


                var result = await list.OrderBy(x => x.Id).ToList().PaginateList(searchVM);

                return result as PaginatedResultModel<T>;
            }
            catch (Exception ex)
            {
                return new PaginatedResultModel<T>();
            }
        }

        private async Task<Timesheet?> GetExistingTimesheet(TimesheetEmployeeSearchViewModel model)
        {
            DayOfWeek dayOfWeek = model.WeekEnding.DayOfWeek;
            if (dayOfWeek != DayOfWeek.Sunday)
            {
                model.WeekEnding = await SetWeekending.SetWeekEnding(model.WeekEnding);
            }
            var timesheet = _db.Timesheets
                               .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.Technician)
                               .Include(x => x.WorkOrderTechnician).ThenInclude(x => x.CraftSkill)
                               .Include(x => x.TimesheetBreakdowns)
                               .Where(x =>
                                        (model.WorkOrderTechnicianId == 0 || x.WorkOrderTechnicianId == model.WorkOrderTechnicianId)
                                        &&
                                        (x.WeekEnding.Date == model.WeekEnding.Date)
                                     ).FirstOrDefault();
            return timesheet;
        }

        public async Task<TimesheetBriefViewModel> MapTimesheetVM(Timesheet? timesheet)
        {
            TimesheetBriefViewModel timesheetViewModel = new TimesheetBriefViewModel();
            if (timesheet != null && timesheet.TimesheetBreakdowns != null && timesheet.TimesheetBreakdowns.Count > 0)
            {
                var craft = timesheet.WorkOrderTechnician?.CraftSkill ?? new CraftSkill();
                timesheetViewModel.TimeSheetId = timesheet.Id;
                timesheetViewModel.Technician = _mapper.Map<TechnicianBriefViewModel>(timesheet.WorkOrderTechnician.Technician);
                timesheetViewModel.Technician.Name = timesheetViewModel.Technician.FirstName + " " + timesheetViewModel.Technician.LastName;
                timesheetViewModel.Craft = _mapper.Map<CraftSkillBriefViewModel>(craft);
                timesheetViewModel.IsApproved = (timesheet.ApproveStatus == TimesheetApproveStatus.Approved);
                timesheetViewModel.STRate = timesheet.STRate;
                timesheetViewModel.OTRate = timesheet.OTRate;
                timesheetViewModel.DTRate = timesheet.DTRate;
                timesheetViewModel.WeekEnding = timesheet.WeekEnding;
                timesheetViewModel.Note = timesheet.Note;
                //timesheetViewModel.Asset = timesheet.WorkOrderTechnician?.WorkOrderId + " - " + timesheet.EmployeeContract?.Contract?.Description + " - " + craft?.Name;

                foreach (var timesheetBreakdown in timesheet.TimesheetBreakdowns)
                {
                    TimesheetBreakdownViewModel viewModel = new()
                    {
                        Id = timesheetBreakdown.Id,
                        Date = timesheetBreakdown.Date,
                        Day = timesheetBreakdown.Day,
                        IsOnSite = timesheetBreakdown.IsOnSite,
                        STHours = timesheetBreakdown.RegularHours,
                        OTHours = timesheetBreakdown.OvertimeHours,
                        DTHours = timesheetBreakdown.DoubleTimeHours,
                        STRate = timesheet.STRate,
                        OTRate = timesheet.OTRate,
                        DTRate = timesheet.DTRate,
                        TotalHours = timesheetBreakdown.RegularHours + timesheetBreakdown.OvertimeHours + timesheetBreakdown.DoubleTimeHours
                    };
                    timesheetViewModel.TimesheetBreakdowns.Add(viewModel);
                }
                await SetDisableDaysDefaultSettings(timesheetViewModel.TimeSheetId, timesheetViewModel.TimesheetBreakdowns.ToList<ITimesheetBreakdownModel>());
            }
            return timesheetViewModel;
        }

        private async Task<long> CreateTimeSheet(TimesheetModifyViewModel model)
        {
            try
            {
                var mappedModel = _mapper.Map<Timesheet>(model);
                mappedModel.ApproveStatus = TimesheetApproveStatus.UnApproved;
                var craft = await _db.CraftSkills.Where(x => x.Id == model.CraftId).FirstOrDefaultAsync();
                mappedModel.WorkOrderId = model.WorkOrder.Id ?? 0;
                mappedModel.STRate = craft?.STRate ?? 0;
                mappedModel.OTRate = craft?.OTRate ?? 0;
                mappedModel.DTRate = craft?.DTRate ?? 0;
                mappedModel.WeekEnding = await SetWeekending.SetWeekEnding(mappedModel.WeekEnding);
                mappedModel.DueDate = mappedModel.WeekEnding.AddDays(23);
                mappedModel.TimesheetBreakdowns = await SetTimesheetBreakdowns(mappedModel.TimesheetBreakdowns, mappedModel.WeekEnding);
                await _db.AddAsync(mappedModel);
                await _db.SaveChangesAsync(model);
                return mappedModel.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Create method of Approval ");
                return -1;
            }
        }


        //Method to read excel and Map them to different POLineItems

        private void AddDataRows(IXLWorksheet worksheet, List<TimeSheetExcelVM> items)
        {
            var row = 2;
            foreach (var item in items)
            {
                var logIndex = 0;
                worksheet.Cell(row, ++logIndex).SetValue(item.Date);
                worksheet.Cell(row, ++logIndex).Value = item.Employee;
                worksheet.Cell(row, ++logIndex).Value = item.Customer;
                worksheet.Cell(row, ++logIndex).Value = item.PONumber;
                worksheet.Cell(row, ++logIndex).SetValue(item.POLineItem);
                worksheet.Cell(row, ++logIndex).Value = item.TransferToPOLineItem;
                worksheet.Cell(row, ++logIndex).Value = item.Craft;
                worksheet.Cell(row, ++logIndex).Value = item.ExcelTSRefStatus;
                worksheet.Cell(row, ++logIndex).Value = item.TSRefNumber;
                worksheet.Cell(row, ++logIndex).Value = item.ExcelPaymentIndicator;
                worksheet.Cell(row, ++logIndex).Value = item.STHours;
                worksheet.Cell(row, ++logIndex).Value = item.OTHours;
                worksheet.Cell(row, ++logIndex).Value = item.DTHours;
                worksheet.Cell(row, ++logIndex).Value = item.STRate;
                worksheet.Cell(row, ++logIndex).Value = item.OTRate;
                worksheet.Cell(row, ++logIndex).Value = item.DTRate;
                worksheet.Cell(row, ++logIndex).Value = item.PerDiem;
                worksheet.Cell(row, ++logIndex).Value = item.Equipment;
                worksheet.Cell(row, ++logIndex).Value = item.Other;
                worksheet.Cell(row, ++logIndex).Value = item.Total;
                row++;
            }
        }

        private static void ChangeBackgroudColor(IXLWorksheet worksheet, int row, XLColor? rowColor)
        {
            if (rowColor != null)
            {
                int columnCount = worksheet.ColumnsUsed().Count();
                var range = worksheet.Range(row, 1, row, columnCount);
                range.Style.Fill.BackgroundColor = rowColor;
                range.Style.Font.FontColor = XLColor.White;
            }
        }

        private void AddColumnHeaders(IXLWorksheet worksheet, List<string> headers, int rowNumber = 1, int skipCell = 0)
        {
            var row = worksheet.Row(rowNumber);
            row.Style.Font.Bold = true; // uncomment it to bold the text of headers row 
            for (int i = 0, ci = skipCell + i; i < headers.Count; i++, ci++)
            {
                row.Cell(ci + 1).Value = headers[i];
            }
        }

        public async Task<string> GetWorkOrderName(long id)
        {
            return await _db.WorkOrder.Where(x => x.Id == id).Select(x => x.SystemGeneratedId).FirstOrDefaultAsync();
        }
    }
}

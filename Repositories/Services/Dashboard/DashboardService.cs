using DataLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Services.Dashboard.Interface;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.Charts.Interfaces;
using ViewModels.Charts;
using ViewModels.Dashboard.interfaces;
using ViewModels.Report.PendingOrder;
using Repositories.Services.Report.Common.interfaces;
using Helpers.Reports;
using ViewModels.Report.WorkOrder;
using Enums;
using DocumentFormat.OpenXml.Bibliography;

namespace Repositories.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserInfoService _userInfoService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportServiceQueries _reportServiceQueries;

        public DashboardService(IUserInfoService userInfoService, ApplicationDbContext db, UserManager<ApplicationUser> userManager, IReportServiceQueries reportServiceQueries)
        {
            _userInfoService = userInfoService;
            _db = db;
            _userManager = userManager;
            _reportServiceQueries = reportServiceQueries;
        }


        public async Task<List<WorkOrderDashboardViewModel>> GetDashboardData()
        {
            var workOrders = await (
                                from wo in _db.WorkOrder.Include(x => x.TaskType).Include(x => x.Asset)
                                join user in _db.Users on wo.CreatedBy equals user.Id
                                group new { wo, user } by wo.Status into g
                                select new
                                {
                                    Status = g.Key,
                                    WorkOrders = g.Select(x => new
                                    {
                                        x.wo,
                                        x.wo.Asset,
                                        WorkStep = x.wo.TaskType.Title ?? "",
                                        CreatedByUser = x.user
                                    }).ToList()
                                }
                            ).ToListAsync();

            var workOrdersDict = workOrders.ToDictionary(wo => wo.Status, wo => wo.WorkOrders);

            // Generate the complete list including all statuses
            var completeWorkOrders = Enum.GetValues(typeof(WOStatusCatalog))
                                         .Cast<WOStatusCatalog>()
                                         .Select(status => new
                                         {
                                             Status = status,
                                             WorkOrders = workOrdersDict.ContainsKey(status)
                                                          ? workOrdersDict[status]
                                                          : new()
                                         }).ToList();


            var list = new List<WorkOrderDashboardViewModel>();

            var imagesUrl = new List<string>()
            {
                "/img/dashboard/image1.jpeg",
                "/img/dashboard/image2.jpeg",
                "/img/dashboard/image3.jpeg",
                "/img/dashboard/image4.jpeg",
                "/img/dashboard/image5.jpeg",
                "/img/dashboard/image6.jpeg"
            };


            var random = new Random();
            foreach (var workOrder in completeWorkOrders)
            {
                var model = new WorkOrderDashboardViewModel
                {
                    Status = workOrder.Status.ToString(),
                };
                var sortedWorkOrders = workOrder.WorkOrders.OrderByDescending(wo => wo.wo.Urgency).ToList();
                foreach (var wo in sortedWorkOrders)
                {
                    int randomIndex = random.Next(imagesUrl.Count);
                    string randomImageUrl = imagesUrl[randomIndex];
                    model.WorkOrders.Add(new WorkOrderDashboardDataViewModel
                    {
                        Id = wo.wo.Id,
                        Number = wo.wo.SystemGeneratedId,
                        Urgency = wo.wo.Urgency,
                        Title = wo.wo.Title,
                        Description = wo.wo.Description,
                        WorkStep = wo.WorkStep,
                        NeedBy = wo.wo.DueDate,
                        CreatedOn = wo.wo.CreatedOn,
                        Manager = wo.CreatedByUser.FirstName + " " + wo.CreatedByUser.LastName,
                        ImageUrl = string.IsNullOrEmpty(wo.wo.DefaultImageUrl) ? randomImageUrl : wo.wo.DefaultImageUrl
                    });
                }
                list.Add(model);
            }
            return list;
        }




        public async Task<ChartResponseViewModel> GetPendingOrderChartData(PendingOrderChartSearchViewModel search)
        {
            try
            {
                var validationReponse = ValidateTimePeriod(search);
                if (validationReponse.IsSuccess == false)
                {
                    return validationReponse;
                }
                var responseQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderChartViewModel>(search);
                var response = await responseQueryable.ToListAsync();
                var reportData = response.SetLabelAndFillMissingData(search.FromDate, search.ToDate, search.TimePeriodGroupingType);
                return SetResponseModel(reportData);
            }
            catch (Exception ex)
            {
            }
            return new ChartResponseViewModel();
        }


        #region[Helpers]
        private ChartResponseViewModel ValidateTimePeriod<T>(T search) where T : class, ITimePeriodSearch, ITimePeriodGroupingType
        {
            var message = "";
            TimeSpan? dateDifference = null;
            if (search.ToDate != null && search.FromDate != null)
            {
                dateDifference = search.ToDate.Value - search.FromDate.Value;

            }

            if (search.TimePeriodGroupingType == TimePeriodGroupingType.Daily)
            {
                if (dateDifference == null || dateDifference?.TotalDays > 30)
                    message = "Date range for daily or weekly grouping should be within 30 days";
            }
            else if (search.TimePeriodGroupingType == TimePeriodGroupingType.Weekly)
            {
                if (dateDifference == null || dateDifference?.TotalDays > 90)
                    message = "Date range for daily or weekly grouping should be within 90 days";
            }
            else if (search.TimePeriodGroupingType == TimePeriodGroupingType.Monthly)
            {
                if (dateDifference == null || dateDifference?.TotalDays > 365 * 2) // 2 years
                    message = "Date range for monthly grouping should be within 2 years";
            }
            else if (search.TimePeriodGroupingType == TimePeriodGroupingType.Quarterly)
            {
                if (dateDifference == null || dateDifference?.TotalDays > 365 * 5) // 5 years
                    message = "Date range for quarterly grouping should be within 5 years";
            }
            else if (search.TimePeriodGroupingType == TimePeriodGroupingType.Annually)
            {
                if (dateDifference == null || dateDifference?.TotalDays > 365 * 10) // 10 years
                    message = "Date range for annually grouping should be within 10 years";
            }
            bool isValid = string.IsNullOrEmpty(message);
            return new ChartResponseViewModel { IsSuccess = isValid, Message = message };

        }

        private ChartResponseViewModel SetResponseModel<T>(List<T> data) where T : IChartDataViewModel
        {
            List<IChartDataViewModel> convertedData = data.Cast<IChartDataViewModel>().ToList();
            return new ChartResponseViewModel { IsSuccess = true, Data = convertedData };

        }

        #endregion

        public async Task<bool> ValidatePassword(string password)
        {
            try
            {
                var loggedInUserId = long.Parse(_userInfoService.LoggedInUserId());
                var user = await _db.Users.Where(x => x.Id == loggedInUserId).FirstOrDefaultAsync();
                var result = await _userManager.CheckPasswordAsync(user, password);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

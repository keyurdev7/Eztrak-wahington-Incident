using Enums;
using Helpers.Reports.Interfaces;
using Helpers.Reports;
using DataLibrary;
using Repositories.Services.Report.Common.interfaces;
using System.Linq.Expressions;
using Helpers.Datetime;
using ViewModels.Report.PendingOrder;
using Repositories.Shared.UserInfoServices.Interface;
using ViewModels.Manager;
using ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Services.Report.Common
{
    public class ReportServiceQueries : IReportServiceQueries
    {
        private readonly ApplicationDbContext _db;
        private readonly string _loggedInUserRole;
        private readonly double _loggedInUserId;
        private readonly bool _isAdminOrManager;
        private readonly bool _isTechnician;

        public ReportServiceQueries(ApplicationDbContext db, IUserInfoService userInfoService)
        {
            _db = db;
            double.TryParse(userInfoService.LoggedInUserId(), out _loggedInUserId);
            _loggedInUserRole = userInfoService.LoggedInUserRole();
            _isAdminOrManager = (_loggedInUserRole == RolesCatalog.SystemAdministrator.ToString() || _loggedInUserRole == RolesCatalog.SuperAdministrator.ToString() || _loggedInUserRole == RolesCatalog.Manager.ToString());
            _isTechnician = (_loggedInUserRole == RolesCatalog.Technician.ToString());
        }

        public async Task<IQueryable<T>> GetPendingOrderReportQuery<T>(PendingOrderReportSearchViewModel search) where T : PendingOrderReportViewModel, IDate, ILabel, new()
        {
            IQueryable<T> queryableRequests = await FilterPendingOrderReportQuery<T>(search);

            Expression<Func<T, object>> additionalKey = (x => 1);
            if (search.PendingOrderReportEntityGroupingType == PendingOrderReportEntityGroupingType.Manufacturer)
            {
                additionalKey = (x => new { ManufacturerId = x.Manufacturer.Id });

            }
            if (search.PendingOrderReportEntityGroupingType == PendingOrderReportEntityGroupingType.Manager)
            {
                additionalKey = (x => new { ManagerId = x.Manager.Id });

            }
            var queryableTimePeriodGroupedPaymentsQueryable = queryableRequests.GroupQueryByTimePeriod(
                search.TimePeriodGroupingType,
                additionalKey,
                _db);

            var queryableTimePeriodGroupedPayments = queryableTimePeriodGroupedPaymentsQueryable.Select(x => new T
            {
                Manufacturer = new ManufacturerBriefViewModel
                {
                    Id = x.Max(y => y.Manufacturer.Id),
                    Name = x.Max(y => y.Manufacturer.Name),
                },
                Manager = new ManagerBriefViewModel()
                {
                    Id = x.Max(y => y.Manager.Id),
                    Name = x.Max(y => y.Manager.Name),
                },
                Orders = x.Count(),
                Date = x.Max(y => y.Date),
            });
            return queryableTimePeriodGroupedPayments;
        }

        public Task<IQueryable<T>> FilterPendingOrderReportQuery<T>(PendingOrderReportSearchViewModel search) where T : PendingOrderReportViewModel, new()
        {
            DateTime threeBusinessDaysBefore = DateTime.Now;// DatetimeHelper.GetNBusinessDaysBefore(DateTime.Now, 3);
            var queryableRequests = (from o in _db.Orders
                                     join wo in _db.WorkOrder on o.WorkOrderId equals wo.Id
                                     join a in _db.Assets on wo.AssetId equals a.Id
                                     join m in _db.Manufacturers on a.ManufacturerId equals m.Id
                                     join au in _db.Users on wo.ManagerId equals au.Id
                                     where
                                     o.CreatedOn < threeBusinessDaysBefore  // Filter out requests older than 3 business days
                                     && o.Status != OrderStatus.Submitted
                                     && (search.FromDate == null || search.FromDate <= o.CreatedOn)
                                     && (search.ToDate == null || search.ToDate >= o.CreatedOn)
                                     && (search.Manufacturer == null || search.Manufacturer.Id == null || search.Manufacturer.Id == 0 || search.Manufacturer.Id == a.ManufacturerId)
                                     && (_isAdminOrManager == false || (search.Manager == null || search.Manager.Id == null || search.Manager.Id == 0 || search.Manager.Id == au.Id))
                                     && (_isTechnician == false || au.Id == _loggedInUserId)
                                     select new T
                                     {
                                         Id = o.Id,
                                         Manager = new ManagerBriefViewModel
                                         {
                                             Id = au.Id,
                                             Name = au.FirstName + " " + au.LastName,
                                         },
                                         Manufacturer = new ManufacturerBriefViewModel()
                                         {
                                             Id = m.Id,
                                             Name = m.Name,
                                         },
                                         Date = o.CreatedOn
                                     }).AsNoTracking();
            return Task.FromResult(queryableRequests);

        }
    }

}

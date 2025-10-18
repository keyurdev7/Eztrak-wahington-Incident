using DataLibrary;
using Helpers.Reports;
using Microsoft.EntityFrameworkCore;
using Repositories.Services.Dashboard.Interface;
using static Helpers.Reports.TimePeriodGroupingHelper;
using Repositories.Services.Report.Common.interfaces;
using ViewModels.Dashboard.Common.Card;
using ViewModels.Report.PendingOrder;

namespace Repositories.Services.Dashboard
{
    public class DashboardCardService : IDashboardCardService
    {
        private readonly IReportServiceQueries _reportServiceQueries;
        private readonly ApplicationDbContext _db;

        public DashboardCardService(IReportServiceQueries reportServiceQueries, ApplicationDbContext db)
        {
            _reportServiceQueries = reportServiceQueries;
            _db = db;
        }


      

       

        public async Task<DashboardCardDataViewModel> GetPendingOrderCardData()
        {
            try
            {
                var response = new DashboardCardDataViewModel("fas fa-calendar-minus");
                var annuallySearchModel = new PendingOrderReportSearchViewModel(TimePeriodGroupingType.Annually, new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now.Date.AddDays(1).AddSeconds(-1));
                var annuallyQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderChartViewModel>(annuallySearchModel);
                var annually = await annuallyQueryable.ToListAsync();
                annually.SetLabelAndFillMissingData(annuallySearchModel.FromDate, annuallySearchModel.ToDate, annuallySearchModel.TimePeriodGroupingType);
                response.Cards.Add(new CardViewModel(annually.FirstOrDefault().Category, annually.FirstOrDefault().FormattedValue, "#dc3545"));


                var quaterlySearchModel = new PendingOrderReportSearchViewModel(TimePeriodGroupingType.Quarterly, QuarterHelper.GetFirstDayOfQuarter(DateTime.Now), DateTime.Now.Date.AddDays(1).AddSeconds(-1));
                var quaterlyQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderChartViewModel>(quaterlySearchModel);
                var quaterly = await quaterlyQueryable.ToListAsync();
                quaterly.SetLabelAndFillMissingData(quaterlySearchModel.FromDate, quaterlySearchModel.ToDate, quaterlySearchModel.TimePeriodGroupingType);
                response.Cards.Add(new CardViewModel(quaterly.FirstOrDefault().Category, quaterly.FirstOrDefault().FormattedValue, "#0d6efd"));


                var monthlySearchModel = new PendingOrderReportSearchViewModel(TimePeriodGroupingType.Monthly, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.Date.AddDays(1).AddSeconds(-1));
                var monthlyQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderChartViewModel>(monthlySearchModel);
                var monthly = await monthlyQueryable.ToListAsync();
                monthly.SetLabelAndFillMissingData(monthlySearchModel.FromDate, monthlySearchModel.ToDate, monthlySearchModel.TimePeriodGroupingType);
                response.Cards.Add(new CardViewModel(monthly.FirstOrDefault().Category, monthly.FirstOrDefault().FormattedValue, "#4650dd"));


                var dailySearchModel = new PendingOrderReportSearchViewModel(TimePeriodGroupingType.Daily, DateTime.Now.Date, DateTime.Now.Date.AddDays(1).AddSeconds(-1));
                var dailyQueryable = await _reportServiceQueries.GetPendingOrderReportQuery<PendingOrderChartViewModel>(dailySearchModel);
                var daily = await dailyQueryable.ToListAsync();
                daily.SetLabelAndFillMissingData(dailySearchModel.FromDate, dailySearchModel.ToDate, dailySearchModel.TimePeriodGroupingType, true);
                response.Cards.Add(new CardViewModel(daily.FirstOrDefault().Category, daily.FirstOrDefault().FormattedValue, "#35b653"));


                return response;
            }
            catch (Exception ex)
            {
            }
            return new DashboardCardDataViewModel();
        }



        #region[Helpers]


        #endregion

    }
}

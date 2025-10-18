using DataLibrary;
using Helpers.Reports.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;


namespace Helpers.Reports
{
    public static class TimePeriodGroupingHelper
    {
        public class PeriodGroupingKey<TAdditional>
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Quarter { get; set; }
            public int Week { get; set; }
            public DateTime Date { get; set; }

            public TAdditional Additional { get; set; }
        }

        public static IQueryable<IGrouping<PeriodGroupingKey<TAdditional>, T>> GroupQueryByTimePeriod<T, TAdditional>(
     this IQueryable<T> query,
     TimePeriodGroupingType groupingType,
     Expression<Func<T, TAdditional>> additionalKey,
     ApplicationDbContext context)
     where T : IDate
        {
            // Define a function to create the PeriodGroupingKey based on grouping type
            Expression<Func<T, TAdditional, PeriodGroupingKey<TAdditional>>> groupingKey;
            switch (groupingType)
            {
                case TimePeriodGroupingType.Annually:
                    groupingKey = (source, additional) => new PeriodGroupingKey<TAdditional>// Source is T. The actual object while additional respresent additionalKey 
                    {
                        Year = source.Date.Year,
                        Additional = additional
                    };
                    break;
                case TimePeriodGroupingType.Quarterly:
                    groupingKey = (source, additional) => new PeriodGroupingKey<TAdditional>
                    {
                        Year = source.Date.Year,
                        Quarter = (source.Date.Month - 1) / 3,
                        Additional = additional
                    };
                    break;
                case TimePeriodGroupingType.Monthly:
                    groupingKey = (source, additional) => new PeriodGroupingKey<TAdditional>
                    {
                        Year = source.Date.Year,
                        Month = source.Date.Month,
                        Additional = additional
                    };
                    break;
                case TimePeriodGroupingType.Weekly:
                    groupingKey = (source, additional) => new PeriodGroupingKey<TAdditional>
                    {
                        Year = source.Date.Year,
                        Week = context.GetWeekNumber(source.Date),
                        Additional = additional
                    };
                    break;
                case TimePeriodGroupingType.Daily:
                    groupingKey = (source, additional) => new PeriodGroupingKey<TAdditional>
                    {
                        Year = source.Date.Year,
                        Date = source.Date.Date,
                        Additional = additional
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupingType), groupingType, null);
            }

            var newGroupingBody = groupingKey.Body;

            // replace original groupingKey parameter with additionalKey parameter
            newGroupingBody = ReplacingExpressionVisitor.Replace(groupingKey.Parameters[0], additionalKey.Parameters[0], newGroupingBody);

            // inject additionalKey body into groupingKey body
            newGroupingBody = ReplacingExpressionVisitor.Replace(groupingKey.Parameters[1], additionalKey.Body, newGroupingBody);

            // combine new grouping lambda
            var groupingLambda = Expression.Lambda<Func<T, PeriodGroupingKey<TAdditional>>>(newGroupingBody, additionalKey.Parameters);

            return query.GroupBy(groupingLambda);
        }


        public static IQueryable<IGrouping<object, T>> GroupQueryByTimePeriod<T>(this IQueryable<T> query,
            TimePeriodGroupingType groupingType,
            ApplicationDbContext context
        ) where T : IDate
        {
            IQueryable<IGrouping<object, T>> groupedQuery = null;
            switch (groupingType)
            {
                case TimePeriodGroupingType.Annually:
                    groupedQuery = query.GroupBy(x => new
                    {
                        x.Date.Year,
                    }).AsQueryable();
                    break;
                case TimePeriodGroupingType.Quarterly:
                    groupedQuery = query.GroupBy(x => new
                    {
                        x.Date.Year,
                        Quarter = ((x.Date.Month - 1) / 3)
                    }).AsQueryable();
                    break;
                case TimePeriodGroupingType.Monthly:
                    groupedQuery = query.GroupBy(x => new
                    {
                        x.Date.Year,
                        //Quarter = ((x.Date.Month - 1) / 3),
                        x.Date.Month
                    }).AsQueryable();
                    break;
                case TimePeriodGroupingType.Weekly:
                    // Assuming weeks start on Mondays
                    groupedQuery = query.GroupBy(x => new
                    {
                        x.Date.Year,
                        //Quarter = ((x.Date.Month - 1) / 3),
                        //x.Date.Month,
                        Week = context.GetWeekNumber(x.Date)
                    }).AsQueryable();
                    break;
                case TimePeriodGroupingType.Daily:
                    groupedQuery = query.GroupBy(x => new
                    {
                        x.Date.Year,
                        //Quarter = ((x.Date.Month - 1) / 3),
                        //x.Date.Month,
                        //Week = context.GetWeekNumber(x.Date),
                        Date = x.Date.Date,
                    }).AsQueryable();
                    break;
            }
            return groupedQuery;
        }


        //public static IQueryable<IGrouping<object, T>> GroupQueryByTimePeriod<T>(
        //    this IQueryable<T> query,
        //    TimePeriodGroupingType groupingType,
        //    ApplicationDbContext context,
        //    string groupExpression = ""
        //    ) where T : IDate
        //{
        //    string timePeriodGroupExpression = "";
        //    switch (groupingType)
        //    {
        //        case TimePeriodGroupingType.Annually:
        //            timePeriodGroupExpression = "Date.Year,Client.Id";
        //            break;
        //        case TimePeriodGroupingType.Quarterly:
        //            timePeriodGroupExpression = $"Date.Year,(Date.Month - 1) / 3 as Quarter";
        //            break;
        //        case TimePeriodGroupingType.Monthly:
        //            timePeriodGroupExpression = $"Date.Year, x.Date.Month";
        //            break;
        //        case TimePeriodGroupingType.Weekly:
        //            timePeriodGroupExpression = $"Date.Year, (Date.DayOfYear + (Date.DayOfWeek == DayOfWeek.Sunday ? -1 : 0)) / 7 as Week";
        //            break;
        //        case TimePeriodGroupingType.Daily:
        //            timePeriodGroupExpression = $"Date.Year,  Date.Date";
        //            break;
        //        default:
        //            throw new ArgumentException("Invalid time period grouping type.");
        //    }

        //    var groupedQuery = query.GroupBy($"new {{{timePeriodGroupExpression}}}") as IQueryable<IGrouping<object, T>>;
        //    return groupedQuery;
        //}

        public static void SetLabelAndIdentifier<T>(this List<T> chartData, TimePeriodGroupingType groupingType,bool needDetailLabel=false) where T : IDate, ILabel, IIdentifier, new()
        {
            foreach (var data in chartData)
            {
                var timePeriod = TimePeriodHelper.GetDesiredLabelAndIdentifier(groupingType, data.Date, needDetailLabel);
                data.Label = timePeriod.Label;
                data.Identifier = timePeriod.Identifier;
            }
        }
        public static List<T> SetLabelAndFillMissingData<T>(this List<T> chartData, DateTime? filterdFromDate, DateTime? filteredToDate, TimePeriodGroupingType groupingType, bool needDetailLabel = false) where T : IDate, ILabel, IIdentifier, new()
        {
            var fromDate = filterdFromDate ?? chartData.Min(x => x.Date);
            var toDate = filteredToDate ?? chartData.Max(x => x.Date);
            var completeTimePeriods = TimePeriodHelper.GetAllTimePeriods(fromDate, toDate, groupingType,needDetailLabel);

            SetLabelAndIdentifier(chartData, groupingType, needDetailLabel);

            if (completeTimePeriods != null && completeTimePeriods.Count <= 90)
            {
                foreach (var period in completeTimePeriods)
                {

                    var existingGroup = chartData.FirstOrDefault(g => g.Label.Equals(period.Label));
                    if (existingGroup == null)
                    {
                        chartData.Add(new T { Label = period.Label, Identifier = period.Identifier });
                    }
                }
            }

            return chartData.OrderBy(x => x.Identifier).ToList();
        }




    }
}

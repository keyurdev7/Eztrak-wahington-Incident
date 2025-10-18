using System.Globalization;
using System.Reflection.Emit;

namespace Helpers.Reports
{
    public static class TimePeriodHelper
    {
        public static List<TimePeriodInfo> GetAllTimePeriods(DateTime fromDate, DateTime toDate, TimePeriodGroupingType periodGroupingType, bool needDetailLabel = false)
        {
            if (periodGroupingType == TimePeriodGroupingType.Annually)
            {
                return GetDesiredLabelAndIdentifiersByYear(fromDate, toDate);
            }
            else if (periodGroupingType == TimePeriodGroupingType.Quarterly)
            {
                return GetDesiredLabelAndIdentifierByQuarter(fromDate, toDate);
            }
            else if (periodGroupingType == TimePeriodGroupingType.Monthly)
            {
                return GetDesiredLabelAndIdentifierByMonths(fromDate, toDate);
            }
            else if (periodGroupingType == TimePeriodGroupingType.Weekly)
            {
                return GetDesiredLabelAndIdentifierByWeeks(fromDate, toDate);
            }
            else
            {
                return GetDesiredLabelAndIdentifierByDay(fromDate, toDate, needDetailLabel);
            }
        }
        private static List<TimePeriodInfo> GetDesiredLabelAndIdentifiersByYear(DateTime fromDate, DateTime toDate)
        {
            var date = fromDate;
            var response = new List<TimePeriodInfo>();
            for (int year = fromDate.Year; year <= toDate.Year; year++)
            {
                response.Add(GetDesiredLabelAndIdentifier(TimePeriodGroupingType.Annually, date));
                date = date.AddYears(1);
            }
            return response;
        }
        private static List<TimePeriodInfo> GetDesiredLabelAndIdentifierByQuarter(DateTime fromDate, DateTime toDate)
        {
            var response = new List<TimePeriodInfo>();
            var quarters = QuarterHelper.GetQuarters(fromDate, toDate);
            var startDate = QuarterHelper.GetFirstDayOfQuarter(fromDate);
            for (int i = 0; i <= quarters; i++)
            {
                response.Add(GetDesiredLabelAndIdentifier(TimePeriodGroupingType.Quarterly, startDate));
                startDate = startDate.AddMonths(3);
                if (startDate > toDate)
                {
                    startDate = toDate;
                }
            }
            return response;
        }
        private static List<TimePeriodInfo> GetDesiredLabelAndIdentifierByMonths(DateTime fromDate, DateTime toDate)
        {
            var response = new List<TimePeriodInfo>();
            fromDate = DayOfMonth.FirstDayOfMonth(fromDate);
            toDate = DayOfMonth.FirstDayOfMonth(toDate);
            while (fromDate <= toDate)
            {
                response.Add(GetDesiredLabelAndIdentifier(TimePeriodGroupingType.Monthly, fromDate));
                fromDate = fromDate.AddMonths(1);
            }
            return response;
        }

        private static List<TimePeriodInfo> GetDesiredLabelAndIdentifierByWeeks(DateTime fromDate, DateTime toDate)
        {
            var response = new List<TimePeriodInfo>();
            var currentDate = fromDate;

            while (currentDate <= toDate)
            {
                response.Add(GetDesiredLabelAndIdentifier(TimePeriodGroupingType.Weekly, currentDate));

                currentDate = currentDate.AddDays(7);
            }

            return response;
        }
        private static List<TimePeriodInfo> GetDesiredLabelAndIdentifierByDay(DateTime fromDate, DateTime toDate, bool needDetailLabel = false)
        {
            var response = new List<TimePeriodInfo>();
            while (fromDate <= toDate)
            {
                response.Add(GetDesiredLabelAndIdentifier(TimePeriodGroupingType.Daily, fromDate, needDetailLabel));
                fromDate = fromDate.AddDays(1);
            }
            return response;
        }

        public static TimePeriodInfo GetDesiredLabelAndIdentifier(TimePeriodGroupingType timePeriodGroupingType, DateTime dt, bool needDetailLabel = false)
        {
            switch (timePeriodGroupingType)
            {
                case TimePeriodGroupingType.Annually:
                    return new TimePeriodInfo { Label = dt.Year.ToString(), Identifier = dt.Year.ToString() };
                case TimePeriodGroupingType.Quarterly:
                    {
                        var label = QuarterHelper.GetFormattedQuarter(dt.Month).ToString() + "-" + dt.Year.ToString();
                        return new TimePeriodInfo { Label = label, Identifier = QuarterHelper.GetQuarterIdentifier(label) };
                    }
                case TimePeriodGroupingType.Monthly:
                    return new TimePeriodInfo { Label = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dt.Month) + "-" + dt.Year, Identifier = MonthHelper.GetMonthIdentifier(dt) };
                case TimePeriodGroupingType.Weekly:
                    {
                        int weekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        int year = dt.Year;
                        var label = $"Week {weekOfYear} - {year}";
                        var identifier = $"{year}{weekOfYear:D2}";
                        return new TimePeriodInfo
                        {
                            Label = label,
                            Identifier = identifier
                        };
                    }

                case TimePeriodGroupingType.Daily:
                    {
                        var label = needDetailLabel ? dt.ToString("MMM dd, yyyy") : dt.ToString("dd");
                        var identifier = needDetailLabel ? $"{dt.Year}{dt.Month:D2}{dt.Day:D2}" : $"{dt.Year}{dt.Day:D2}";
                        return new TimePeriodInfo { Label = label, Identifier = identifier };

                    }
            }
            return new TimePeriodInfo();
        }

    }

    public class MonthHelper
    {
        public static string GetMonthIdentifier(DateTime date)
        {
            return $"{date.Year}-{date.Month:D2}";
        }
    }
    public class QuarterHelper
    {
        public static long GetQuarters(DateTime dt1, DateTime dt2)
        {
            double d1Quarter = GetQuarter(dt1.Month);
            double d2Quarter = GetQuarter(dt2.Month);
            double d1 = d2Quarter - d1Quarter;
            double d2 = (4 * (dt2.Year - dt1.Year));
            return Round(d1 + d2);
        }
        public static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }
        public static DateTime GetFirstDayOfQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1;
            int year = date.Year;

            switch (quarter)
            {
                case 1:
                    return new DateTime(year, 1, 1);
                case 2:
                    return new DateTime(year, 4, 1);
                case 3:
                    return new DateTime(year, 7, 1);
                case 4:
                    return new DateTime(year, 10, 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(date), "Invalid date provided.");
            }
        }
        public static string GetQuarterIdentifier(string quarterLabel)
        {
            var quarter = quarterLabel.Split('-')[0];
            var year = quarterLabel.Split('-')[1];
            switch (quarter)
            {
                case "Q1":
                    return $"{year}1";
                case "Q2":
                    return $"{year}2";
                case "Q3":
                    return $"{year}3";
                case "Q4":
                    return $"{year}4";
                default:
                    throw new ArgumentOutOfRangeException(nameof(quarter), "Invalid date provided.");
            }
        }
        public static string GetFormattedQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return "Q1";
            if (nMonth <= 6)
                return "Q2";
            if (nMonth <= 9)
                return "Q3";
            return "Q4";
        }
        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }
    }
    public class TimePeriodInfo
    {
        public string Label { get; set; }
        public string Identifier { get; set; }
    }
}

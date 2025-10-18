using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Datetime
{
    public static class DatetimeHelper
    {
        public static string FormatDatetimeInPST(this DateTime date)
        {
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime pstTime = TimeZoneInfo.ConvertTimeFromUtc(date, pstZone);
            return pstTime.ToString("MM/dd/yyyy hh:mm tt");
        }
        public static string FormatDateInPST(this DateTime date)
        {
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime pstTime = TimeZoneInfo.ConvertTimeFromUtc(date, pstZone);
            return pstTime.ToString("MM/dd/yyyy");
        }

        public static string FormatDate(this DateTime date)
        {
            return date != new DateTime() ? date.ToString("MM/dd/yyyy") : "-";
        }

        public static string FormatTimeInPST(this DateTime date)
        {
            TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime pstTime = TimeZoneInfo.ConvertTimeFromUtc(date, pstZone);
            return pstTime.ToString("hh:mm tt");
        }

        public static DateTime GetNBusinessDaysBefore(DateTime currentDate, int numberOfBusinessDays)
        {
            int daysToSubtract = numberOfBusinessDays;
            while (daysToSubtract > 0)
            {
                currentDate = currentDate.AddDays(-1);
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                    daysToSubtract--;
            }
            return currentDate;
        }
    }
}

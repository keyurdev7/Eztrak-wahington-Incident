using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class SetWeekending
    {
        public static async Task<DateTime> SetWeekEnding(DateTime date)
        {
            DayOfWeek dayOfWeek = date.DayOfWeek;
            if (dayOfWeek != DayOfWeek.Sunday)
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:

                        date = date.AddDays(6);
                        break;
                    case DayOfWeek.Tuesday:
                        date = date.AddDays(5);
                        break;
                    case DayOfWeek.Wednesday:
                        date = date.AddDays(4);
                        break;
                    case DayOfWeek.Thursday:
                        date = date.AddDays(3);
                        break;
                    case DayOfWeek.Friday:
                        date = date.AddDays(2);
                        break;
                    case DayOfWeek.Saturday:
                        date = date.AddDays(1);
                        break;
                }
            }
            return date.Date.AddDays(1).AddMilliseconds(-1);
        }
    }
}

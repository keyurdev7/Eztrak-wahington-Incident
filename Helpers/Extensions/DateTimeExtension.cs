using System;
using System.Globalization;

namespace Helpers.Extensions
{
    public static class DateTimeExtension
    {
        public static string GetFormatedDateTime(this DateTime? datetime, string format) => !datetime.HasValue ? string.Empty : datetime.Value.ToString(format, (IFormatProvider)CultureInfo.InvariantCulture);

    }
}


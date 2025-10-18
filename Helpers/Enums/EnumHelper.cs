using System;
using Enums;
using Helpers.Extensions;

namespace Helpers.Enums
{
    public static class EnumHelper
    {

        public static List<(string value, string text)> GetEnumList<TEnum>() where TEnum : Enum
        {
            List<(string value, string text)> enumList = new();
            Type enumType = typeof(TEnum);

            if (enumType.IsEnum)
            {
                foreach (var value in Enum.GetValues(enumType))
                {
                    string intValue = value?.ToString() ?? "";
                    string text = ((TEnum)value).GetDisplayName();
                    enumList.Add((intValue, text));
                }
            }

            return enumList;
        }
    }
}


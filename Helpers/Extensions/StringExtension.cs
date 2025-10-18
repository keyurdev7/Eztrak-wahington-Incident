using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class StringExtension
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public static string RemoveSpecialChars(this string input)
        {
            return Regex.Replace(input, @"[^0-9a-zA-Z\._]", string.Empty);
        }
        public static DateTime ToDateTime(this string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                    return DateTime.Parse(input);
            }
            catch (Exception)
            {
            }
            return DateTime.Now;
        }
        public static string AddSpaceBeforeCapital(this string input)
        {
            // Use regex to find capital letters
            string spacedString = Regex.Replace(input, "([A-Z])", " $1");
            return spacedString.Trim(); // Trim to remove leading space if any
        }
    }
}

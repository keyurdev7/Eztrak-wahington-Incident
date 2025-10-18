using System.Text.RegularExpressions;

namespace Helpers.Extensions;

public static class CommonHelper
{
    public static string MakeValidFileName(string name)
    {
        char[] charArray = "#\\//$&%".ToCharArray();
        string pattern = string.Format("([{0}]*\\.+$)|([{0}]+)", Regex.Escape(new string(Path.GetInvalidFileNameChars().Concat(charArray).ToArray())));
        return Regex.Replace(name, pattern, "_");
    }
    public static string TruncateWithEllipsis(this string input, int maxLength = 30)
    {
        if (input.Length <= maxLength)
        {
            // No truncation needed
            return input;
        }
        else
        {
            // Truncate the string and add ellipsis
            return input.Substring(0, maxLength - 3) + "...";
        }
    }

}

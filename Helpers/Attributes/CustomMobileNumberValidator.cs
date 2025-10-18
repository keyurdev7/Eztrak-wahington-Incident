using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers.ValidationAttributes
{
    public class CustomMobileNumberValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;
            var number = value?.ToString().RemoveSpecialChars();
            Regex _regex = new Regex(@"^[0-9]{7,15}$", RegexOptions.Compiled);
            return _regex.IsMatch(number);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Enums
{
    public enum TicketCategory
    {
        [Display(Name = "Account Access")]
        AccountAccess,
        [Display(Name = "Error Message")]
        ErrorMessage,
        [Display(Name = "Other")]
        Other,
        [Display(Name = "Technical Support")]
        TechnicalSupport

    }
}

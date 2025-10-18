using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Enums
{
    public enum TSRefStatus
    {

        [Display(Name = "AUTH")]
        [Description("AUTH")]
        AUTH,

        [Display(Name = "NOT ACC")]
        [Description("NOT ACC")]
        NOT_ACC,

        [Display(Name = "NOT AUTH")]
        [Description("NOT AUTH")]
        NOT_AUTH,

        [Display(Name = "REV")]
        [Description("REV")]
        REV,

        [Display(Name = "INT")]
        [Description("INT")]
        INT
    }
}

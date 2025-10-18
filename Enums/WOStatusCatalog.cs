using System.ComponentModel.DataAnnotations;

namespace Enums
{
    public enum WOStatusCatalog
    {
        Open = 0,
        Working = 10,
        Complete = 20,
        [Display(Name = "On Hold")]
        OnHold = 30,
        Approved = 100
    }
}

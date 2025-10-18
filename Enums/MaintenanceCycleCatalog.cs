using System.ComponentModel.DataAnnotations;

namespace Enums
{
    public enum MaintenanceCycleCatalog
    {
        [Display(Name = "1 Year")]
        OneYear = 1,
        [Display(Name = "2 Years")]
        TwoYears = 2,
        [Display(Name = "3 Years")]
        ThreeYears = 3,
        [Display(Name = "5 Years")]
        FiveYears = 5,
        [Display(Name = "10 Years")]
        TenYears = 10
    }
}

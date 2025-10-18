using System.ComponentModel.DataAnnotations;

namespace Enums
{
    public enum RolesCatalog
    {
        SystemAdministrator,
        [Display(Name = "Master Admin")]
        SuperAdministrator,
        [Display(Name = "General User (read only)")]
        Guest,
        [Display(Name = "Supervisor")]
        Manager,
        Technician
    }
}

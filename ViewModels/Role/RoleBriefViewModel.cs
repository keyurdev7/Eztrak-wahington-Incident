using Enums;
using Helpers.Extensions;
using Select2.Model;
using System.ComponentModel;

namespace ViewModels.Role
{
    public class RoleBriefViewModel : BaseSelect2VM, ISelect2BaseVM
    {
        public RoleBriefViewModel() : base(true, "The Role field is required.")
        {

        }
        [DisplayName("Role")]
        public string? Name { get; set; }
        public string? DisplayName
        {
            get
            {
                try
                {
                    RolesCatalog role = (RolesCatalog)Enum.Parse(typeof(RolesCatalog), Name);
                    return role.GetDisplayName();
                }
                catch
                {
                    return "";
                }
            }
        }
        public override string? Select2Text
        {
            get
            {
                return Name;
            }
        }
    }

}

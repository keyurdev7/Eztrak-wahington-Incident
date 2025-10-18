using Enums;
using Helpers.Extensions;
using Models.Common.Interfaces;
using System.ComponentModel;

namespace ViewModels.Shared
{
    public interface IBaseCrudViewModel : IIsActive
    {

    }
    public class BaseCrudViewModel : IBaseCrudViewModel
    {
        [DisplayName("Status")]
        public ActiveStatus ActiveStatus { get; set; } = ActiveStatus.Active;
        public string FormattedActiveStatus
        {
            get
            {
                return ActiveStatus.GetDisplayName();
            }
        }

        public RolesCatalog? LoggedInUserRole { get; set; }


    }

    public interface IEmailVM
    {
        string Email { get; set; }
    }
}

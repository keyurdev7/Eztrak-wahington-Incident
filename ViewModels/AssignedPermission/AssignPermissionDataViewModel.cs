using Enums;
using ViewModels.Role;
using ViewModels.Users;

namespace ViewModels.AssignedPermissions
{
    public class AssignPermissionDataViewModel
    {
        public long EntityId { get; set; }
        public UserBriefViewModel User { get; set; } = new();
        public RoleBriefViewModel Role { get; set; } = new();
        public PermissionEntityType EntityType { get; set; } = PermissionEntityType.Role;
        public List<AssignedPermissionUpdateViewModel> Permissions { get; set; } = new();
        public bool HideEntityDiv { get => (EntityType == PermissionEntityType.User && User.Id > 0) || (EntityType == PermissionEntityType.Role && Role.Id > 0); }
        public string EntityTitle { get => EntityType == PermissionEntityType.User ? User.Name : Role.Name; }
        public bool IsGeneralPermissionsScreen { get; set; } = false;
    }
}


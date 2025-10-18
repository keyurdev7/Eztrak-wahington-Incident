using System;
using Enums;
using ViewModels.Permission;
using ViewModels.Shared;

namespace ViewModels.AssignedPermissions
{
    public class AssignedPermissionCreateViewModel : BaseVM
    {
        public PermissionStatus Status { get; set; }
        public long EntityId { get; set; }
        public PermissionEntityType EntityType { get; set; }
        public PermissionBriefViewModel Permission { get; set; } = new();
    }
}


using System;
using Enums;
using Select2.Model;
using ViewModels.Permission;
using ViewModels.Shared;

namespace ViewModels.AssignedPermissions
{
    public class AssignedPermissionDetailViewModel : BaseVM
    {
        public long Priority { get; set; }
        public PermissionStatus Status { get; set; }
        public long EntityId { get; set; }
        public PermissionEntityType EntityType { get; set; }
        public PermissionBriefViewModel Permission { get; set; } = new();

    }
}


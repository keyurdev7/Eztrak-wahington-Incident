using System;
using Enums;
using Models.Common.Interfaces;
using ViewModels.Permission;
using ViewModels.Shared;

namespace ViewModels.AssignedPermissions
{
    public class AssignedPermissionUpdateViewModel : BaseUpdateVM, IIdentitifier
    {
        public long Priority { get; set; }
        public PermissionStatus Status { get; set; }
        public long EntityId { get; set; }
        public PermissionEntityType EntityType { get; set; }
        public PermissionBriefViewModel Permission { get; set; } = new();
    }
}


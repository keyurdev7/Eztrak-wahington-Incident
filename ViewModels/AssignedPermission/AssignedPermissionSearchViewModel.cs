using System;
using Enums;
using Pagination;
using ViewModels.Permission;
using ViewModels.Shared;

namespace ViewModels.AssignedPermissions
{
    public class AssignedPermissionSearchViewModel : BaseSearchModel
    {
        public PermissionStatus? Status { get; set; }
        public long? EntityId { get; set; }
        public PermissionEntityType? EntityType { get; set; }
        public PermissionBriefViewModel Permission { get; set; } = new();
    }
}


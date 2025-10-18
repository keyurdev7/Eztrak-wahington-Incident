using System;
using Enums;
using Microsoft.EntityFrameworkCore;
using Models.Models.Shared;

namespace Models
{
    //[Index(nameof(Name), IsUnique = true)]
    public class AssignedPermission : BaseDBModel
    {
        public long Priority { get; set; }
        public PermissionStatus Status { get; set; }
        public long EntityId { get; set; }
        public PermissionEntityType EntityType { get; set; }

        public long PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}


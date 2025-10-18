using System;
using Enums;
using Microsoft.AspNetCore.Identity;
using Models.Common.Interfaces;

namespace Models
{
    public class ApplicationRole : IdentityRole<long>, IBaseModel
    {
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public ActiveStatus ActiveStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
    }
}


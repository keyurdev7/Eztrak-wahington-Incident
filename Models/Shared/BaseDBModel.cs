using Enums;
using Models.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Models.Models.Shared
{
    public abstract class BaseDBModel : IBaseModel

    {
        [Key]
        public long Id { get; set; }
        public bool IsDeleted { get; set; }
        public ActiveStatus ActiveStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
    }
}

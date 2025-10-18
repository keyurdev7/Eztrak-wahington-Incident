using System.ComponentModel.DataAnnotations.Schema;
using Models.Models.Shared;

namespace Models
{
    public class DynamicColumnValue : BaseDBModel
    {
        public string Value { get; set; }

        public long EntityId { get; set; }

        [ForeignKey("DynamicColumn")]
        public long DynamicColumnId { get; set; }
        public DynamicColumn DynamicColumn { get; set; }
    }
}

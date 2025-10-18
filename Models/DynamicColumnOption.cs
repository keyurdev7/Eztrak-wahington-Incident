
using Enums;
using Models.Models.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class DynamicColumnOption : BaseDBModel
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public DynamicColumnType Type { get; set; }

        [ForeignKey("DynamicColumn")]
        public long DynamicColumnId { get; set; }
        public DynamicColumn DynamicColumn { get; set; }
    }
}

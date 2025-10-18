using Enums;
using Models.Models.Shared;

namespace Models
{
    public class LogData : BaseDBModel
    {
        public string TableName { get; set; }
        public string? JsonDBModelData { get; set; }
        public string? JsonViewModelData { get; set; }
        public LogDataAction Action { get; set; }
        public long UserId { get; set; }
        public string CorrelationId { get; set; }
        public string Type { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? AffectedColumns { get; set; }
        public long PrimaryKey { get; set; }
    }
}


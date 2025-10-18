using System;
using Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models;
using Newtonsoft.Json;

namespace ViewModels.Shared
{
    public class LogDataViewModel
    {
        public LogDataViewModel(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public long UserId { get; set; }
        public string CorrelationId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public LogDataAction AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
        public string JsonDBModelData { get; set; }
        public string JsonViewModelData { get; set; }
        public LogData ToAudit()
        {
            var currentDate = DateTime.Now;
            var audit = new LogData
            {
                UserId = UserId,
                Type = AuditType.ToString(),
                Action = AuditType,
                TableName = TableName,
                CorrelationId = CorrelationId,
                PrimaryKey = (long)KeyValues.Where(x => x.Key == "Id").Select(x => x.Value).FirstOrDefault(),
                OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }),
                JsonDBModelData = JsonDBModelData,
                JsonViewModelData = JsonViewModelData,
                CreatedOn = currentDate,
                UpdatedOn = currentDate,
                CreatedBy = UserId,
                UpdatedBy = UserId
            };
            return audit;
        }
    }
}


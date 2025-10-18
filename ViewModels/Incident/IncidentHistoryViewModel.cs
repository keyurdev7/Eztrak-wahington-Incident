using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{
    public class IncidentHistoryViewModel
    {
        public long IncidentId { get; set; }
        public string IncidentNumber { get; set; } = string.Empty;

        // Summary
        public int StatusChangeCount { get; set; }
        public int NotesCount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        // History Records
        public List<IncidentHistoryRecord> HistoryRecords { get; set; } = new();
    }

    public class IncidentHistoryRecord
    {
        public string StatusName { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Intersection { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string SeverityColor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EsIndicators { get; set; } = string.Empty;
    }
}

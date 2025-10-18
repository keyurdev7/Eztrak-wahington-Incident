using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{
    public class SaveValidationNoteRequest
    {
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }   // optional (0) if none
        public string Notes { get; set; } = string.Empty;
    }
}

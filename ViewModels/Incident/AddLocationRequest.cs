using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{
    public class AddLocationRequest
    {
        public long IncidentId { get; set; }
        public string Address { get; set; }
    }

    public class IncidentMapChatRequest
    {
        public long? IncidentId { get; set; }
        public string ChatMessage { get; set; }
        public string SentBy { get; set; }
    }
    public class AssestmentFilterRequest
    {
        public long IncidentId { get; set; }
        public string step { get; set; } = string.Empty;
        public long ownerId { get; set; }
        public long statusID { get; set; }
    }
}

using Models.Models.Shared;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class IncidentMapChat : BaseDBModel
    {
        [ForeignKey("IncidentId")]
        public long? IncidentId { get; set; }
        public Incident Incident { get; set; }
        public string ChatMessage { get; set; }
        public string SentBy { get; set; }
    }
}

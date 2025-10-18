using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Incident
{
    public class AddPersonRequest
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public long RoleId { get; set; }
        public long ShiftId { get; set; }
        public long IncidentId { get; set; }
        public long IncidentValidationId { get; set; }
    }
}

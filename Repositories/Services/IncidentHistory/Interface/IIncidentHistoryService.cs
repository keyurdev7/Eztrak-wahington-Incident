using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Incident;

namespace Repositories.Services.IncidentHistory.Interface
{
    public interface IIncidentHistoryService
    {
        Task<IncidentHistoryViewModel> GetIncidentHistoryAsync(long incidentId);
        Task<string> GetEventTypes(string ids);
    }
}

using Centangle.Common.ResponseHelpers.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

using Models;
using Models.Common.Interfaces;

using Pagination;

using Repositories.Interfaces;

using ViewModels;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IIncidentValidationService
    {
        Task<List<IncidentValidationPendingViewModel>> GetValidationPendingList();
        Task<List<RecentlyIncidentValidationViewModel>> GetRecentlyValidationList();
        Task<long> GetHighPriorityIncidentCount();
        Task<IncidentValidationViewModel> GetIncidentValidationAlarm(long id);
        Task<IncidentValidationDetailViewModel> GetIncidentValidationDetail(long id);
        Task<List<IncidentResponseTeamViewModel>> GetIncidentValidationResponseTeam();
        Task<List<IncidentPolicyViewModel>> GetIncidentValidationPolicy();
        Task<long> SavePolicy(PolicyModifyViewModel request);
        Task<List<SelectListItem>> GetTeamsList();
        Task<long> SaveIncidentValidation(IncidentSubmitViewModel request);
        Task<List<TeamWithUsersViewModel>> GetIncidentTeamUsers();
        Task<List<IncidentAdditionalLocationViewModel>> GetIncidentAdditionalLocationByIncident(long incidentId);
        Task<GeocodeResult?> GetLatLngFromAddress(string address);
        Task<long> AddAdditionalLocationAsync(long incidentId, string address, double lat, double lng);
        Task<long> DeleteAdditionalLocationAsync(long id);
    }
}

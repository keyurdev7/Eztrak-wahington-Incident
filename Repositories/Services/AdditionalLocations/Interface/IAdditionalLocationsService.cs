using Centangle.Common.ResponseHelpers.Models;
using Models.Common.Interfaces;
using Pagination;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Dashboard;
using ViewModels.Incident;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IAdditionalLocationsService
    {
        Task<string> SaveadditionalLocations(List<AdditionalLocationViewModel> additionalLocations);

        // Verification/import support (per-incident)
        Task<List<AdditionalLocationViewModel>> GetVerificationLocationsByIncidentId(long incidentId);
        Task<AdditionalLocationViewModel?> GetVerificationLocationById(long id);
        Task<int> AddVerificationLocations(long incidentId, IEnumerable<AdditionalLocationViewModel> locations, Guid importBatchId);
        Task<bool> UpdateVerificationLocation(long id, string status, string? notes, string? serviceAccount, string? assetIds, string? photoUrl, long? userId, string? userName);
    }
}

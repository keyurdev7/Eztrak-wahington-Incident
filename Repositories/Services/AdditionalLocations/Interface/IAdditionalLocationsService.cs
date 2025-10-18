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
    }
}

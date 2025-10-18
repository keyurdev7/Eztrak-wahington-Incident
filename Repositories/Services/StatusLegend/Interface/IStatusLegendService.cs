using Models.Common.Interfaces;

using Repositories.Interfaces;

using System;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IStatusLegendService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<StatusLegendModifyViewModel>> GetAllStatusLegends();
        Task<long> SaveStatusLegend(StatusLegendModifyViewModel viewModel);
        Task<long> UpdateStatusLegend(StatusLegendModifyViewModel viewModel);
        Task<StatusLegendModifyViewModel> GetStatusLegendById(long Id);
        Task<long> DeleteStatusLegend(long id);
    }
}
    
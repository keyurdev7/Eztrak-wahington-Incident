using Models.Common.Interfaces;

using Repositories.Interfaces;

using System;

using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface ISeverityLevelService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<SeverityLevelModifyViewModel>> GetAllSeverityLevels();
        Task<long> SaveSeverityLevel(SeverityLevelModifyViewModel viewModel);
        Task<long> UpdateSeverityLevel(SeverityLevelModifyViewModel viewModel);
        Task<SeverityLevelModifyViewModel> GetSeverityLevelById(long Id);
        Task<long> DeleteSeverityLevel(long id);
    }
}

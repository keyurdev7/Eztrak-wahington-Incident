using Models.Common.Interfaces;
using Repositories.Interfaces;
using ViewModels;
using ViewModels.Shared;

namespace Repositories.Common
{
    public interface IProgressService<CreateViewModel, UpdateViewModel, DetailViewModel>
        : IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel>
        where DetailViewModel : class, IBaseCrudViewModel, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {
        Task<List<ProgressModifyViewModel>> GetAllProgress();
        Task<long> SaveProgress(ProgressModifyViewModel viewModel);
        Task<long> UpdateProgress(ProgressModifyViewModel viewModel);
        Task<ProgressModifyViewModel> GetProgressById(long Id);
        Task<long> DeleteProgress(long id);
    }
}
using Centangle.Common.ResponseHelpers.Models;
using ViewModels.Shared;

namespace Repositories.Interfaces
{
    public interface IBaseCreate<CreateViewModel>
        where CreateViewModel : class, IBaseCrudViewModel, new()
    {
        Task<IRepositoryResponse> Create(CreateViewModel model);
    }

}

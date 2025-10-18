using Centangle.Common.ResponseHelpers.Models;
using ViewModels.Shared;

namespace Repositories.Interfaces
{
    public interface IBaseUpdate<UpdateViewModel>
        where UpdateViewModel : class, IBaseCrudViewModel, new()
    {
        Task<IRepositoryResponse> Update(UpdateViewModel model);
    }

}

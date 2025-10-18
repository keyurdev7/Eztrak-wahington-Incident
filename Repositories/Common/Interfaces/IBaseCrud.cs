using Models.Common.Interfaces;
using ViewModels.Shared;

namespace Repositories.Interfaces
{
    public interface IBaseCrud<CreateViewModel, UpdateViewModel, DetailViewModel> :
        IBaseSearch,
        IBaseDetail,
        IBaseCreate<CreateViewModel>,
        IBaseUpdate<UpdateViewModel>,
        IBaseDelete
        where DetailViewModel : class, new()
        where CreateViewModel : class, IBaseCrudViewModel, new()
        where UpdateViewModel : class, IBaseCrudViewModel, IIdentitifier, new()
    {

    }
}

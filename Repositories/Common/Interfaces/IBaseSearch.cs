using Centangle.Common.ResponseHelpers.Models;
using Pagination;

namespace Repositories.Interfaces
{
    public interface IBaseSearch
    {
        Task<IRepositoryResponse> GetAll<M>(IBaseSearchModel model);
    }

}

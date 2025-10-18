using Centangle.Common.ResponseHelpers.Models;

namespace Repositories.Interfaces
{
    public interface IBaseDelete
    {
        Task<IRepositoryResponse> Delete(long id);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Centangle.Common.ResponseHelpers.Models;

namespace Repositories.Interfaces
{
    public interface IBaseDetail
    {
        Task<IRepositoryResponse> GetById(long id);
    }
}

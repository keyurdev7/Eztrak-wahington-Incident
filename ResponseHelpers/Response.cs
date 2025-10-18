using Centangle.Common.ResponseHelpers.Models;
using System.Net;

namespace Centangle.Common.ResponseHelpers
{
    public static class Response
    {
        public static IRepositoryResponse UnAuthorizedResponse(IRepositoryResponse repositoryResponse)
        {
            repositoryResponse.Status = HttpStatusCode.Unauthorized;
            return repositoryResponse;
        }
        public static IRepositoryResponse BadRequestResponse(IRepositoryResponse repositoryResponse)
        {
            repositoryResponse.Status = HttpStatusCode.BadRequest;
            return repositoryResponse;
        }
        public static IRepositoryResponse NotFoundResponse(IRepositoryResponse repositoryResponse)
        {
            repositoryResponse.Status = HttpStatusCode.NotFound;
            return repositoryResponse;
        }
    }
}

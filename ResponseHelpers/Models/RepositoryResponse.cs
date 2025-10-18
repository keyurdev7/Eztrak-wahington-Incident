using System.Net;

namespace Centangle.Common.ResponseHelpers.Models
{
    public interface IRepositoryResponse
    {
        HttpStatusCode Status { get; set; }
        string Message { get; set; }
    }

    public class RepositoryResponse : IRepositoryResponse
    {
        public RepositoryResponse()
        {
            Status = HttpStatusCode.OK;
        }
        public HttpStatusCode Status { get; set; } 
        public string Message { get; set; }
    }

    public interface IRepositoryResponseWithModel<T> : IRepositoryResponse where T : new()
    {
        T ReturnModel { get; set; }
    }

    public class RepositoryResponseWithModel<T> : RepositoryResponse, IRepositoryResponseWithModel<T> where T : new()
    {
        public RepositoryResponseWithModel()
        {
            ReturnModel = new T();
        }
        public T ReturnModel { get; set; }
    }
}

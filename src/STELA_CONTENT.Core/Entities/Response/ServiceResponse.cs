using System.Net;

namespace STELA_CONTENT.Core.Entities.Response
{
    public class ServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public T? Body { get; set; }
    }
}
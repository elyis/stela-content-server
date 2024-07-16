using System.Net;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.IService
{
    public interface IMaterialService
    {
        Task<HttpStatusCode> Create(CreateMemorialMaterialBody body);
        Task<ServiceResponse<MemorialMaterialBody>> Get(Guid id);
        Task<HttpStatusCode> Remove(Guid id);
        Task<ServiceResponse<PaginationResponse<MemorialMaterialBody>>> GetAll(int offset, int count);
    }
}
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.IService
{
    public interface IMemorialService
    {
        Task<ServiceResponse<MemorialBody>> Create(CreateMemorialBody body);
        Task<ServiceResponse<MemorialBody>> Get(Guid id);
        Task<ServiceResponse<PaginationResponse<ShortMemorialBody>>> GetAll(int offset, int count);
    }
}
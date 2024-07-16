using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.IService
{
    public interface IPortfolioMemorialService
    {
        Task<ServiceResponse<PaginationResponse<ShortPortfolioMemorialBody>>> GetAll(int offset, int count);
        Task<ServiceResponse<PortfolioMemorialBody>> Get(Guid id);
        Task<ServiceResponse<PortfolioMemorialBody>> Create(CreatePortfolioMemorialBody body);
    }
}
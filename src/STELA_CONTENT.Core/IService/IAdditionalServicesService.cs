using System.Net;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.IService
{
    public interface IAdditionalServicesService
    {
        Task<ServiceResponse<IEnumerable<AdditionalServiceBody>>> GetAdditionalServices(int count, int offset);
        Task<ServiceResponse<AdditionalServiceBody>> Create(CreateAdditionalServiceBody body);
        Task<HttpStatusCode> Remove(Guid id);
        Task<ServiceResponse<AdditionalServiceBody>> GetService(Guid id);
        Task<ServiceResponse<AdditionalServiceBody>> GetService(string name);
    }
}
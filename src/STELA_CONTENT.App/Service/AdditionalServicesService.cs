using System.Net;
using Microsoft.EntityFrameworkCore;
using STELA_CONTENT.Core.Entities.Models;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.App.Service
{
    public class AdditionalServicesService : IAdditionalServicesService
    {
        private readonly ContentDbContext _context;

        public AdditionalServicesService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AdditionalServiceBody>> Create(CreateAdditionalServiceBody body)
        {
            var service = await GetService(body.Name);
            if (service.IsSuccess)
            {
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    IsSuccess = false,
                    Body = null
                };
            }

            var newService = new AdditionalService
            {
                Name = body.Name,
                Price = body.Price
            };

            await _context.AdditionalServices.AddAsync(newService);
            await _context.SaveChangesAsync();

            return new ServiceResponse<AdditionalServiceBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = newService.ToAdditionalServiceBody()
            };
        }

        public async Task<ServiceResponse<IEnumerable<AdditionalServiceBody>>> GetAdditionalServices(int count, int offset)
        {
            var services = await _context.AdditionalServices.OrderBy(s => s.Name)
                                                            .Skip(offset)
                                                            .Take(count)
                                                            .ToListAsync();
            return new ServiceResponse<IEnumerable<AdditionalServiceBody>>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = services.Select(s => s.ToAdditionalServiceBody())
            };
        }

        public async Task<ServiceResponse<AdditionalServiceBody>> GetService(Guid id)
        {
            var service = await _context.AdditionalServices.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
            {
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    Body = null
                };
            }

            return new ServiceResponse<AdditionalServiceBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = service.ToAdditionalServiceBody()
            };
        }

        public async Task<ServiceResponse<AdditionalServiceBody>> GetService(string name)
        {
            var nameInLower = name.ToLower();
            var service = await _context.AdditionalServices.FirstOrDefaultAsync(s => s.Name.ToLower() == nameInLower);
            if (service == null)
            {
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    Body = null
                };
            }

            return new ServiceResponse<AdditionalServiceBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = service.ToAdditionalServiceBody()
            };
        }

        public async Task<HttpStatusCode> Remove(Guid id)
        {
            var service = await _context.AdditionalServices.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return HttpStatusCode.NoContent;

            _context.AdditionalServices.Remove(service);
            await _context.SaveChangesAsync();

            return HttpStatusCode.NoContent;
        }
    }
}
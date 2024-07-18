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
        private readonly ICacheService _cacheService;
        private const string _prefixKey = "additional_service_";

        public AdditionalServicesService(
            ContentDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
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

            var key = GetKey(newService.Id.ToString());
            await _cacheService.SetCache(key, newService, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10));

            return new ServiceResponse<AdditionalServiceBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = newService.ToAdditionalServiceBody()
            };
        }

        public async Task<ServiceResponse<IEnumerable<AdditionalServiceBody>>> GetAdditionalServices(int count, int offset)
        {
            var key = GetKey($"all_{count}_{offset}");
            var services = await _cacheService.GetCache<IEnumerable<AdditionalService>>(key);
            if (services != null)
                return new ServiceResponse<IEnumerable<AdditionalServiceBody>>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Body = services.Select(s => s.ToAdditionalServiceBody())
                };

            services = await _context.AdditionalServices.AsNoTracking()
                                                        .OrderBy(s => s.Name)
                                                        .Skip(offset)
                                                        .Take(count)
                                                        .ToListAsync();
            await _cacheService.SetCache(key, services, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5));

            return new ServiceResponse<IEnumerable<AdditionalServiceBody>>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = services.Select(s => s.ToAdditionalServiceBody())
            };
        }

        public async Task<ServiceResponse<AdditionalServiceBody>> GetService(Guid id)
        {
            var key = GetKey(id.ToString());
            var service = await _cacheService.GetCache<AdditionalService>(key);
            if (service != null)
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Body = service.ToAdditionalServiceBody()
                };

            service = await _context.AdditionalServices.AsNoTracking()
                                                           .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
            {
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    Body = null
                };
            }

            await _cacheService.SetCache(key, service, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10));

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
            var key = GetKey(nameInLower);
            var service = await _cacheService.GetCache<AdditionalService>(key);
            if (service != null)
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Body = service.ToAdditionalServiceBody()
                };

            service = await _context.AdditionalServices.AsNoTracking()
                                                           .FirstOrDefaultAsync(s => s.Name.ToLower() == nameInLower);
            if (service == null)
            {
                return new ServiceResponse<AdditionalServiceBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    Body = null
                };
            }

            await _cacheService.SetCache(key, service, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10));

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

            await _cacheService.RemoveCache(GetKey(service.Id.ToString()));
            await _cacheService.RemoveCache(GetKey(service.Name.ToLower()));

            return HttpStatusCode.NoContent;
        }

        private string GetKey(string identifier) => $"{_prefixKey}{identifier}";
    }
}
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using STELA_CONTENT.Core.Entities.Models;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.App.Service
{
    public class MaterialService : IMaterialService
    {
        private readonly ContentDbContext _context;
        private readonly ICacheService _cacheService;
        private const string _prefixKey = "material_";

        public MaterialService(
            ContentDbContext context,
            ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<HttpStatusCode> Create(CreateMemorialMaterialBody body)
        {
            var nameInLowercase = body.Name.ToLower();
            var material = await _context.Materials.FirstOrDefaultAsync(m => m.Name.ToLower() == nameInLowercase);
            if (material != null)
                return HttpStatusCode.Conflict;

            material = new MemorialMaterial
            {
                Name = body.Name,
                Hex = body.Hex,
                ColorName = body.ColorName,
            };

            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();

            var key = GetKey(material.Id.ToString());
            await _cacheService.SetCache(key, material, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10));
            return HttpStatusCode.OK;
        }

        public async Task<ServiceResponse<MemorialMaterialBody>> Get(Guid id)
        {
            var key = GetKey(id.ToString());
            var material = await _cacheService.GetCache<MemorialMaterial>(key);
            if (material != null)
                return new ServiceResponse<MemorialMaterialBody>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Body = material.ToMemorialMaterialBody()
                };

            material = await _context.Materials.AsNoTracking()
                                               .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
                return new ServiceResponse<MemorialMaterialBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                };

            await _cacheService.SetCache(key, material, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10));
            return new ServiceResponse<MemorialMaterialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = material.ToMemorialMaterialBody()
            };
        }

        public async Task<ServiceResponse<PaginationResponse<MemorialMaterialBody>>> GetAll(int count, int offset)
        {
            var key = GetKey($"all_{count}_{offset}");
            var materials = await _cacheService.GetCache<IEnumerable<MemorialMaterial>>(key);
            if (materials != null)
                return new ServiceResponse<PaginationResponse<MemorialMaterialBody>>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Body = new PaginationResponse<MemorialMaterialBody>
                    {
                        Count = count,
                        Offset = offset,
                        Total = materials.Count(),
                        Items = materials.Select(m => m.ToMemorialMaterialBody())
                    }
                };

            materials = await _context.Materials.AsNoTracking()
                                                .OrderBy(m => m.Name)
                                                .Skip(offset)
                                                .Take(count)
                                                .ToListAsync();


            var keyTotalMaterials = GetKey("total_materials_");
            var totalMaterials = await _cacheService.GetCache<int>(keyTotalMaterials);
            if (totalMaterials == null)
            {
                totalMaterials = await _context.Materials.CountAsync();
                await _cacheService.SetCache(keyTotalMaterials, totalMaterials, TimeSpan.FromMinutes(0.5), TimeSpan.FromMinutes(5));
            }

            await _cacheService.SetCache(key, materials, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(5));
            return new ServiceResponse<PaginationResponse<MemorialMaterialBody>>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = new PaginationResponse<MemorialMaterialBody>
                {
                    Count = count,
                    Offset = offset,
                    Total = totalMaterials,
                    Items = materials.Select(m => m.ToMemorialMaterialBody())
                }
            };
        }

        public async Task<HttpStatusCode> Remove(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
                return HttpStatusCode.NoContent;

            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();

            await _cacheService.RemoveCache(GetKey(material.Id.ToString()));
            return HttpStatusCode.NoContent;
        }

        private string GetKey(string identifier) => $"{_prefixKey}{identifier}";
    }
}
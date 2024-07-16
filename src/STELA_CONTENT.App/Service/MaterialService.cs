using System.Net;
using Microsoft.EntityFrameworkCore;
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

        public MaterialService(ContentDbContext context)
        {
            _context = context;
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

            return HttpStatusCode.OK;
        }

        public async Task<ServiceResponse<MemorialMaterialBody>> Get(Guid id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
                return new ServiceResponse<MemorialMaterialBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                };

            return new ServiceResponse<MemorialMaterialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = material.ToMemorialMaterialBody()
            };
        }

        public async Task<ServiceResponse<PaginationResponse<MemorialMaterialBody>>> GetAll(int count, int offset)
        {
            var materials = await _context.Materials.OrderBy(m => m.Name)
                                                    .Skip(offset)
                                                    .Take(count)
                                                    .ToListAsync();

            var totalMaterials = await _context.Materials.CountAsync();
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
            return HttpStatusCode.NoContent;
        }
    }
}
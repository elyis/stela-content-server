using System.Net;
using Microsoft.EntityFrameworkCore;
using STELA_CONTENT.Core.Entities.Models;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.App.Service
{
    public class MemorialService : IMemorialService
    {
        private readonly ContentDbContext _context;

        public MemorialService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<MemorialBody>> Create(CreateMemorialBody body)
        {
            var materials = await _context.Materials.Where(e => body.MaterialIds.Contains(e.Id))
                                                    .ToListAsync();
            if (materials.Count == 0)
                return new ServiceResponse<MemorialBody>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false
                };

            var memorial = new Memorial
            {
                Name = body.Name,
                Description = body.Description,
                StelaHeight = body.StelaHeight,
                StelaWidth = body.StelaWidth,
                StelaLength = body.StelaLength
            };

            var memorialMaterials = materials.Select(e => new MemorialMaterials
            {
                Material = e,
                Memorial = memorial
            });

            await _context.Memorials.AddAsync(memorial);
            await _context.MemorialMaterials.AddRangeAsync(memorialMaterials);
            await _context.SaveChangesAsync();

            return new ServiceResponse<MemorialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = memorial.ToMemorialBody()
            };
        }

        public async Task<ServiceResponse<MemorialBody>> Get(Guid id)
        {
            var memorial = await _context.Memorials.Include(e => e.Materials)
                                                    .FirstOrDefaultAsync(e => e.Id == id);
            if (memorial == null)
                return new ServiceResponse<MemorialBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false
                };

            return new ServiceResponse<MemorialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = memorial.ToMemorialBody()
            };
        }

        public async Task<ServiceResponse<PaginationResponse<ShortMemorialBody>>> GetAll(int count, int offset)
        {
            var memorials = await _context.Memorials.Include(e => e.Materials)
                                                    .OrderBy(m => m.Name)
                                                    .Skip(offset)
                                                    .Take(count)
                                                    .ToListAsync();
            var totalCount = await _context.Memorials.CountAsync();

            return new ServiceResponse<PaginationResponse<ShortMemorialBody>>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = new PaginationResponse<ShortMemorialBody>
                {
                    Total = totalCount,
                    Items = memorials.Select(m => m.ToShortMemorialBody()),
                    Count = count,
                    Offset = offset,
                }
            };
        }
    }
}
using System.Net;
using Microsoft.EntityFrameworkCore;
using STELA_CONTENT.Core.Entities.Models;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.App.Service
{
    public class PortfolioMemorialService : IPortfolioMemorialService
    {
        private readonly ContentDbContext _context;

        public PortfolioMemorialService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<PortfolioMemorialBody>> Create(CreatePortfolioMemorialBody body)
        {
            var materials = await _context.Materials.Where(e => body.MaterialIds.Contains(e.Id))
                                                    .ToListAsync();

            var portfolioMemorial = new PortfolioMemorial
            {
                Name = body.Name,
                Description = body.Description,
                CemeteryName = body.CemeteryName,
            };

            var portfolioMemorialMaterials = materials.Select(e => new PortfolioMemorialMaterials
            {
                Material = e,
                PortfolioMemorial = portfolioMemorial
            });

            await _context.PortfolioMemorials.AddAsync(portfolioMemorial);
            await _context.SaveChangesAsync();

            return new ServiceResponse<PortfolioMemorialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = portfolioMemorial.ToPortfolioMemorialBody()
            };
        }

        public async Task<ServiceResponse<PortfolioMemorialBody>> Get(Guid id)
        {
            var portfolioMemorial = await _context.PortfolioMemorials.Include(e => e.Materials)
                                                                     .FirstOrDefaultAsync(e => e.Id == id);
            if (portfolioMemorial == null)
            {
                return new ServiceResponse<PortfolioMemorialBody>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                };
            }
            return new ServiceResponse<PortfolioMemorialBody>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = portfolioMemorial.ToPortfolioMemorialBody()
            };
        }

        public async Task<ServiceResponse<PaginationResponse<ShortPortfolioMemorialBody>>> GetAll(int count, int offset)
        {
            var portfolioMemorials = await _context.PortfolioMemorials.Include(e => e.Materials)
                                                                      .OrderBy(e => e.Name)
                                                                      .Skip(offset)
                                                                      .Take(count)
                                                                      .ToListAsync();
            var totalCount = await _context.PortfolioMemorials.CountAsync();
            return new ServiceResponse<PaginationResponse<ShortPortfolioMemorialBody>>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Body = new PaginationResponse<ShortPortfolioMemorialBody>
                {
                    Offset = offset,
                    Count = count,
                    Total = totalCount,
                    Items = portfolioMemorials.Select(x => x.ToShortPortfolioMemorialBody())
                }
            };
        }
    }
}
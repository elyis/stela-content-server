using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using Swashbuckle.AspNetCore.Annotations;

namespace STELA_CONTENT.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioMemorialService _portfolioMemorialService;

        public PortfolioController(IPortfolioMemorialService portfolioMemorialService)
        {
            _portfolioMemorialService = portfolioMemorialService;
        }

        [SwaggerOperation("Создать памятник")]
        [SwaggerResponse(200, "Памятник создан", typeof(PortfolioMemorialBody))]
        [SwaggerResponse(400, "Материалы не указаны")]

        [HttpPost("portfolio-memorial"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePortfolioMemorial(CreatePortfolioMemorialBody body)
        {
            var result = await _portfolioMemorialService.Create(body);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Получить список памятников")]
        [SwaggerResponse(200, "Список памятников получен", typeof(PaginationResponse<ShortPortfolioMemorialBody>))]

        [HttpGet("portfolio-memorials")]
        public async Task<IActionResult> GetPortfolioMemorials(
            [FromQuery] int count = 10,
            [FromQuery] int offset = 0)
        {
            var result = await _portfolioMemorialService.GetAll(count, offset);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Получить памятник по идентификатору")]
        [SwaggerResponse(200, Type = typeof(PortfolioMemorialBody))]
        [SwaggerResponse(404, "Памятник не найден")]

        [HttpGet("portfolio-memorial")]

        public async Task<IActionResult> GetPortfolioMemorialById([FromQuery, Required] Guid id)
        {
            var result = await _portfolioMemorialService.Get(id);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }
    }
}
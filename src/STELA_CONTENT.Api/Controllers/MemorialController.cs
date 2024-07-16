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
    public class MemorialController : ControllerBase
    {
        private readonly IMemorialService _memorialService;

        public MemorialController(IMemorialService memorialService)
        {
            _memorialService = memorialService;
        }

        [SwaggerOperation("Создать памятник")]
        [SwaggerResponse(200, "Памятник создан", typeof(MemorialBody))]
        [SwaggerResponse(400, "Материалы не указаны")]

        [HttpPost("memorial"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMemorial(CreateMemorialBody body)
        {
            var result = await _memorialService.Create(body);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Получить список памятников")]
        [SwaggerResponse(200, "Список памятников получен", typeof(PaginationResponse<ShortMemorialBody>))]

        [HttpGet("memorials")]
        public async Task<IActionResult> GetMemorials(
            [FromQuery] int count = 10,
            [FromQuery] int offset = 0)
        {
            var result = await _memorialService.GetAll(count, offset);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Получить памятник по идентификатору")]
        [SwaggerResponse(200, Type = typeof(MemorialBody))]
        [SwaggerResponse(404, "Памятник не найден")]

        [HttpGet("memorial")]

        public async Task<IActionResult> GetMemorialById([FromQuery, Required] Guid id)
        {
            var result = await _memorialService.Get(id);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }
    }
}
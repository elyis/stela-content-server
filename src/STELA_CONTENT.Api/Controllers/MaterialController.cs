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
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [SwaggerOperation("Создать материал")]
        [SwaggerResponse(200)]
        [SwaggerResponse(409)]

        [HttpPost("material"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMaterial(CreateMemorialMaterialBody body)
        {
            var result = await _materialService.Create(body);
            return StatusCode((int)result);
        }

        [SwaggerOperation("Получить материал по идентификатору")]
        [SwaggerResponse(200, Type = typeof(MemorialMaterialBody))]
        [SwaggerResponse(404)]

        [HttpGet("material")]
        public async Task<IActionResult> GetMaterialById(
            [FromQuery, Required] Guid key)
        {
            var result = await _materialService.Get(key);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }


        [SwaggerOperation("Удалить материал")]
        [SwaggerResponse(204)]
        [SwaggerResponse(400)]

        [HttpDelete("material"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMaterial(
            [FromQuery, Required] Guid key)
        {
            var result = await _materialService.Remove(key);
            return StatusCode((int)result);
        }

        [SwaggerOperation("Получить список материалов")]
        [SwaggerResponse(200, Type = typeof(PaginationResponse<MemorialMaterialBody>))]

        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials(
            [FromQuery] int count = 10,
            [FromQuery] int offset = 0)
        {
            var result = await _materialService.GetAll(count, offset);
            if (result.IsSuccess)
                return Ok(result.Body);

            return StatusCode((int)result.StatusCode);
        }
    }
}
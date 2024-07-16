using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using Swashbuckle.AspNetCore.Annotations;

namespace STELA_CONTENT.Api.Controllers
{
    public class AdditionalServiceController : ControllerBase
    {
        private readonly IAdditionalServicesService _additionalServiceService;

        public AdditionalServiceController(IAdditionalServicesService additionalServiceService)
        {
            _additionalServiceService = additionalServiceService;
        }

        [SwaggerOperation("Получить список дополнительных услуг")]
        [SwaggerResponse(200, "OK", typeof(IEnumerable<AdditionalServiceBody>))]

        [HttpGet("additional-services")]
        public async Task<IActionResult> GetAdditionalServices(
            [FromQuery] int count = 10,
            [FromQuery] int offset = 0)
        {
            var result = await _additionalServiceService.GetAdditionalServices(count, offset);
            return Ok(result.Body);
        }

        [SwaggerOperation("Создать дополнительную услугу")]
        [SwaggerResponse(200, Type = typeof(AdditionalServiceBody))]
        [SwaggerResponse(409, "Данная услуга уже существует")]

        [HttpPost("additional-service"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdditionalService(CreateAdditionalServiceBody body)
        {
            var result = await _additionalServiceService.Create(body);
            return result.IsSuccess ? Ok(result.Body) : StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Удалить дополнительную услугу")]
        [SwaggerResponse(204)]
        [SwaggerResponse(404, "Услуга не найдена")]

        [HttpDelete("additional-service"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdditionalService([FromQuery, Required] Guid key)
        {
            var result = await _additionalServiceService.Remove(key);
            return StatusCode((int)result);
        }

        [SwaggerOperation("Получить услугу по имени")]
        [SwaggerResponse(200, Type = typeof(AdditionalServiceBody))]
        [SwaggerResponse(404, "Услуга не найдена")]

        [HttpGet("additional-service/name")]
        public async Task<IActionResult> GetAdditionalServiceByName([FromQuery, Required] string key)
        {
            var result = await _additionalServiceService.GetService(key);
            return result.IsSuccess ? Ok(result.Body) : StatusCode((int)result.StatusCode);
        }

        [SwaggerOperation("Получить услугу по идентификатору")]
        [SwaggerResponse(200, Type = typeof(AdditionalServiceBody))]
        [SwaggerResponse(404, "Услуга не найдена")]

        [HttpGet("additional-service/id")]
        public async Task<IActionResult> GetAdditionalServiceById([FromQuery, Required] Guid key)
        {
            var result = await _additionalServiceService.GetService(key);
            return result.IsSuccess ? Ok(result.Body) : StatusCode((int)result.StatusCode);
        }
    }
}
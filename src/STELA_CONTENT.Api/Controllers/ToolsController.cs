using Microsoft.AspNetCore.Mvc;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.IService;
using Swashbuckle.AspNetCore.Annotations;

namespace STELA_CONTENT.Api.Controllers
{
    [ApiController]
    [Route("api/tools")]
    public class ToolsController : ControllerBase
    {
        private readonly IPlotPriceCalculationService _plotPriceCalculationService;

        public ToolsController(IPlotPriceCalculationService plotPriceCalculationService)
        {
            _plotPriceCalculationService = plotPriceCalculationService;
        }

        [SwaggerOperation("Расчитать стоимость участка")]
        [SwaggerResponse(200, Type = typeof(PlotValuationCheckBody))]
        [SwaggerResponse(400)]

        [HttpPost("calculate-plot-price")]
        public async Task<IActionResult> CalculatePlotPrice(PlotPriceCalculationBody plotPriceCalculationBody)
        {
            var result = await _plotPriceCalculationService.Invoke(plotPriceCalculationBody);
            if (!result.IsSuccess)
                return StatusCode((int)result.StatusCode);

            return Ok(result.Body);
        }
    }
}
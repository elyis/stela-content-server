using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;

namespace STELA_CONTENT.Core.IService
{
    public interface IPlotPriceCalculationService
    {
        Task<ServiceResponse<PlotValuationCheckBody>> Invoke(PlotPriceCalculationBody body);
    }
}
using Microsoft.EntityFrameworkCore;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.Entities.Response;
using STELA_CONTENT.Core.Enums;
using STELA_CONTENT.Core.IService;
using STELA_CONTENT.Infrastructure.Data;

namespace STELA_CONTENT.App.Service
{
    public class PlotPriceCalculationService : IPlotPriceCalculationService
    {
        private readonly ContentDbContext _context;

        public PlotPriceCalculationService(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<PlotValuationCheckBody>> Invoke(PlotPriceCalculationBody body)
        {
            var nameInLowercase = body.AdditionalService.ToLower();
            var additionalService = await _context.AdditionalServices.FirstOrDefaultAsync(x => x.Name.ToLower() == nameInLowercase);
            if (additionalService == null)
                return new ServiceResponse<PlotValuationCheckBody>
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };


            var valuationCheck = new PlotValuationCheckBody
            {
                PlotSizePrice = body.PlotSize.GetCost(),
                GraniteColorPrice = body.GraniteColor.GetCost(),
                AdditionalServicePrice = additionalService.Price
            };

            valuationCheck.TotalPrice = valuationCheck.PlotSizePrice
                + valuationCheck.GraniteColorPrice
                + valuationCheck.AdditionalServicePrice;

            return new ServiceResponse<PlotValuationCheckBody>
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Body = valuationCheck
            };
        }
    }
}
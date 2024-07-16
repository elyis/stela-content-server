using System.ComponentModel.DataAnnotations;
using STELA_CONTENT.Core.Enums;

namespace STELA_CONTENT.Core.Entities.Request
{
    public class PlotPriceCalculationBody
    {
        [Required]
        public string AdditionalService { get; set; }

        [EnumDataType(typeof(GraniteColor))]
        public GraniteColor GraniteColor { get; set; }

        [EnumDataType(typeof(PlotSize))]
        public PlotSize PlotSize { get; set; }
    }
}
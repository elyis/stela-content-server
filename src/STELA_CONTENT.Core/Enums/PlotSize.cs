using System.ComponentModel;

namespace STELA_CONTENT.Core.Enums
{
    public enum PlotSize
    {
        [Description("0.8*1.1 м")]
        [PlotSizeCost(1000)]
        Size0_8x1_1,

        [Description("1.0*2.0 м")]
        [PlotSizeCost(2000)]
        Size1_0x2_0,

        [Description("1.8*2.0 м")]
        [PlotSizeCost(3000)]
        Size1_8x2_0,

        [Description("2.0*2.0 м")]
        [PlotSizeCost(4000)]
        Size2_0x2_0,

        [Description("2.0*2.5 м")]
        [PlotSizeCost(5000)]
        Size2_0x2_5,

        [Description("Введите другой размер")]
        [PlotSizeCost(0)]
        Other
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class PlotSizeCostAttribute : Attribute
    {
        public float Cost { get; }

        public PlotSizeCostAttribute(float cost)
        {
            Cost = cost;
        }
    }

    public static class PlotSizeExtensions
    {
        public static float GetCost(this PlotSize plotSize)
        {
            var type = plotSize.GetType();
            var memberInfo = type.GetMember(plotSize.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(PlotSizeCostAttribute), false);
            return attributes.Length > 0 ? ((PlotSizeCostAttribute)attributes[0]).Cost : 0;
        }
    }
}
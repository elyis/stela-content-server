using System.Text.Json.Serialization;

namespace STELA_CONTENT.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GraniteColor
    {
        [GraniteColorCost(1000)]
        White,

        [GraniteColorCost(1000)]
        Black,

        [GraniteColorCost(1000)]
        Grey,

        [GraniteColorCost(1000)]
        Other
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class GraniteColorCostAttribute : Attribute
    {
        public float Cost { get; }

        public GraniteColorCostAttribute(float cost)
        {
            Cost = cost;
        }
    }

    public static class GraniteColorExtensions
    {
        public static float GetCost(this GraniteColor graniteColor)
        {
            var type = graniteColor.GetType();
            var memberInfo = type.GetMember(graniteColor.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(GraniteColorCostAttribute), false);
            return attributes.Length > 0 ? ((GraniteColorCostAttribute)attributes[0]).Cost : 0;
        }
    }
}
using System.Text.Json.Serialization;

namespace STELA_CONTENT.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountRole
    {
        User,
        Admin
    }
}
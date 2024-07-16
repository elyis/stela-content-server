using STELA_CONTENT.Core.Entities.Request;

namespace STELA_CONTENT.Core.IService
{
    public interface IJwtService
    {
        TokenPayload GetTokenPayload(string token);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using STELA_CONTENT.Core.Entities.Request;
using STELA_CONTENT.Core.IService;

namespace STELA_CONTENT.App.Service
{
    public class JwtService : IJwtService
    {
        private List<Claim> GetClaims(string token) =>
            new JwtSecurityTokenHandler()
                .ReadJwtToken(token.Replace("Bearer ", ""))
                .Claims
                .ToList();

        public TokenPayload GetTokenPayload(string token)
        {
            var claims = GetClaims(token);
            return new TokenPayload
            {
                Role = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value,
                AccountId = Guid.Parse(claims.FirstOrDefault(claim => claim.Type == "AccountId")?.Value),
            };
        }
    }
}
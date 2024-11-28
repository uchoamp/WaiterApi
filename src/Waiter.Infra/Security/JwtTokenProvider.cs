using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Waiter.Application.Models.Common;
using Waiter.Application.Security;
using Waiter.Domain.Constants;

namespace Waiter.Infra.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private static readonly TimeSpan s_expirationInterval = TimeSpan.FromMinutes(60);
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenProvider(IConfiguration configuration)
        {
            var key = configuration[ApplicationSettings.JwtKey];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
        }

        public AccessTokenResponse CreateAcessTokenAsync(Guid userId, string?[] roles)
        {
            var issuedAt = DateTime.UtcNow;
            var expires = issuedAt + s_expirationInterval;

            var claims = new List<Claim>();

            claims.Add(new Claim("sub", userId.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role!));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = issuedAt,
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = _signingCredentials
            };

            var token = new JwtSecurityTokenHandler().CreateEncodedJwt(tokenDescriptor);

            return new AccessTokenResponse(token, expires);
        }
    }
}

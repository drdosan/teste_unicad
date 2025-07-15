using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Raizen.UniCad.Web.Util
{
    public class Jwt
    {
        public static string GenerateToken(Dictionary<string, string> claims, string secret, TimeSpan? expire = null)
        {
            expire = expire ?? TimeSpan.FromDays(300); // Senão for informado, não expira
            var symmetricKey = Convert.FromBase64String(secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value)).ToArray()
            ),

                Expires = now.Add(expire.Value),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static IDictionary<string, string> GetClaims(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null)
            {
                throw new ArgumentException();
            }
            var symmetricKey = Convert.FromBase64String(secret);

            var validationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
            };
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                throw new InvalidOperationException();
            }
            return identity.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
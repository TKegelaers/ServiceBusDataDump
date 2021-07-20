using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace JWTToken
{
    public static class JsonWebTokenHandler
    {
        /// <summary>
        /// Creates a JWT client assertion used for authenticating an OAuth client.
        /// </summary>
        /// <param name="certificate">Certificate that includes both public and private key.</param>
        /// <param name="clientId">Your OAuth client id, this can be found here: https://beta.oauth.vlaanderen.be/admin/OAuthClients.</param>
        /// <param name="endpoint">e.g. https://beta.oauth.vlaanderen.be/authorization/ws/oauth/v2/token</param>
        /// <returns>JWT client assertion</returns>
        public static string CreateJwtClientAssertion(X509Certificate2 certificate, string clientId, string endpoint)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(960),
                SigningCredentials = new SigningCredentials(new X509SecurityKey(certificate), SecurityAlgorithms.RsaSha256Signature),
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim("sub", clientId),
                    new Claim("iss", clientId),
                    new Claim("jti", Guid.NewGuid().ToString()),
                    new Claim("aud", endpoint)
                })
            };

            return tokenHandler.WriteToken(tokenHandler.CreateJwtSecurityToken(tokenDescriptor));
        }
    }
}

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cerberus.Api.Services
{
    /// <summary>
    /// JWT security token generator implementing ISecurityTokenGenerator interface.
    /// </summary>
    /// <param name="jwtConfigurationSectionService">The service to retrieve jwt configuration section</param>
    public class JwtSecurityTokenGenerator(IJwtConfigurationSectionService jwtConfigurationSectionService) : ISecurityTokenGenerator
    {
        /// <summary>
        /// Generates a JWT security token based on the JWT configuration key and issuer.
        /// </summary>
        /// <param name="username">Logging in user's name</param>
        /// <returns>A JWT token issued by the configuration issuer, expiring in 5 minutes, signed with the configuration key.</returns>
        public string GenerateSecurityToken(string username)
        {
            var jwtConfigurationSection = jwtConfigurationSectionService.GetJwtConfigurationSection();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurationSection.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, username) };

            var jwtSecurityToken = new JwtSecurityToken(
                jwtConfigurationSection.Issuer,
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials,
                claims: claims
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return jwt;
        }
    }
}

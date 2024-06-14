using Cerberus.Api.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cerberus.Api.Generators
{
    /// <summary>
    /// JWT security token generator implementing ISecurityTokenGenerator interface.
    /// </summary>
    /// <param name="configuration">The API configuration parameters.</param>
    public class JwtSecurityTokenGenerator(IConfiguration configuration) : ISecurityTokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Generates a JWT security token based on the JWT configuration key and issuer.
        /// </summary>
        /// <param name="userName">Logging in user's name</param>
        /// <returns>A JWT token issued by the configuration issuer, expiring in 5 minutes, signed with the configuration key.</returns>
        public string GenerateSecurityToken(string userName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[JwtConfigurationPropertyNames.Key]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userName) };

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration[JwtConfigurationPropertyNames.Issuer],
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

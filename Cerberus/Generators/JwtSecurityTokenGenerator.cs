using Cerberus.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Cerberus.Generators
{
    public class JwtSecurityTokenGenerator(IConfiguration configuration) : ISecurityTokenGenerator
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateSecurityToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[JwtConfigurationPropertyNames.Key]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration[JwtConfigurationPropertyNames.Issuer],
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return jwt;
        }
    }
}

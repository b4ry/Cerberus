using Cerberus.Api.Constants;
using Cerberus.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tests.Services
{
    public class JwtSecurityTokenGeneratorTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private const string secretIssuer = "secretIssuer.com";

        public JwtSecurityTokenGeneratorTests()
        {
            _configuration = new Mock<IConfiguration>();        
        }

        [Fact]
        public void GenerateSecurityToken_ShouldReturnJwtTokenWithCorrectProperties()
        {
            // Arrange
            _configuration.SetupGet(config => config[JwtConfigurationPropertyNames.Key]).Returns("TestSecurityKeyForAuthentication");
            _configuration.SetupGet(config => config[JwtConfigurationPropertyNames.Issuer]).Returns(secretIssuer);

            var tokenGenerator = new JwtSecurityTokenGenerator(_configuration.Object);

            // Act
            var token = tokenGenerator.GenerateSecurityToken("testUserName");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(securityToken);
            Assert.Equal(secretIssuer, securityToken.Issuer);
            Assert.NotNull(securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier));
            Assert.True(securityToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateSecurityToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var secretKey = "TestSecurityKeyForAuthentication";

            _configuration.SetupGet(config => config[JwtConfigurationPropertyNames.Key]).Returns(secretKey);
            _configuration.SetupGet(config => config[JwtConfigurationPropertyNames.Issuer]).Returns(secretIssuer);

            var tokenGenerator = new JwtSecurityTokenGenerator(_configuration.Object);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidIssuer = secretIssuer,
                IssuerSigningKey = securityKey
            };

            // Act
            var token = tokenGenerator.GenerateSecurityToken("testUserName");

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            Assert.NotNull(securityToken);
            Assert.Equal(signingCredentials.Key.GetHashCode(), securityToken.SigningKey.GetHashCode());
        }
    }
}
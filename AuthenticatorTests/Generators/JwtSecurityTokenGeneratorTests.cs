using Authenticator.Constants;
using Authenticator.Generators;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Tests.Generators
{
    public class JwtSecurityTokenGeneratorTests
    {
        [Fact]
        public void GenerateSecurityToken_ShouldReturnJwtTokenWithCorrectProperties()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var secretIssuer = "secretIssuer.com";

            mockConfiguration.SetupGet(config => config[JwtConfigurationPropertyNames.Key]).Returns("TestSecurityKeyForAuthentication");
            mockConfiguration.SetupGet(config => config[JwtConfigurationPropertyNames.Issuer]).Returns(secretIssuer);

            var tokenGenerator = new JwtSecurityTokenGenerator(mockConfiguration.Object);

            // Act
            var token = tokenGenerator.GenerateSecurityToken();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(securityToken);
            Assert.Equal(secretIssuer, securityToken.Issuer);
            Assert.True(securityToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateSecurityToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            var secretIssuer = "secretIssuer.com";
            var secretKey = "TestSecurityKeyForAuthentication";

            mockConfiguration.SetupGet(config => config[JwtConfigurationPropertyNames.Key]).Returns(secretKey);
            mockConfiguration.SetupGet(config => config[JwtConfigurationPropertyNames.Issuer]).Returns(secretIssuer);

            var tokenGenerator = new JwtSecurityTokenGenerator(mockConfiguration.Object);

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
            var token = tokenGenerator.GenerateSecurityToken();

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
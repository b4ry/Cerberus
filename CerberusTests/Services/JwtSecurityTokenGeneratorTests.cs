using Cerberus.Api.ConfigurationSections;
using Cerberus.Api.Services;
using Cerberus.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Tests.Services
{
    public class JwtSecurityTokenGeneratorTests
    {
        private readonly Mock<IJwtConfigurationSectionService> _jwtConfigurationSectionService;
        private const string secretIssuer = "secretIssuer.com";
        private const string secretKey = "TestSecurityKeyForAuthentication";

        public JwtSecurityTokenGeneratorTests()
        {
            _jwtConfigurationSectionService = new Mock<IJwtConfigurationSectionService>();        
        }

        [Fact]
        public void GenerateSecurityToken_ShouldReturnJwtAndRefreshTokensWithCorrectProperties()
        {
            // Arrange
            _jwtConfigurationSectionService.Setup(x => x.GetJwtConfigurationSection()).Returns(new JwtConfigurationSection
            {
                Key = secretKey,
                Issuer = secretIssuer
            });

            var tokenGenerator = new JwtSecurityTokenGenerator(_jwtConfigurationSectionService.Object);

            // Act
            var authToken = tokenGenerator.GenerateSecurityToken("testUserName");

            // Assert
            Assert.NotNull(authToken.Jwt);
            Assert.NotNull(authToken.RefreshToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(authToken.Jwt) as JwtSecurityToken;

            Assert.NotNull(securityToken);
            Assert.NotNull(authToken.RefreshToken);
            Assert.Equal(secretIssuer, securityToken.Issuer);
            Assert.NotNull(securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier));
            Assert.True(securityToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateSecurityToken_ShouldReturnValidJwtAndRefreshTokens()
        {
            // Arrange
            _jwtConfigurationSectionService.Setup(x => x.GetJwtConfigurationSection()).Returns(new JwtConfigurationSection
            {
                Key = secretKey,
                Issuer = secretIssuer
            });

            var tokenGenerator = new JwtSecurityTokenGenerator(_jwtConfigurationSectionService.Object);

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
            var authToken = tokenGenerator.GenerateSecurityToken("testUserName");

            // Assert
            Assert.NotNull(authToken.Jwt);
            Assert.NotNull(authToken.RefreshToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(authToken.Jwt, tokenValidationParameters, out var securityToken);

            Assert.NotNull(securityToken);
            Assert.Equal(signingCredentials.Key.GetHashCode(), securityToken.SigningKey.GetHashCode());
        }
    }
}
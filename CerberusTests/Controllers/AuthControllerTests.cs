using Cerberus.Api.Controllers;
using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<ILogger<AuthController>> _logger;
        private readonly Mock<ISecurityTokenGenerator> _securityTokenGenerator;

        public AuthControllerTests()
        {
            _authService = new Mock<IAuthService>();
            _logger = new Mock<ILogger<AuthController>>();
            _securityTokenGenerator = new Mock<ISecurityTokenGenerator>();
            _controller = new AuthController(_logger.Object, _securityTokenGenerator.Object, _authService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnAccessAndRefreshTokens_WhenValidRequestAndUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            var accessToken = "testAccessToken";
            var refreshToken = "test";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityTokenAsync(request.Username, true)).Returns(Task.FromResult(new AuthToken(accessToken, refreshToken)));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authTokenResult = Assert.IsType<AuthToken>(okResult.Value);
            Assert.Equal(accessToken, authTokenResult.AccessToken);
            Assert.Equal(refreshToken, authTokenResult.RefreshToken);
            _authService.Verify(x => x.RegisterUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityTokenAsync(request.Username, true), Times.Once);
        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenValidRequestAndUserExists()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            _authService.Setup(x => x.RegisterUserAsync(request)).ThrowsAsync(new Exception());

            // Act
            await Assert.ThrowsAnyAsync<Exception>(() => _controller.Register(request));
            _authService.Verify(x => x.RegisterUserAsync(request), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenExceptionOccurs()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            _authService.Setup(x => x.LoginUserAsync(request)).ThrowsAsync(new Exception());

            // Act
            await Assert.ThrowsAnyAsync<Exception>(() => _controller.Login(request));
            _authService.Verify(x => x.LoginUserAsync(request), Times.Once);
        }

        [Fact]
        public async void Login_ShouldReturnAccessAndRefreshTokens_WhenValidRequestAndSuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var accessToken = "testAccessToken";
            var refreshToken = "test";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityTokenAsync(request.Username, true)).Returns(Task.FromResult(new AuthToken(accessToken, refreshToken)));
            _authService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authToken = Assert.IsType<AuthToken>(okResult.Value);
            Assert.Equal(accessToken, authToken.AccessToken);
            Assert.Equal(refreshToken, authToken.RefreshToken);
            _authService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityTokenAsync(request.Username, true), Times.Once);
        }

        [Fact]
        public async void Login_ShouldReturnUnauthorized_WhenValidRequestAndUnsuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var accessToken = "testAccessToken";
            var refreshToken = "test";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityTokenAsync(request.Username, false)).Returns(Task.FromResult(new AuthToken(accessToken, refreshToken)));
            _authService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _authService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityTokenAsync(request.Username, false), Times.Never);
        }

        [Fact]
        public async void RefreshToken_ShouldReturnUnauthorized_WhenValidRequestAndRefreshTokenDoesNotExist()
        {
            // Arrange
            var refreshTokenId = "testId";
            var username = "testUser";
            var refreshTokenRequest = new RefreshTokenRequest() { RefreshTokenId = refreshTokenId, Username = username };

            _authService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest)).ReturnsAsync((AuthToken?)null);

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _authService.Verify(x => x.RefreshTokenAsync(refreshTokenRequest), Times.Once);
        }

        [Fact]
        public async void RefreshToken_ShouldReturnAuthToken_WhenValidRequestAndRefreshTokenExists()
        {
            // Arrange
            var refreshTokenId = "testId";
            var username = "testUser";
            var accessToken = "testAccessToken";
            var refreshTokenRequest = new RefreshTokenRequest() { RefreshTokenId = refreshTokenId, Username = username };
            var authToken = new AuthToken(accessToken, refreshTokenId);

            _authService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest)).ReturnsAsync(authToken);

            // Act
            var result = await _controller.RefreshToken(refreshTokenRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var authTokenResult = Assert.IsType<AuthToken>(okResult.Value);
            Assert.Equal(refreshTokenId, authTokenResult.RefreshToken);
            Assert.Equal(accessToken, authTokenResult.AccessToken);
            _authService.Verify(x => x.RefreshTokenAsync(refreshTokenRequest), Times.Once);
        }
    }
}
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
        public async Task Register_ShouldReturnNoContent_WhenValidRequestAndUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _authService.Verify(x => x.RegisterUserAsync(request), Times.Once);
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
        public async void Login_ShouldReturnJWT_WhenValidRequestAndSuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var jwtToken = "testJWT";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityToken(request.Username)).Returns(jwtToken);
            _authService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("testJWT", jwtToken);
            _authService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityToken(request.Username), Times.Once);
        }

        [Fact]
        public async void Login_ShouldReturnUnauthorized_WhenValidRequestAndUnsuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var jwtToken = "testJWT";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityToken(request.Username)).Returns(jwtToken);
            _authService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _authService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityToken(request.Username), Times.Never);
        }
    }
}
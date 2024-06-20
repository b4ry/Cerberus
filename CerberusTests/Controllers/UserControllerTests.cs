using Cerberus.Api.Controllers;
using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Controllers
{
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<IUserRegistrationService> _mockRegistrationService;
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<ISecurityTokenGenerator> _securityTokenGenerator;

        public UserControllerTests()
        {
            _mockRegistrationService = new Mock<IUserRegistrationService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _securityTokenGenerator = new Mock<ISecurityTokenGenerator>();
            _controller = new UserController(_mockLogger.Object, _securityTokenGenerator.Object, _mockRegistrationService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnNoContent_WhenValidRequestAndUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            _mockRegistrationService.Setup(svc => svc.RegisterUserAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRegistrationService.Verify(svc => svc.RegisterUserAsync(request), Times.Once);
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenValidRequestAndUserExists()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            _mockRegistrationService.Setup(svc => svc.RegisterUserAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
            _mockRegistrationService.Verify(svc => svc.RegisterUserAsync(request), Times.Once);
        }

        [Fact]
        public void Login_ShouldReturnJWT_WhenValidRequestAndSuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var jwtToken = "testJWT";

            _securityTokenGenerator.Setup(svc => svc.GenerateSecurityToken(request.Username)).Returns(jwtToken);

            // Act
            var result = _controller.Login(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("testJWT", jwtToken);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityToken(request.Username), Times.Once);
        }
    }
}
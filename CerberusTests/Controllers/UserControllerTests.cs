using Cerberus.Api.Controllers;
using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<ILogger<UserController>> _logger;
        private readonly Mock<ISecurityTokenGenerator> _securityTokenGenerator;

        public UserControllerTests()
        {
            _userService = new Mock<IUserService>();
            _logger = new Mock<ILogger<UserController>>();
            _securityTokenGenerator = new Mock<ISecurityTokenGenerator>();
            _controller = new UserController(_logger.Object, _securityTokenGenerator.Object, _userService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnNoContent_WhenValidRequestAndUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            _userService.Setup(x => x.RegisterUserAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userService.Verify(x => x.RegisterUserAsync(request), Times.Once);
        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenValidRequestAndUserExists()
        {
            // Arrange
            var request = new RegisterRequest("testUsername", "testPassword");
            _userService.Setup(x => x.RegisterUserAsync(request)).ThrowsAsync(new Exception());

            // Act
            await Assert.ThrowsAnyAsync<Exception>(() => _controller.Register(request));
            _userService.Verify(x => x.RegisterUserAsync(request), Times.Once);
        }

        [Fact]
        public async void Login_ShouldReturnJWT_WhenValidRequestAndSuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var jwtToken = "testJWT";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityToken(request.Username)).Returns(jwtToken);
            _userService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(true);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("testJWT", jwtToken);
            _userService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityToken(request.Username), Times.Once);
        }

        [Fact]
        public async void Login_ShouldReturnUnauthorized_WhenValidRequestAndUnsuccessfulLogin()
        {
            // Arrange
            var request = new LoginRequest("testUsername", "testPassword");
            var jwtToken = "testJWT";

            _securityTokenGenerator.Setup(x => x.GenerateSecurityToken(request.Username)).Returns(jwtToken);
            _userService.Setup(x => x.LoginUserAsync(request)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            _userService.Verify(x => x.LoginUserAsync(request), Times.Once);
            _securityTokenGenerator.Verify(svc => svc.GenerateSecurityToken(request.Username), Times.Never);
        }
    }
}
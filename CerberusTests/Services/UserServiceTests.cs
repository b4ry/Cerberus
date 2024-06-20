using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(true));
            _userRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1));

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await _userService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldNotRegisterUser_WhenSuchUserExists()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(false));
            _userRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await _userService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

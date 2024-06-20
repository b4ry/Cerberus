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
        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(true));
            userRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1));

            var logger = new Mock<ILogger<UserService>>();

            var registerUserService = new UserService(userRepository.Object, logger.Object);
            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await registerUserService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldNotRegisterUser_WhenSuchUserExists()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(false));
            userRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));

            var logger = new Mock<ILogger<UserService>>();

            var registerUserService = new UserService(userRepository.Object, logger.Object);
            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await registerUserService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

using Cerberus.Api.DTOs;
using Cerberus.Api.Repositories;
using Cerberus.Api.Services;
using Moq;

namespace Tests.Services
{
    public class UserRegistrationServiceTests
    {
        [Fact]
        public async Task RegisterUser_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<RegisterRequest>())).Returns(Task.FromResult(true));

            var registerUserService = new UserRegistrationService(userRepository.Object);
            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await registerUserService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RegisterUser_ShouldNotRegisterUser_WhenSuchUserExists()
        {
            // Arrange
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<RegisterRequest>())).Returns(Task.FromResult(false));

            var registerUserService = new UserRegistrationService(userRepository.Object);
            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await registerUserService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

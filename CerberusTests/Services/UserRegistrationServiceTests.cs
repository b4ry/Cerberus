using Castle.Core.Logging;
using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.UnitOfWork;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Services
{
    public class UserRegistrationServiceTests
    {
        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1));

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(true));

            var logger = new Mock<ILogger<UserRegistrationService>>();

            var registerUserService = new UserRegistrationService(userRepository.Object, unitOfWork.Object, logger.Object);
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
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(0));

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(false));

            var logger = new Mock<ILogger<UserRegistrationService>>();

            var registerUserService = new UserRegistrationService(userRepository.Object, unitOfWork.Object, logger.Object);
            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await registerUserService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

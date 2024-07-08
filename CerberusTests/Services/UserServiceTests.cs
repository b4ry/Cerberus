using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPasswordService> _passwordService;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordService = new Mock<IPasswordService>();
            _userService = new UserService(_userRepository.Object, _passwordService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(true));

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

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await _userService.RegisterUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRethrowDbUpdateException_WhenUserExistsAndReadded()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).ThrowsAsync(new DbUpdateException());

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act && Assert
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => _userService.RegisterUserAsync(registerRequest));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRethrowGeneralException()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).ThrowsAsync(new Exception());

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act && Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _userService.RegisterUserAsync(registerRequest));
        }

        [Fact]
        public async Task LoginUserAsync_ShouldLogsInUser_WhenSuchUserExistsAndCredentialsAreCorrect()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");
            var userEntity = new UserEntity() { Username = "testUser", Password = "testPassword" };

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(userEntity));

            // Act
            var result = await _userService.LoginUserAsync(registerRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldNotLogInUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(new Mock<UserEntity>().Object));

            // Act
            var result = await _userService.LoginUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldNotLogInUser_WhenSuchUserExistsAndInvalidCredentials()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");
            var userEntity = new UserEntity() { Username = "testUser", Password = "testPassword1" };

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(userEntity));

            // Act
            var result = await _userService.LoginUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

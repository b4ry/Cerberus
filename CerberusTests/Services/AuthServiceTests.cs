using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPasswordService> _passwordService;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("testPassword");

            _authService = new AuthService(_userRepository.Object, _passwordService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.FromResult(true));

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = Record.ExceptionAsync(() => _authService.RegisterUserAsync(registerRequest));

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Exception);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRethrowDbUpdateException_WhenAddingAlreadyExistingUser()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).ThrowsAsync(new DbUpdateException());

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act & Assert
            await Assert.ThrowsAnyAsync<DbUpdateException>(() => _authService.RegisterUserAsync(registerRequest));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRethrowGeneralException()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).ThrowsAsync(new Exception());

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _authService.RegisterUserAsync(registerRequest));
        }

        [Fact]
        public async Task LoginUserAsync_ShouldLogsInUser_WhenSuchUserExistsAndCredentialsAreCorrect()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");
            var userEntity = new UserEntity()
            {
                Username = "testUser",
                Password = "testPassword",
                Salt = "testSalt"
            };

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(userEntity)!);

            // Act
            var result = await _authService.LoginUserAsync(registerRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldNotLogInUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(new Mock<UserEntity>().Object)!);

            // Act
            var result = await _authService.LoginUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldNotLogInUser_WhenSuchUserExistsAndInvalidCredentials()
        {
            // Arrange
            var registerRequest = new LoginRequest("testUser", "testPassword");
            var userEntity = new UserEntity()
            {
                Username = "testUser",
                Password = "testPassword1",
                Salt = "testSalt"
            };

            _userRepository.Setup(x => x.FindAsync(It.IsAny<string>())).Returns(Task.FromResult(userEntity)!);

            // Act
            var result = await _authService.LoginUserAsync(registerRequest);

            // Assert
            Assert.False(result);
        }
    }
}

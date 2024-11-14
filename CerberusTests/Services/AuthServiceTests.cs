using Cerberus.Api.DTOs;
using Cerberus.Api.Services;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private readonly Mock<IPasswordService> _passwordService;
        private readonly Mock<ISecurityTokenGenerator> _securityTokenGenerator;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _securityTokenGenerator = new Mock<ISecurityTokenGenerator>();
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("testPassword");

            _authService = new AuthService(_userRepository.Object, _refreshTokenRepository.Object, _passwordService.Object, _securityTokenGenerator.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenSuchUserDoesNotExist()
        {
            // Arrange
            _userRepository.Setup(x => x.AddAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);

            var registerRequest = new RegisterRequest("testUser", "testPassword");

            // Act
            var result = await Record.ExceptionAsync(() => _authService.RegisterUserAsync(registerRequest));

            // Assert
            Assert.Null(result);
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

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenDoesNotExist()
        {
            // Arrange
            var refreshTokenId = "testId";
            var refreshTokenRequest = new RefreshTokenRequest() { RefreshTokenId = refreshTokenId, Username = "testUser" };

            _refreshTokenRepository.Setup(x => x.FindAsync(refreshTokenId)).ReturnsAsync((RefreshTokenEntity?)null);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenExpired()
        {
            // Arrange
            var refreshTokenId = "testId";
            var refreshTokenRequest = new RefreshTokenRequest() { RefreshTokenId = refreshTokenId, Username = "testUser" };
            var refreshToken = new RefreshTokenEntity() { Id = refreshTokenId, ValidUntil = DateTime.UtcNow.AddDays(-1) };

            _refreshTokenRepository.Setup(x => x.FindAsync(refreshTokenId)).ReturnsAsync(refreshToken);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnAuthToken_WhenUnexpiredRefreshTokenExists()
        {
            // Arrange
            var refreshTokenId = "testId";
            var username = "testUser";
            var accessToken = "testAccessToken";
            var refreshTokenRequest = new RefreshTokenRequest() { RefreshTokenId = refreshTokenId, Username = username };
            var refreshToken = new RefreshTokenEntity() { Id = refreshTokenId, ValidUntil = DateTime.UtcNow.AddDays(1) };
            var authToken = new AuthToken(accessToken, null);

            _refreshTokenRepository.Setup(x => x.FindAsync(refreshTokenId)).ReturnsAsync(refreshToken);
            _securityTokenGenerator.Setup(x => x.GenerateSecurityTokenAsync(username, false)).ReturnsAsync(authToken);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.Equal(accessToken, result.AccessToken);
            Assert.Equal(refreshTokenId, result.RefreshToken);
        }
    }
}

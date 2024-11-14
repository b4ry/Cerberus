using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;

namespace Cerberus.Api.Services
{
    /// <summary>
    /// Handles users' authentication and authorization logic.
    /// </summary>
    /// <param name="userRepository">User repository handling user entity</param>
    /// <param name="refreshTokenRepository">Refresh token repository</param>
    /// <param name="passwordService">Password service handling password related logic like hashing, salting</param>
    /// <param name="securityTokenGenerator">Service generating a security token</param>
    public class AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ISecurityTokenGenerator securityTokenGenerator) : IAuthService
    {
        /// <summary>
        /// Registers users. If a user already exists, then the underlying exception, enriched with some additional information, is being re-thrown.
        /// </summary>
        /// <param name="registerRequest">Request for user registration</param>
        public async Task RegisterUserAsync(RegisterRequest registerRequest)
        {
            try
            {
                var salt = passwordService.GenerateSalt();
                var hashedPassword = passwordService.HashPassword(registerRequest.Password, salt);

                var userEntity = new UserEntity()
                {
                    Username = registerRequest.Username,
                    Password = hashedPassword,
                    Salt = salt
                };

                await userRepository.AddAsync(userEntity);
            }
            catch (Exception ex)
            {
                ex.Data.Add("RegisterUser", $"Error occured while registering user: {registerRequest.Username}");

                throw;
            }
        }

        /// <summary>
        /// Logins users validating with a salt technique.
        /// </summary>
        /// <param name="loginRequest">Request for login a user</param>
        /// <returns>
        ///     True when successful,
        ///     False when fails
        /// </returns>
        public async Task<bool> LoginUserAsync(LoginRequest loginRequest)
        {
            var user = await userRepository.FindAsync(loginRequest.Username);

            if (user != null)
            {
                var hash = passwordService.HashPassword(loginRequest.Password, user.Salt);

                if (hash == user.Password)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Refreshes token, generating a new access token when the refresh token is still valid. Otherwise, returns null.
        /// </summary>
        /// <param name="refreshTokenRequest">Request to refresh a token</param>
        /// <returns>
        ///     AuthToken when successful,
        ///     Null when fails
        /// </returns>
        public async Task<AuthToken?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = await refreshTokenRepository.FindAsync(refreshTokenRequest.RefreshTokenId);

            if(refreshToken == null || refreshToken.ValidUntil < DateTime.UtcNow)
            {
                return null;
            }

            var authToken = await securityTokenGenerator.GenerateSecurityTokenAsync(refreshTokenRequest.Username);

            return new AuthToken(authToken.AccessToken, refreshTokenRequest.RefreshTokenId);
        }
    }
}

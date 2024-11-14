using Cerberus.Api.DTOs;

namespace Cerberus.Api.Services.Interfaces
{
    /// <summary>
    /// Interface handles users' authentication and authorization logic.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers users. If a user already exists, then the underlying exception, enriched with some additional information, is being re-thrown.
        /// </summary>
        /// <param name="registerRequest">Request for user registration</param>
        public Task RegisterUserAsync(RegisterRequest registerRequest);

        /// <summary>
        /// Logins users validating with a salt technique.
        /// </summary>
        /// <param name="loginRequest">Request to login a user</param>
        /// <returns>
        ///     True when successful,
        ///     False when fails
        /// </returns>
        public Task<bool> LoginUserAsync(LoginRequest loginRequest);

        /// <summary>
        /// Refreshes token, generating a new access token when the refresh token is still valid. Otherwise, returns null.
        /// </summary>
        /// <param name="refreshTokenRequest">Request to refresh a token</param>
        /// <returns>
        ///     AuthToken when successful,
        ///     Null when fails
        /// </returns>
        public Task<AuthToken?> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}

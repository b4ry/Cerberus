using Cerberus.Api.DTOs;

namespace Cerberus.Api.Services.Interfaces
{
    /// <summary>
    /// Security token generators' interface. It has to be implemented by all the security token generators.
    /// </summary>
    public interface ISecurityTokenGenerator
    {
        /// <summary>
        /// Generates a security token, eg. jwt.
        /// </summary>
        /// <param name="userName">Logging in user's name</param>
        /// <param name="generateRefreshToken">Flag to control refresh token generation</param>
        /// <returns>A security token. AuthToken object.</returns>
        public Task<AuthToken> GenerateSecurityTokenAsync(string userName, bool generateRefreshToken = false);
    }
}

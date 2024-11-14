namespace Cerberus.Api.DTOs
{
    /// <summary>
    /// Auth token containing access and refresh tokens
    /// </summary>
    public sealed record AuthToken
    {
        /// <summary>
        /// Authorization token constructor.
        /// Accepts two string parameters: AccessToken and RefreshToken
        /// </summary>
        /// <param name="accessToken"> Access token. String. </param>
        /// <param name="refreshToken"> Refresh token. Nullable, String. </param>
        public AuthToken(string accessToken, string? refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Access token. String.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token. Nullable, String.
        /// </summary>
        public string? RefreshToken { get; set; }
    }
}

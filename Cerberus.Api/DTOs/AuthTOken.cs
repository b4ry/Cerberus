namespace Cerberus.Api.DTOs
{
    /// <summary>
    /// Auth token containing jwt access and refresh tokens
    /// </summary>
    public sealed record AuthToken
    {
        /// <summary>
        /// Authorization token constructor.
        /// Accepts two string parameters: jwt and refreshToken
        /// </summary>
        /// <param name="jwt"> Encoded jwt access token. String. </param>
        /// <param name="refreshToken"> Refresh token. String. </param>
        public AuthToken(string jwt, string refreshToken)
        {
            Jwt = jwt;
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Jwt access token. String.
        /// </summary>
        public string Jwt { get; set; }

        /// <summary>
        /// Refresh token. String.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}

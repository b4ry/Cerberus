using System.ComponentModel.DataAnnotations;

namespace Cerberus.Api.DTOs
{
    /// <summary>
    /// Refresh token request encapsulating RefreshTokenId and Username fields.
    /// </summary>
    public sealed record RefreshTokenRequest
    {
        /// <summary>
        /// Refresh token id. String.
        /// </summary>
        [Required]
        public required string RefreshTokenId { get; set; }

        /// <summary>
        /// User's name. String.
        /// </summary>
        [Required]
        public required string Username { get; set; }
    }
}

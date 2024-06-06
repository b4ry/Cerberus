using Cerberus.Attributes;

namespace Cerberus.DTOs
{
    /// <summary>
    /// A request object containing two string fields: UserName and Password
    /// </summary>
    public sealed class LoginRequest
    {
        /// <summary>
        /// Current user's name. String.
        /// </summary>
        [DevaultValueValidation("User name not provided!")]
        public required string UserName { get; set; }

        /// <summary>
        /// Current user's password. String.
        /// </summary>
        [DevaultValueValidation("Password not provided!")]
        public required string Password { get; set; }
    }
}

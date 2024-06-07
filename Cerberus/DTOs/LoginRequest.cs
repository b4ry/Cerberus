using Cerberus.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerberus.DTOs
{
    /// <summary>
    /// A record containing two string fields: UserName and Password
    /// </summary>
    public sealed record LoginRequest
    {
        /// <summary>
        /// Login request constructor.
        /// Accepts two string parameters: userName and password
        /// </summary>
        /// <param name="userName"> Current user's name. String. </param>
        /// <param name="password"> Current user's password. String. </param>
        public LoginRequest(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Current user's name. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("User name not provided!")]
        public string UserName { get; set; }

        /// <summary>
        /// Current user's password. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("Password not provided!")]
        public string Password { get; set; }
    }
}

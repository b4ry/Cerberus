using Cerberus.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerberus.Api.DTOs
{
    /// <summary>
    /// A record containing two string fields: Username and Password
    /// </summary>
    public sealed record RegisterRequest
    {
        /// <summary>
        /// Register request constructor.
        /// Accepts two string parameters: username and password
        /// </summary>
        /// <param name="username"> Registering user's name. String. </param>
        /// <param name="password"> Registering user's password. String. </param>
        public RegisterRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Registering user's name. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("Username not provided!")]
        public string Username { get; set; }

        /// <summary>
        /// Registering user's password. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("Password not provided!")]
        public string Password { get; set; }
    }
}

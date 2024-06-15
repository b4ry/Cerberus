﻿using Cerberus.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cerberus.Api.DTOs
{
    /// <summary>
    /// A record containing two string fields: Username and Password
    /// </summary>
    public sealed record LoginRequest
    {
        /// <summary>
        /// Login request constructor.
        /// Accepts two string parameters: username and password
        /// </summary>
        /// <param name="userame"> Current user's name. String. </param>
        /// <param name="password"> Current user's password. String. </param>
        public LoginRequest(string userame, string password)
        {
            Username = userame;
            Password = password;
        }

        /// <summary>
        /// Current user's name. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("Username not provided!")]
        public string Username { get; set; }

        /// <summary>
        /// Current user's password. String.
        /// </summary>
        [Required]
        [DevaultValueValidation("Password not provided!")]
        public string Password { get; set; }
    }
}

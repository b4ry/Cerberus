using Cerberus.Attributes;

namespace Cerberus.DTOs
{
    public sealed class LoginRequest
    {
        [DevaultValueValidation("User name not provided!")]
        public required string UserName { get; set; }

        [DevaultValueValidation("Password not provided!")]
        public required string Password { get; set; }
    }
}

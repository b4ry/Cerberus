using System.Security.Cryptography;
using Cerberus.Api.Services.Interfaces;

namespace Cerberus.Api.Services
{
    public class PasswordService : IPasswordService
    {
        private const int _saltSize = 16;

        public string GenerateSalt()
        {
            var saltBytes = new byte[_saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using Cerberus.Api.Services.Interfaces;

namespace Cerberus.Api.Services
{
    public class PasswordService : IPasswordService
    {
        private const int _saltSize = 16;
        private const int _keySize = 32;
        private const int _iterationsNumber = 1000;

        public string GenerateSalt()
        {
            var saltBytes = new byte[_saltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(
                password, Encoding.UTF8.GetBytes(salt), _iterationsNumber, HashAlgorithmName.SHA256))
            {
                var hash = rfc2898DeriveBytes.GetBytes(_keySize);

                return Convert.ToBase64String(hash);
            }
        }
    }
}

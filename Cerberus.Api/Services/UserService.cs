using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;

namespace Cerberus.Api.Services
{
    public class UserService(IUserRepository userRepository, IPasswordService passwordService) : IUserService
    {
        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            try
            {
                var salt = passwordService.GenerateSalt();
                var hashedPassword = passwordService.HashPassword(registerRequest.Password, salt);

                var userEntity = new UserEntity()
                {
                    Username = registerRequest.Username,
                    Password = hashedPassword,
                    Salt = salt
                };

                return await userRepository.AddAsync(userEntity);
            }
            catch (Exception ex)
            {
                ex.Data.Add("RegisterUser", $"Error occured while registering user: {registerRequest.Username}");

                throw;
            }
        }

        public async Task<bool> LoginUserAsync(LoginRequest loginRequest)
        {
            var user = await userRepository.FindAsync(loginRequest.Username);

            if (user != null)
            {
                var hash = passwordService.HashPassword(loginRequest.Password, user.Salt);

                if (hash == user.Password)
                {
                    return true;
                }
            }

            return false;
        }

    }
}

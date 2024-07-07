using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;

namespace Cerberus.Api.Services
{
    public class UserService(IUserRepository userRepository, IPasswordService passwordService) : IUserService
    {
        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            var userEntity = new UserEntity()
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password
            };

            try
            {
                var salt = passwordService.GenerateSalt();

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

            if(user != null && user.Password == loginRequest.Password)
            {
                return true;
            }

            return false;
        }

    }
}

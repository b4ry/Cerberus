using Cerberus.Api.DTOs;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.Api.Services
{
    public class UserRegistrationService(
        IUserRepository userRepository,
        ILogger<UserRegistrationService> logger) : IUserRegistrationService
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
                await userRepository.AddAsync(userEntity);
                var successfulSave = (await userRepository.SaveChangesAsync()) != 0;

                return successfulSave;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError($"Error occured while registering user: {registerRequest.Username}. Exception: {ex}");

                return false;
            }
        }
    }
}

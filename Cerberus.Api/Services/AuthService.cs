using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;

namespace Cerberus.Api.Services
{
    /// <summary>
    /// Handles users' authentication and authorization logic.
    /// </summary>
    /// <param name="userRepository">User repository handling user entity</param>
    /// <param name="passwordService">Password service handling password related logic like hashing, salting</param>
    public class AuthService(IUserRepository userRepository, IPasswordService passwordService) : IAuthService
    {
        /// <summary>
        /// Registers users. If a user already exists, then the underlying exception, enriched with some additional information, is being re-thrown.
        /// </summary>
        /// <param name="registerRequest">Request for user registration</param>
        public async Task RegisterUserAsync(RegisterRequest registerRequest)
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

                await userRepository.AddAsync(userEntity);
            }
            catch (Exception ex)
            {
                ex.Data.Add("RegisterUser", $"Error occured while registering user: {registerRequest.Username}");

                throw;
            }
        }

        /// <summary>
        /// Logins users validating with a salt technique.
        /// </summary>
        /// <param name="loginRequest">Request for login a user</param>
        /// <returns>
        ///     True when successful,
        ///     False when fails
        /// </returns>
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

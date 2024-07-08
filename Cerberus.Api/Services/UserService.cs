﻿using Cerberus.Api.DTOs;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;

namespace Cerberus.Api.Services
{
    public class UserService(IUserRepository userRepository, IPasswordService passwordService) : IUserService
    {
        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            var userEntity = new UserEntity()
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password,
                Salt = passwordService.GenerateSalt()
            };

            try
            {
                var hash = passwordService.HashPassword(userEntity.Password, userEntity.Salt);

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

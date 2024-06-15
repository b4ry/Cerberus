using Cerberus.Api.Controllers;
using Cerberus.Api.DTOs;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ILogger<UserRegistrationService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRegistrationService(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UserRegistrationService> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            var userEntity = new UserEntity()
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password
            };
            try
            {
                await _userRepository.AddAsync(userEntity);
                var successfulSave = (await _unitOfWork.SaveChangesAsync()) != 0;

                return successfulSave;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error occured while registering user: {registerRequest.Username}. Exception: {ex}");

                return false;
            }
        }
    }
}

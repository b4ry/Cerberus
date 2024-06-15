using Cerberus.Api.DTOs;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.UnitOfWork;

namespace Cerberus.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRegistrationService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            var userEntity = new UserEntity()
            {
                Username = registerRequest.Username,
                Password = registerRequest.Password
            };

            await _userRepository.AddAsync(userEntity);
            var successfulSave = (await _unitOfWork.SaveChangesAsync()) != 0;

            return successfulSave;
        }
    }
}

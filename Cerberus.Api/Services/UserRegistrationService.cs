using Cerberus.Api.DTOs;
using Cerberus.Api.Repositories;

namespace Cerberus.Api.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRepository _userRepository;

        public UserRegistrationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(RegisterRequest registerRequest)
        {
            var registered = await _userRepository.AddAsync(registerRequest);

            return registered;
        }
    }
}

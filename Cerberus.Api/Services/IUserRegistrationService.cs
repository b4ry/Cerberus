using Cerberus.Api.DTOs;

namespace Cerberus.Api.Services
{
    public interface IUserRegistrationService
    {
        public Task<bool> RegisterUserAsync(RegisterRequest registerRequest);
    }
}

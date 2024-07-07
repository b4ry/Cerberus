using Cerberus.Api.DTOs;

namespace Cerberus.Api.Services.Interfaces
{
    public interface IUserService
    {
        public Task<bool> RegisterUserAsync(RegisterRequest registerRequest);
        public Task<bool> LoginUserAsync(LoginRequest loginRequest);
    }
}

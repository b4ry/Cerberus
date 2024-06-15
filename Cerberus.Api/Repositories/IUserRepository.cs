using Cerberus.Api.DTOs;

namespace Cerberus.Api.Repositories
{
    public interface IUserRepository
    {
        public Task<bool> AddAsync(RegisterRequest registerRequest);
    }
}
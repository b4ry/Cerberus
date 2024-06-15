using Cerberus.DatabaseContext.Entities;

namespace Cerberus.DatabaseContext
{
    public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
    {
        public async Task AddAsync(UserEntity userEntity)
        {
            await applicationDbContext.AddAsync(userEntity);
        }
    }
}

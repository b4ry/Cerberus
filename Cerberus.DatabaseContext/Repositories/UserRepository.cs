using Cerberus.DatabaseContext.Entities;

namespace Cerberus.DatabaseContext
{
    public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
    {
        public async Task AddAsync(UserEntity userEntity)
        {
            await applicationDbContext.AddAsync(userEntity);
        }

        public async Task<UserEntity?> FindAsync(string username)
        {
            return await applicationDbContext.FindAsync<UserEntity>(username);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await applicationDbContext.SaveChangesAsync();
        }
    }
}

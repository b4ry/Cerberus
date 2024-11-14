using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.DatabaseContext.Repositories
{
    public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
    {
        public async Task AddAsync(UserEntity userEntity)
        {
            try
            {
                await applicationDbContext.AddAsync(userEntity);
                await applicationDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserEntity?> FindAsync(string username)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

            return user;
        }
    }
}

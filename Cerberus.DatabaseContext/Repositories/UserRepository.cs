using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.DatabaseContext.Repositories
{
    public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
    {
        public async Task<bool> AddAsync(UserEntity userEntity)
        {
            try
            {
                await applicationDbContext.AddAsync(userEntity);
                var savedEntitiesNumber = await applicationDbContext.SaveChangesAsync();

                return savedEntitiesNumber > 0;
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

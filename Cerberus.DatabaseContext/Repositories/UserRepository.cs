using Cerberus.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.DatabaseContext
{
    public class UserRepository(ApplicationDbContext applicationDbContext) : IUserRepository
    {
        public async Task<bool> AddAsync(UserEntity userEntity)
        {
            await using var transaction = await applicationDbContext.Database.BeginTransactionAsync();

            try
            {
                await applicationDbContext.AddAsync(userEntity);
                var savedEntitiesNumber = await applicationDbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return savedEntitiesNumber > 0;
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();

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

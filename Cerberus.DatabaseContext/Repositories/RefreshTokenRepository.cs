using Cerberus.DatabaseContext.Entities;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cerberus.DatabaseContext.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext applicationDbContext) : IRefreshTokenRepository
    {
        public async Task AddAsync(RefreshTokenEntity entity)
        {
            try
            {
                await applicationDbContext.AddAsync(entity);
                await applicationDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RefreshTokenEntity?> FindAsync(string key)
        {
            var refreshToken = await applicationDbContext.RefreshToken.FirstOrDefaultAsync(x => x.Id == key);

            return refreshToken;
        }
    }
}

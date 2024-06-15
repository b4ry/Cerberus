
namespace Cerberus.DatabaseContext.UnitOfWork
{
    public class UnitOfWork(ApplicationDbContext applicationDbContext) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync()
        {
            return await applicationDbContext.SaveChangesAsync();
        }
    }
}

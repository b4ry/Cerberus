using Cerberus.DatabaseContext.Entities;

namespace Cerberus.DatabaseContext
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        public Task AddAsync(TEntity entity);
        public Task<int> SaveChangesAsync();
    }
}

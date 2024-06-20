using Cerberus.DatabaseContext.Entities;

namespace Cerberus.DatabaseContext
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        public Task<bool> AddAsync(TEntity entity);
        public Task<TEntity?> FindAsync(string key);
    }
}

using Cerberus.DatabaseContext.Entities;

namespace Cerberus.DatabaseContext.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        public Task AddAsync(TEntity entity);
        public Task<TEntity?> FindAsync(string key);
    }
}

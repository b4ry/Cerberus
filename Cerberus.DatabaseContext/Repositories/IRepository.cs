namespace Cerberus.DatabaseContext
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task AddAsync(TEntity entity);
    }
}

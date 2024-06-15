namespace Cerberus.DatabaseContext.UnitOfWork
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();
    }
}

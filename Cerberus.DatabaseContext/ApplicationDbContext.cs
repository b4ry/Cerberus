using Cerberus.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Cerberus.DatabaseContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        private readonly string _entityMethodName = "Entity";
        private readonly string _databaseContextAssemblyName = "Cerberus.DatabaseContext";

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityMethod = typeof(ModelBuilder)
                .GetMethods()
                .FirstOrDefault(x => x.Name == _entityMethodName && x.IsGenericMethodDefinition);

            var entities = Assembly
                .Load(_databaseContextAssemblyName)
                .GetTypes()
                .Where(x => x.GetTypeInfo().BaseType == typeof(BaseEntity));

            foreach (var entity in entities)
            {
                var constructedMethod = entityMethod!.MakeGenericMethod(entity);

                constructedMethod.Invoke(modelBuilder, null);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}

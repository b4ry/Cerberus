using Cerberus.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Cerberus.DatabaseContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityMethod = typeof(ModelBuilder).GetMethods().FirstOrDefault(x => x.Name == "Entity" && x.IsGenericMethodDefinition);
            var entities = Assembly.Load("Cerberus.DatabaseContext").GetTypes().Where(x => x.GetTypeInfo().BaseType == typeof(BaseEntity));

            foreach (var entity in entities)
            {
                var constructedMethod = entityMethod!.MakeGenericMethod(entity);

                constructedMethod.Invoke(modelBuilder, null);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}

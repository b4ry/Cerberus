using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Tests.DatabaseContexts
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void DbContext_ShouldCreateAllModelEntities()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("testInMemoryDatabase");
            var options = builder.Options;
            var applicationDbContext = new ApplicationDbContext(options);
            var allModelEntitiesCreated = true;

            var modelEntities = typeof(BaseEntity).GetTypeInfo().Assembly.GetTypes().Where(x => x.GetTypeInfo().BaseType == typeof(BaseEntity));

            // Act
            foreach (Type modelEntity in modelEntities)
            {
                var modelEntityTypeName = modelEntity.GetTypeInfo().FullName;
                var foundEntityType = applicationDbContext.Model.FindEntityType(modelEntityTypeName);

                if (foundEntityType == null)
                {
                    allModelEntitiesCreated = false;
                    break;
                }
            }

            // Assert
            Assert.True(allModelEntitiesCreated, "Did not create all of the application's model entities");
        }
    }
}

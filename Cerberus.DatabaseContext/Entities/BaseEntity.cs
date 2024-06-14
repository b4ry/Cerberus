using System.ComponentModel.DataAnnotations;

namespace Cerberus.DatabaseContext.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { set; get; }
    }
}

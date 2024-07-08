using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cerberus.DatabaseContext.Entities
{
    [Table("Users")]
    [PrimaryKey("Username")]
    public class UserEntity : BaseEntity
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string Salt { get; set; }
    }
}

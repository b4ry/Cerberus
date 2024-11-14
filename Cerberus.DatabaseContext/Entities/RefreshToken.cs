using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cerberus.DatabaseContext.Entities
{
    [Table("RefreshTokens")]
    [PrimaryKey("Id")]
    public class RefreshTokenEntity : BaseEntity
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required DateTime ValidUntil { get; set; }
    }
}

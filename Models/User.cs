using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? UserName { get; set; }

        [MaxLength(50)]
        public string? Password { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        public int? RoleId { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(50)]
        public string? UpdatedAt { get; set; }

        public int? CentreId { get; set; }
    }
}

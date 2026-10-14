using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Consultant
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MobileNumber { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; }

        public int? CentreId { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

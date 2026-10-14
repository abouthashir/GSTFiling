using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class District
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UserId { get; set; }

        public int? Priority { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }
    }
}

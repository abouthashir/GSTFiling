using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Centre
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        public int? DistrictId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UserId { get; set; }

        public int? Priority { get; set; }

        [MaxLength(150)]
        public string? ContactName { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? MobileNumber { get; set; }

        [MaxLength(50)]
        public string? AlternateMobileNumber { get; set; }

        public string? Address { get; set; }
    }
}

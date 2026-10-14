using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? ShopName { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? GstNumber { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? MobileNumber { get; set; }

        public string? Address { get; set; }

        public int? CentreId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ConsultantId { get; set; }

        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
    }
}

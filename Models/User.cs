using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GSTFiling.Models
{
    // One row for EVERY account that can log in (Admin, Centre, Consultant, Shop).
    [Index(nameof(Email), IsUnique = true)]
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

        // Filled for Centre users (and for Consultant/Shop users: their centre)
        public int? CentreId { get; set; }

        // Filled only for Consultant users: links to Consultants.Id
        public int? ConsultantId { get; set; }

        // Filled only for Shop users: links to Customers.Id
        public int? ShopId { get; set; }
    }
}
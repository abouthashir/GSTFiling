using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        // Role names used in code. They must match the Name column in the Roles table.
        public const string Admin = "Admin";
        public const string Centre = "Centre";
        public const string Consultant = "Consultant";
        public const string Shop = "Shop";
    }
}
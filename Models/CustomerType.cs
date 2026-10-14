using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class CustomerType
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        public int? Priority { get; set; }
    }
}

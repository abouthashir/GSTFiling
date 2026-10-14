using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{

    public class AboutUs
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(150)]
        public string? BannerImage { get; set; }

        [MaxLength(150)]
        public string? AboutImage { get; set; }

        [MaxLength(50)]
        public string? Date { get; set; }

        public bool? IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models.ViewModels
{
    public class EditConsultantViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the consultant's name.")]
        [StringLength(200, ErrorMessage = "Name can be at most 200 characters.")]
        [Display(Name = "Full name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address, like name@example.com.")]
        [StringLength(150, ErrorMessage = "Email can be at most 150 characters.")]
        [Display(Name = "Email (used to log in)")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a mobile number.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter a 10-digit mobile number.")]
        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address can be at most 500 characters.")]
        public string? Address { get; set; }

        [Display(Name = "Active (can log in)")]
        public bool IsActive { get; set; }

        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be 8 to 50 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password needs a capital letter, a small letter, a number and a symbol.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password (optional)")]
        public string? NewPassword { get; set; }
    }
}
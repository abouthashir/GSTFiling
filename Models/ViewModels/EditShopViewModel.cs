using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GSTFiling.Models.ViewModels
{
    public class EditShopViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the shop name.")]
        [StringLength(200, ErrorMessage = "Shop name can be at most 200 characters.")]
        [Display(Name = "Shop name")]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the owner's name.")]
        [StringLength(200, ErrorMessage = "Owner name can be at most 200 characters.")]
        [Display(Name = "Owner name")]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter the GST number.")]
        [RegularExpression(@"^[0-9]{2}[A-Za-z]{5}[0-9]{4}[A-Za-z][1-9A-Za-z][Zz][0-9A-Za-z]$",
            ErrorMessage = "Enter a valid 15-character GST number, like 32ABCDE1234F1Z5.")]
        [Display(Name = "GST number (GSTIN)")]
        public string GstNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a mobile number.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter a 10-digit mobile number.")]
        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address, like name@example.com.")]
        [StringLength(150, ErrorMessage = "Email can be at most 150 characters.")]
        [Display(Name = "Email (used to log in)")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be 8 to 50 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password needs a capital letter, a small letter, a number and a symbol.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password (optional)")]
        public string? NewPassword { get; set; }

        [StringLength(500, ErrorMessage = "Address can be at most 500 characters.")]
        public string? Address { get; set; }

        [Display(Name = "Consultant")]
        public int? ConsultantId { get; set; }

        [ValidateNever]
        public List<SelectListItem> ConsultantOptions { get; set; } = new();
    }
}
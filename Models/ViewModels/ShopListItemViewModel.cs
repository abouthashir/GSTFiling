namespace GSTFiling.Models.ViewModels
{
    // One row of the Shops table
    public class ShopListItemViewModel
    {
        public int Id { get; set; }

        public string ShopName { get; set; } = string.Empty;

        public string? OwnerName { get; set; }

        public string? GstNumber { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }

        public int? ConsultantId { get; set; }

        // Comes from the Consultants table (null = no consultant assigned)
        public string? ConsultantName { get; set; }
    }
}
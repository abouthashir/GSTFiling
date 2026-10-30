namespace GSTFiling.Models.ViewModels
{
    // Everything the Centre's "Shop invoices" page needs (view only)
    public class ShopInvoicesPageViewModel
    {
        // The 12 month names, the same way the Shop module saves them ("October")
        public static readonly string[] MonthNames =
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        // Shop details for the page header
        public int ShopId { get; set; }

        public string ShopName { get; set; } = string.Empty;

        public string? OwnerName { get; set; }

        public string? GstNumber { get; set; }

        // Selected filters (null = all)
        public int? Year { get; set; }

        public string? Month { get; set; }

        // Years this shop has invoices for, for the Year dropdown
        public List<int> YearOptions { get; set; } = new();

        public List<ShopInvoiceRowViewModel> Invoices { get; set; } = new();

        // Totals of the rows shown
        public decimal TotalTaxable => Invoices.Sum(i => i.TaxableAmount);

        public decimal TotalGst => Invoices.Sum(i => i.GstAmount);

        public decimal GrandTotal => TotalTaxable + TotalGst;
    }
}
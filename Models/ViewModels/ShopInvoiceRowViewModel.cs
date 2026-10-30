namespace GSTFiling.Models.ViewModels
{
    // One row of the shop's invoice table (read-only)
    public class ShopInvoiceRowViewModel
    {
        public int Id { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? InvoiceDate { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal Cgst { get; set; }

        public decimal Sgst { get; set; }

        public decimal Cess { get; set; }

        // CGST + SGST + Cess (the Shop module saves it this way)
        public decimal GstAmount { get; set; }

        // Taxable amount + GST = the bill total
        public decimal TotalAmount => TaxableAmount + GstAmount;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Shop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string GSTIN { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<InvoiceRecord> Invoices { get; set; } = new List<InvoiceRecord>();
    }

    public class InvoiceRecord
    {
        [Key]
        public int Id { get; set; }

        public int ShopId { get; set; }
        public Shop? Shop { get; set; }

        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        public string? CustomerName { get; set; }
        public string? InvoiceGstNumber { get; set; }

        [Required]
        public string TaxRate { get; set; } = string.Empty;

        [Required]
        public decimal BillAmount { get; set; }

        [Required]
        public decimal TotalBillAmount { get; set; }

        [Required]
        public decimal TaxableAmount { get; set; }

        public decimal CgstAmount { get; set; }
        public decimal SgstAmount { get; set; }
        public decimal CessAmount { get; set; } = 0.00m;

        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Type { get; set; } = "B2C"; // e.g., B2C, B2B Sales, Debit Note Sales
        public string Status { get; set; } = "Pending Consultant"; // Pending Consultant, Draft, Approved
        public string? GstDetails { get; set; }
    }
}
// Models/Invoices.cs
using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Invoices
    {
        [Key]
        public int invid { get; set; }
        public string? invnumber { get; set; }
        public string? invdate { get; set; }
        public decimal invamtdec { get; set; }
        public string? gstnum { get; set; }
        public int txid { get; set; }
        public int cmid { get; set; }
        public decimal amtgstout { get; set; }
        public decimal sgst { get; set; }
        public decimal cgst { get; set; }
        public decimal cess { get; set; }
        public string? customer { get; set; }
    }
}

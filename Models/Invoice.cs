using System.ComponentModel.DataAnnotations;

namespace GSTFiling.Models
{
    public class Invoice
    {
        [Key]
        public int invid { get; set; }
        public int cstmrid { get; set; }
        public string? invno { get; set; }
        public string? invdate { get; set; }
        public decimal taxable_amt { get; set; }
        public decimal gst_amt { get; set; }
        public string? type { get; set; }
        public int invyear { get; set; }
        public string? invmonth { get; set; }
        // New fields
        public decimal taxrate { get; set; }
        public decimal sgst { get; set; }
        public decimal cgst { get; set; }
        public decimal cess { get; set; }
        public string? billamtstr { get; set; }
    }

    public class InvMonthRow
    {
        public int cmid { get; set; }
        public string month { get; set; } = "";
        public int year { get; set; }
        public int ctypid { get; set; }
        public string ctype { get; set; } = "";
        public int fstatus { get; set; }
        public string? message { get; set; }
        public decimal? amount { get; set; }
        public string? docpath { get; set; }
        public string? consultant_doc { get; set; }
        public string? openai_result { get; set; }
        public decimal tot_taxable { get; set; }
        public decimal tot_cgst { get; set; }
        public decimal tot_sgst { get; set; }
    }
}

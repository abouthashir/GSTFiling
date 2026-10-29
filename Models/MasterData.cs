namespace GSTFiling.Models
{
    public class Year
    {
        public int yid { get; set; }
        public int year { get; set; }
    }

    public class Month
    {
        public int mid { get; set; }
        public string month { get; set; } = string.Empty;
    }

    public class TaxRate
    {
        public int txid { get; set; }
        public string txrate { get; set; } = string.Empty;
    }

    public class ConsultantRate
    {
        public int crid { get; set; }
        public decimal rate { get; set; }
    }

    public class InvoiceType
    {
        public int ctypid { get; set; } // Changed from itid to ctypid
        public string ctype { get; set; } = string.Empty; // Changed from type to ctype
    }

    public class MasterDataViewModel
    {
        public List<Year> Years { get; set; } = new();
        public List<Month> Months { get; set; } = new();
        public List<TaxRate> TaxRates { get; set; } = new();
        public List<ConsultantRate> ConsultantRates { get; set; } = new();
        public List<InvoiceType> InvoiceTypes { get; set; } = new();
    }
}


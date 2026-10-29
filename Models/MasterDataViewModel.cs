using System.Collections.Generic;

namespace GSTFiling.Models
{
    public class MasterDataViewModel
    {
        public List<FinancialYear> Years { get; set; }
            = new List<FinancialYear>();

        public List<FilingMonths> Months { get; set; }
            = new List<FilingMonths>();

        public List<GstTaxRates> Rates { get; set; }
            = new List<GstTaxRates>();

        public List<InvoiceClassification> Classifications
        {
            get;
            set;
        } = new List<InvoiceClassification>();
    }
}
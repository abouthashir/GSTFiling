namespace GSTFiling.Models
{
    public class GstTaxRates
    {
        public int Id { get; set; }

        public decimal Rate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSTFiling.Models
{
    public class GstFiling
    {
        [Key]
        public int InvoiceMonthId { get; set; }

        public int? Status { get; set; }

        public string? Message { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        public string? DocumentPath { get; set; }

        public string? ConsultantDocumentPath { get; set; }

        public string? OpenAiResult { get; set; }
    }
}

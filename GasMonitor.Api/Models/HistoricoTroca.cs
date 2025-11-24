using System.ComponentModel.DataAnnotations;

namespace GasMonitor.Api.Models
{
    public class HistoricoTroca
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataTroca { get; set; }
        public decimal PrecoPago { get; set; }
        public string NomeProduto { get; set; } = string.Empty; // Ex: "P13"
    }
}

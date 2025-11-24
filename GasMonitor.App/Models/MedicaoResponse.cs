namespace GasMonitor.App.Models
{
    public class MedicaoResponse
    {
        public long Id { get; set; }
        public string IdDispositivo { get; set; } = string.Empty;
        public double PesoTotalKg { get; set; }
        public DateTime DataHora { get; set; }
        public int PorcentagemGas { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ValorGasto { get; set; }
        public decimal ValorRestante { get; set; }
        public int DiasRestantesEstimados { get; set; }
        public bool AlertaVazamento { get; set; }
    }
}

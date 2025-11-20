namespace GasMonitor.Api.Models
{
    public class MedicaoResponse
    {
        public long Id { get; set; }
        public string IdDispositivo { get; set; } = string.Empty;
        public double PesoTotalKg { get; set; }
        public DateTime DataHora { get; set; }

        // --- O Campo Novo Calculado ---
        public int PorcentagemGas { get; set; }
        
        public string Status { get; set; } = string.Empty; // Ex: "Cheio", "Atenção", "Vazio"
    }
}
namespace GasMonitor.App.Models
{
    public class EstatisticasResponse
    {
        public DateTime DataAquisicao { get; set; }
        public int DiasDeUso { get; set; }
        public double MediaConsumoDiarioKg { get; set; }

        public Dictionary<string, double> ConsumoPorDiaSemana { get; set; } = new();
        public Dictionary<string, double> ConsumoPorMes { get; set; } = new();

        public double ConsumoDiaKg { get; set; }
        public double ConsumoNoiteKg { get; set; }
        public string TurnoMaisConsumidor { get; set; } = string.Empty;

        // --- CAMPO NOVO DO HISTÓRICO ---
        public List<HistoricoPonto> HistoricoPrecos { get; set; } = new();
    }

    public class HistoricoPonto
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
    }
}

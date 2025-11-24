namespace GasMonitor.App.Models
{
    public class EstatisticasResponse
    {
        // Resumo Geral
        public DateTime DataAquisicao { get; set; }
        public int DiasDeUso { get; set; }
        public double MediaConsumoDiarioKg { get; set; }

        // Análise Temporal (Dicionários para gráficos)
        public Dictionary<string, double> ConsumoPorDiaSemana { get; set; } = new();
        public Dictionary<string, double> ConsumoPorMes { get; set; } = new();

        // Análise de Turno
        public double ConsumoDiaKg { get; set; }
        public double ConsumoNoiteKg { get; set; }
        public string TurnoMaisConsumidor { get; set; } = string.Empty;
    }
}

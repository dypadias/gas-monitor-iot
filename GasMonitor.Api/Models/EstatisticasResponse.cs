namespace GasMonitor.Api.Models
{
    public class EstatisticasResponse
    {
        // Resumo Geral
        public DateTime DataAquisicao { get; set; }
        public int DiasDeUso { get; set; }
        public double MediaConsumoDiarioKg { get; set; }

        // Análise Temporal
        public Dictionary<string, double> ConsumoPorDiaSemana { get; set; } = new(); // Ex: "Segunda": 0.5kg
        public Dictionary<string, double> ConsumoPorMes { get; set; } = new(); // Ex: "Novembro": 12kg

        // Análise de Turno
        public double ConsumoDiaKg { get; set; } // 06:00 - 18:00
        public double ConsumoNoiteKg { get; set; } // 18:00 - 06:00
        public string TurnoMaisConsumidor { get; set; } = string.Empty;
    }
}

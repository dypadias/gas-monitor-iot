namespace GasMonitor.Api.Models
{
    /// <summary>
    /// Objeto "Inteligente" enviado para o Site/App, já com cálculos feitos.
    /// </summary>
    public class MedicaoResponse
    {
        public long Id { get; set; }
        public string IdDispositivo { get; set; } = string.Empty;
        public double PesoTotalKg { get; set; }
        public DateTime DataHora { get; set; }
        public int PorcentagemGas { get; set; }
        public string Status { get; set; } = string.Empty;

        // --- NOVOS CAMPOS FINANCEIROS E DE SEGURANÇA ---

        /// <summary>
        /// Quanto dinheiro já foi "queimado" com base no consumo.
        /// </summary>
        public decimal ValorGasto { get; set; }

        /// <summary>
        /// Quanto dinheiro ainda resta dentro do botijão.
        /// </summary>
        public decimal ValorRestante { get; set; }

        /// <summary>
        /// Estimativa de quantos dias o gás ainda dura (baseado na média).
        /// </summary>
        public int DiasRestantesEstimados { get; set; }

        /// <summary>
        /// Alerta crítico para o Frontend piscar vermelho.
        /// </summary>
        public bool AlertaVazamento { get; set; }
    }
}

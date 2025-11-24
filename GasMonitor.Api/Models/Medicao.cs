using System.ComponentModel.DataAnnotations;

namespace GasMonitor.Api.Models
{
    /// <summary>
    /// Representa uma leitura histórica guardada no Banco de Dados.
    /// </summary>
    public class Medicao
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// O ID do dispositivo que enviou a medição.
        /// </summary>
        [Required]
        public string IdDispositivo { get; set; } = string.Empty;

        /// <summary>
        /// O peso bruto lido no momento.
        /// </summary>
        [Required]
        public double PesoKg { get; set; }

        /// <summary>
        /// Data e hora exata em que o dado chegou ao servidor.
        /// </summary>
        [Required]
        public DateTime DataHoraRegisto { get; set; }

        /// <summary>
        /// NOVO: Regista se houve alerta de vazamento neste momento específico.
        /// </summary>
        public bool VazamentoDetectado { get; set; }
    }
}

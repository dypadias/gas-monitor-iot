using System.ComponentModel.DataAnnotations; // Necessário para [Key]

namespace GasMonitor.Api.Models
{
    /// <summary>
    /// Representa uma única leitura de peso guardada no banco de dados.
    /// </summary>
    public class Medicao
    {
        [Key] // Indica que esta é a Chave Primária (PK) da tabela
        public long Id { get; set; }

        /// <summary>
        /// O ID do dispositivo que enviou a medição (ex: "ESP32_COZINHA_01")
        /// </summary>
        [Required] // Indica que esta coluna não pode ser nula
        public string IdDispositivo { get; set; } = string.Empty;
        /// <summary>
        /// O peso lido em Quilogramas (Kg)
        /// </summary>
        [Required]
        public double PesoKg { get; set; }

        /// <summary>
        /// O carimbo de data/hora de quando esta medição foi registada no sistema.
        /// </summary>
        [Required]
        public DateTime DataHoraRegisto { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace GasMonitor.Api.Models
{
    /// <summary>
    /// Configuração do tipo de recipiente (Ex: P13, P45).
    /// </summary>
    public class ProdutoConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Peso do botijão vazio.
        /// </summary>
        public double TaraKg { get; set; }

        /// <summary>
        /// Quantidade de produto (líquido) que cabe dentro.
        /// </summary>
        public double CapacidadeTotalKg { get; set; }

        public string TipoUnidade { get; set; } = "Kg";

        public bool Ativo { get; set; }

        /// <summary>
        /// NOVO: Quanto o utilizador pagou por este botijão (em Reais).
        /// Usado para calcular o "Valor Gasto".
        /// </summary>
        public decimal PrecoPago { get; set; }
    }
}

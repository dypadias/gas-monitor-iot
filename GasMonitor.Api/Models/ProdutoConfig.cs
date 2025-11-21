using System.ComponentModel.DataAnnotations;

namespace GasMonitor.Api.Models
{
    public class ProdutoConfig
    {
        [Key]
        public int Id { get; set; }

        // Ex: "Gás de Cozinha P13", "Barril de Chopp 50L"
        [Required]
        public string Nome { get; set; } = string.Empty;

        // Peso do recipiente vazio (Tara). Ex: 15.0 para P13
        public double TaraKg { get; set; }

        // Quanto produto cabe dentro (Líquido). Ex: 13.0 para P13
        public double CapacidadeTotalKg { get; set; }
        
        // Se é Gás, Líquido, Sólido (apenas para exibição no gráfico/unidade)
        public string TipoUnidade { get; set; } = "Kg"; 

        // Define se este é o produto que está atualmente na balança
        public bool Ativo { get; set; }
    }
}
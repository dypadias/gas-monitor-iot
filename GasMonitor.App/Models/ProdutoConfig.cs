namespace GasMonitor.App.Models
{
    public class ProdutoConfig
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double TaraKg { get; set; }
        public double CapacidadeTotalKg { get; set; }
        public bool Ativo { get; set; }
        public decimal PrecoPago { get; set; }
    }
}

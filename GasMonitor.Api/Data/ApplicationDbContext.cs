using GasMonitor.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GasMonitor.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Medicao> Medicoes { get; set; }
        public DbSet<ProdutoConfig> ProdutosConfig { get; set; }

        // ESTA LINHA É OBRIGATÓRIA PARA O GRÁFICO DE PREÇOS FUNCIONAR:
        public DbSet<HistoricoTroca> HistoricoTrocas { get; set; }
    }
}

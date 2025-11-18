using GasMonitor.Api.Models; // Importa o modelo
using Microsoft.EntityFrameworkCore;

namespace GasMonitor.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Mapeia a nossa classe "Medicao" para uma tabela chamada "Medicoes"
        public DbSet<Medicao> Medicoes { get; set; }
    }
}
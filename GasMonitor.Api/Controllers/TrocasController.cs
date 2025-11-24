using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrocasController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public TrocasController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarTroca([FromBody] HistoricoTroca troca)
        {
            // 1. Salva no histórico eterno
            troca.DataTroca = DateTime.UtcNow;
            _contexto.HistoricoTrocas.Add(troca);

            // 2. Atualiza o preço do produto ATIVO atual (para o cálculo do dia a dia)
            var produtoAtivo = await _contexto.ProdutosConfig.FirstOrDefaultAsync(p => p.Ativo);
            if (produtoAtivo != null)
            {
                produtoAtivo.PrecoPago = troca.PrecoPago;
            }

            await _contexto.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoricoTroca>>> GetHistorico()
        {
            return await _contexto
                .HistoricoTrocas.OrderByDescending(t => t.DataTroca)
                .ToListAsync();
        }
    }
}

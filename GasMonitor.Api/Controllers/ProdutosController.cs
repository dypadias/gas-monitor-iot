using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public ProdutosController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoConfig>>> GetProdutos()
        {
            return await _contexto.ProdutosConfig.ToListAsync();
        }

        // GET: api/produtos/ativo
        [HttpGet("ativo")]
        public async Task<ActionResult<ProdutoConfig>> GetProdutoAtivo()
        {
            var ativo = await _contexto.ProdutosConfig.FirstOrDefaultAsync(p => p.Ativo);
            if (ativo == null)
            {
                // Padrão se nada estiver configurado
                return new ProdutoConfig
                {
                    Nome = "Padrão (P13)",
                    TaraKg = 15.0,
                    CapacidadeTotalKg = 13.0,
                    Ativo = true,
                    PrecoPago = 0,
                };
            }
            return ativo;
        }

        // POST: api/produtos
        [HttpPost]
        public async Task<ActionResult<ProdutoConfig>> CriarProduto(ProdutoConfig produto)
        {
            _contexto.ProdutosConfig.Add(produto);
            await _contexto.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        // POST: api/produtos/{id}/ativar
        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> AtivarProduto(int id)
        {
            var todos = await _contexto.ProdutosConfig.ToListAsync();
            foreach (var p in todos)
                p.Ativo = false;

            var escolhido = todos.FirstOrDefault(p => p.Id == id);
            if (escolhido == null)
                return NotFound();

            escolhido.Ativo = true;
            await _contexto.SaveChangesAsync();

            return Ok(escolhido);
        }

        // --- MÉTODOS NOVOS (Dentro da classe!) ---

        // PUT: api/produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, ProdutoConfig produto)
        {
            if (id != produto.Id) return BadRequest();

            var existente = await _contexto.ProdutosConfig.FindAsync(id);
            if (existente == null) return NotFound();

            // Atualiza os campos
            existente.Nome = produto.Nome;
            existente.TaraKg = produto.TaraKg;
            existente.CapacidadeTotalKg = produto.CapacidadeTotalKg;
            existente.PrecoPago = produto.PrecoPago;
            // Não alteramos o 'Ativo' aqui para não causar conflitos

            await _contexto.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProduto(int id)
        {
            var produto = await _contexto.ProdutosConfig.FindAsync(id);
            if (produto == null) return NotFound();

            // Não deixa apagar o produto que está em uso
            if (produto.Ativo) return BadRequest("Não é possível apagar o produto que está em uso.");

            _contexto.ProdutosConfig.Remove(produto);
            await _contexto.SaveChangesAsync();
            return NoContent();
        }
    } // <--- A CLASSE FECHA AQUI
} // <--- O NAMESPACE FECHA AQUI
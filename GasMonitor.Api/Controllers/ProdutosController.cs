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

        // GET: api/produtos (Lista todos os tipos configurados)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoConfig>>> GetProdutos()
        {
            return await _contexto.ProdutosConfig.ToListAsync();
        }

        // GET: api/produtos/ativo (Retorna a configuração atual da balança)
        [HttpGet("ativo")]
        public async Task<ActionResult<ProdutoConfig>> GetProdutoAtivo()
        {
            var ativo = await _contexto.ProdutosConfig.FirstOrDefaultAsync(p => p.Ativo);
            if (ativo == null)
            {
                // Se não houver nenhum configurado, devolve um padrão P13
                return new ProdutoConfig 
                { 
                    Nome = "Padrão (P13)", 
                    TaraKg = 15.0, 
                    CapacidadeTotalKg = 13.0, 
                    Ativo = true 
                };
            }
            return ativo;
        }

        // POST: api/produtos (Cria um novo tipo, ex: P45)
        [HttpPost]
        public async Task<ActionResult<ProdutoConfig>> CriarProduto(ProdutoConfig produto)
        {
            _contexto.ProdutosConfig.Add(produto);
            await _contexto.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        // POST: api/produtos/{id}/ativar (Troca o que está na balança)
        [HttpPost("{id}/ativar")]
        public async Task<IActionResult> AtivarProduto(int id)
        {
            // 1. Desativa todos
            var todos = await _contexto.ProdutosConfig.ToListAsync();
            foreach (var p in todos) p.Ativo = false;

            // 2. Ativa o escolhido
            var escolhido = todos.FirstOrDefault(p => p.Id == id);
            if (escolhido == null) return NotFound();

            escolhido.Ativo = true;
            await _contexto.SaveChangesAsync();

            return Ok(escolhido);
        }
        
        // A funcionalidade de "Tarar" (Zero) ficará aqui no futuro,
        // enviando um comando para o ESP32 ou ajustando um offset no banco.
    }
}
using GasMonitor.Api.Data; // <-- ALTERAÇÃO: Precisamos disto para usar o DbContext
using GasMonitor.Api.Models; 
using Microsoft.AspNetCore.Mvc;

namespace GasMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class MedicoesController : ControllerBase
    {
        // --- ALTERAÇÃO 1: Substituir o "Logger" pelo "DbContext" ---
        
        // Removemos o ILogger temporário
        // private readonly ILogger<MedicoesController> _logger;
        
        // Adicionamos o nosso contexto do banco de dados
        private readonly ApplicationDbContext _contexto;

        // O .NET vai "injetar" (entregar) o DbContext automaticamente aqui
        public MedicoesController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // --- Fim da ALTERAÇÃO 1 ---

        /// <summary>
        /// Endpoint para receber novas medições de peso dos dispositivos.
        /// </summary>
        [HttpPost] 
        // --- ALTERAÇÃO 2: Tornar o método "Assíncrono" ---
        public async Task<IActionResult> ReceberMedicao([FromBody] MedicaoInput medicaoInput)
        {
            // Validação simples
            if (medicaoInput.PesoKg <= 0)
            {
                return BadRequest("Peso inválido. O valor deve ser maior que zero.");
            }

            // --- ALTERAÇÃO 3: Mapear e Salvar no Banco ---

            // 1. "Mapear" os dados do "Input" para o nosso modelo de Banco de Dados
            // O "Input" (MedicaoInput) não tem "Id" nem "DataHoraRegisto".
            // O "Modelo" (Medicao) precisa desses dados.
            var novaMedicao = new Medicao
            {
                IdDispositivo = medicaoInput.IdDispositivo,
                PesoKg = medicaoInput.PesoKg,
                // Definimos o carimbo de data/hora no servidor, para ser mais fiável
                DataHoraRegisto = DateTime.UtcNow 
            };

            // 2. Adicionar o novo objeto ao "contexto" do EF Core
            // (Isto ainda não guarda no banco, apenas "prepara" a mudança)
            _contexto.Medicoes.Add(novaMedicao);

            // 3. Salvar as mudanças no banco de dados
            // Esta é a linha que executa o "INSERT INTO..."
            await _contexto.SaveChangesAsync();

            // --- Fim da ALTERAÇÃO 3 ---
            
            // Em vez de "Ok(...)", devolvemos "CreatedAtAction" (Boas Práticas de API)
            // Isto devolve um código "201 Created" e diz ao cliente onde 
            // encontrar o novo recurso (embora ainda não tenhamos criado esse endpoint GET)
            return CreatedAtAction(null, new { id = novaMedicao.Id });
        }
    }
}
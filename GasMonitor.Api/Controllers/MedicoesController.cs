using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <-- IMPORTANTE: Necessário para o ToListAsync

namespace GasMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoesController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public MedicoesController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// Endpoint para receber novas medições (POST).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ReceberMedicao([FromBody] MedicaoInput medicaoInput)
        {
            if (medicaoInput.PesoKg <= 0)
            {
                return BadRequest("Peso inválido. O valor deve ser maior que zero.");
            }

            var novaMedicao = new Medicao
            {
                IdDispositivo = medicaoInput.IdDispositivo,
                PesoKg = medicaoInput.PesoKg,
                DataHoraRegisto = DateTime.UtcNow
            };

            _contexto.Medicoes.Add(novaMedicao);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodasMedicoes), new { id = novaMedicao.Id }, novaMedicao);
        }


        /// <summary>
        /// Endpoint para LER todo o histórico de medições (GET).
        /// URL: GET /api/medicoes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicaoResponse>>> GetTodasMedicoes()
        {
            // 1. Busca os dados brutos no banco
            var medicoesBrutas = await _contexto.Medicoes
                                               .OrderByDescending(m => m.DataHoraRegisto)
                                               .ToListAsync();

            // 2. Cria a lista de resposta transformando os dados
            var listaResposta = new List<MedicaoResponse>();

            // Constantes do P13 (poderiam vir de uma configuração no futuro)
            const double PESO_TARA = 15.0; // Peso do botijão vazio
            const double PESO_GAS_TOTAL = 13.0; // Capacidade total de gás

            foreach (var item in medicoesBrutas)
            {
                // Lógica de Cálculo
                double pesoDoGasAtual = item.PesoKg - PESO_TARA;
                
                // Se o peso for menor que a tara, assumimos 0 (para não dar negativo)
                if (pesoDoGasAtual < 0) pesoDoGasAtual = 0;

                // Regra de 3 para achar a percentagem
                double porcentagem = (pesoDoGasAtual / PESO_GAS_TOTAL) * 100;

                // Define um Status amigável
                string statusTexto = "Normal";
                if (porcentagem > 100) statusTexto = "Sobrecarregado"; // Erro de leitura ou tara errada
                else if (porcentagem <= 0) statusTexto = "Vazio";
                else if (porcentagem < 20) statusTexto = "Atenção: Gás no Fim!";

                listaResposta.Add(new MedicaoResponse
                {
                    Id = item.Id,
                    IdDispositivo = item.IdDispositivo,
                    PesoTotalKg = item.PesoKg,
                    DataHora = item.DataHoraRegisto,
                    PorcentagemGas = (int)porcentagem, // Arredonda para inteiro
                    Status = statusTexto
                });
            }

            return Ok(listaResposta);
        }
    }
}
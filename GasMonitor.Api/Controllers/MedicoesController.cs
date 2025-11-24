using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                VazamentoDetectado = medicaoInput.TemVazamento,
                DataHoraRegisto = DateTime.UtcNow,
            };

            _contexto.Medicoes.Add(novaMedicao);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTodasMedicoes),
                new { id = novaMedicao.Id },
                novaMedicao
            );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicaoResponse>>> GetTodasMedicoes()
        {
            var produtoAtivo = await _contexto.ProdutosConfig.FirstOrDefaultAsync(p => p.Ativo);

            double pesoTara = produtoAtivo?.TaraKg ?? 15.0;
            double capacidadeTotal = produtoAtivo?.CapacidadeTotalKg ?? 13.0;
            decimal precoBotijao = produtoAtivo?.PrecoPago ?? 0;

            var medicoesBrutas = await _contexto
                .Medicoes.OrderByDescending(m => m.DataHoraRegisto)
                .ToListAsync();

            var listaResposta = new List<MedicaoResponse>();
            double consumoDiarioMedio = 0.2;

            foreach (var item in medicoesBrutas)
            {
                double pesoDoGasAtual = item.PesoKg - pesoTara;
                if (pesoDoGasAtual < 0)
                {
                    pesoDoGasAtual = 0;
                }

                double porcentagem = 0;
                if (capacidadeTotal > 0)
                {
                    porcentagem = (pesoDoGasAtual / capacidadeTotal) * 100;
                }

                decimal valorDoGasAtual = 0;
                if (capacidadeTotal > 0 && precoBotijao > 0)
                {
                    valorDoGasAtual =
                        (decimal)pesoDoGasAtual * (precoBotijao / (decimal)capacidadeTotal);
                }

                decimal valorQueimado = precoBotijao - valorDoGasAtual;
                if (valorQueimado < 0)
                {
                    valorQueimado = 0;
                }

                int diasRestantes = 0;
                if (consumoDiarioMedio > 0 && pesoDoGasAtual > 0)
                {
                    diasRestantes = (int)(pesoDoGasAtual / consumoDiarioMedio);
                }

                string statusTexto = "Normal";

                if (item.VazamentoDetectado)
                {
                    statusTexto = "PERIGO: VAZAMENTO!";
                }
                else if (porcentagem > 110)
                {
                    statusTexto = "Sobrecarregado / Erro Tara";
                }
                else if (porcentagem <= 0)
                {
                    statusTexto = "Vazio";
                }
                else if (porcentagem < 20)
                {
                    statusTexto = "Reserva (Atenção)";
                }

                listaResposta.Add(
                    new MedicaoResponse
                    {
                        Id = item.Id,
                        IdDispositivo = item.IdDispositivo,
                        PesoTotalKg = item.PesoKg,
                        DataHora = item.DataHoraRegisto,
                        PorcentagemGas = (int)porcentagem,
                        Status = statusTexto,
                        ValorRestante = Math.Round(valorDoGasAtual, 2),
                        ValorGasto = Math.Round(valorQueimado, 2),
                        DiasRestantesEstimados = diasRestantes,
                        AlertaVazamento = item.VazamentoDetectado,
                    }
                );
            }

            return Ok(listaResposta);
        }
    }
}

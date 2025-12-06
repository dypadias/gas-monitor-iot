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
            // Validação simples
            if (medicaoInput.PesoKg < 0)
                return BadRequest("Peso inválido.");

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

            // Valores padrão se não houver config
            double pesoTara = produtoAtivo?.TaraKg ?? 13.0;
            double capacidadeTotal = produtoAtivo?.CapacidadeTotalKg ?? 13.0;
            decimal precoBotijao = produtoAtivo?.PrecoPago ?? 0;

            // Busca as últimas 1000 medições para não pesar o banco
            var medicoesBrutas = await _contexto
                .Medicoes.OrderByDescending(m => m.DataHoraRegisto)
                .Take(1000)
                .ToListAsync();

            if (!medicoesBrutas.Any())
                return Ok(new List<MedicaoResponse>());

            // --- LÓGICA DE MÉDIA INTELIGENTE ---
            double consumoDiarioMedio = 0.200; // Começa com padrão de mercado

            // Se tivermos dados suficientes, calculamos a média real deste ciclo
            if (medicoesBrutas.Count > 10)
            {
                var ultimaLeitura = medicoesBrutas.First();

                // Procura no histórico o momento em que o botijão estava "mais cheio" recentemente
                // (Isso identifica o início do uso deste botijão atual)
                var leituraInicioCiclo = medicoesBrutas
                    .OrderByDescending(m => m.PesoKg) // Ordena pelo peso maior
                    .FirstOrDefault();

                if (leituraInicioCiclo != null)
                {
                    var diasUso = (
                        ultimaLeitura.DataHoraRegisto - leituraInicioCiclo.DataHoraRegisto
                    ).TotalDays;
                    var gasGasto = leituraInicioCiclo.PesoKg - ultimaLeitura.PesoKg;

                    // Só calcula se tiver passado pelo menos meio dia e gasto algo relevante
                    if (diasUso > 0.5 && gasGasto > 0.5)
                    {
                        consumoDiarioMedio = gasGasto / diasUso;
                    }
                }
            }
            // ------------------------------------

            var listaResposta = new List<MedicaoResponse>();

            foreach (var item in medicoesBrutas.Take(50)) // Retorna só as 50 últimas para o frontend ser rápido
            {
                double pesoDoGasAtual = item.PesoKg - pesoTara;
                if (pesoDoGasAtual < 0)
                    pesoDoGasAtual = 0;

                double porcentagem = 0;
                if (capacidadeTotal > 0)
                {
                    porcentagem = (pesoDoGasAtual / capacidadeTotal) * 100;
                }

                // Cálculo Financeiro
                decimal valorDoGasAtual = 0;
                if (capacidadeTotal > 0 && precoBotijao > 0)
                {
                    valorDoGasAtual =
                        (decimal)pesoDoGasAtual * (precoBotijao / (decimal)capacidadeTotal);
                }
                decimal valorQueimado = precoBotijao - valorDoGasAtual;

                // Previsão de Término (Agora usando a média inteligente!)
                int diasRestantes = 0;
                if (consumoDiarioMedio > 0 && pesoDoGasAtual > 0)
                {
                    diasRestantes = (int)(pesoDoGasAtual / consumoDiarioMedio);
                }

                // Status Textual
                string statusTexto = "Normal";
                if (item.VazamentoDetectado)
                    statusTexto = "PERIGO: VAZAMENTO!";
                else if (porcentagem > 110)
                    statusTexto = "Erro Calibração";
                else if (porcentagem <= 0)
                    statusTexto = "Vazio";
                else if (porcentagem < 20)
                    statusTexto = "Reserva";

                listaResposta.Add(
                    new MedicaoResponse
                    {
                        Id = item.Id,
                        IdDispositivo = item.IdDispositivo,
                        PesoTotalKg = Math.Round(item.PesoKg, 2),
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

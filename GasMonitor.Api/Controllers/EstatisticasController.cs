using System.Globalization;
using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstatisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public EstatisticasController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public async Task<ActionResult<EstatisticasResponse>> GetEstatisticas(
            [FromQuery] DateTime? inicio,
            [FromQuery] DateTime? fim
        )
        {
            // 1. Começa a preparar a consulta (ainda não vai ao banco)
            var query = _contexto.Medicoes.AsQueryable();

            // 2. Aplica os filtros de data se o usuário os enviou
            if (inicio.HasValue)
            {
                // Converte para UTC para garantir compatibilidade com o banco
                var dataInicio = inicio.Value.ToUniversalTime();
                query = query.Where(m => m.DataHoraRegisto >= dataInicio);
            }

            if (fim.HasValue)
            {
                // Adiciona 23:59:59 para pegar o dia inteiro final
                var dataFim = fim.Value.Date.AddDays(1).AddTicks(-1).ToUniversalTime();
                query = query.Where(m => m.DataHoraRegisto <= dataFim);
            }

            // 3. Executa a consulta no Banco de Dados
            var historico = await query.OrderBy(m => m.DataHoraRegisto).ToListAsync();

            // Se não houver dados no período, retorna vazio
            if (!historico.Any())
            {
                return NoContent();
            }

            var stats = new EstatisticasResponse();

            // --- A. Dados Gerais ---
            stats.DataAquisicao = historico.First().DataHoraRegisto;

            // Calcula dias de uso reais dentro do período selecionado
            stats.DiasDeUso = (int)
                (historico.Last().DataHoraRegisto - stats.DataAquisicao).TotalDays;
            if (stats.DiasDeUso == 0)
                stats.DiasDeUso = 1;

            // --- B. Loop de Análise de Consumo ---
            for (int i = 1; i < historico.Count; i++)
            {
                var leituraAnterior = historico[i - 1];
                var leituraAtual = historico[i];

                // Quanto consumiu entre uma leitura e outra?
                double diferenca = leituraAnterior.PesoKg - leituraAtual.PesoKg;

                // Filtros para ignorar ruído ou troca de botijão (peso subindo)
                // Aceitamos apenas consumos positivos entre 5 gramas e 5 quilos (ex: fugas massivas)
                if (diferenca > 0.005 && diferenca < 5.0)
                {
                    // Data local para saber se é dia/noite corretamente
                    var dataLocal = leituraAtual.DataHoraRegisto.ToLocalTime();

                    // 1. Análise: Dia da Semana (Segunda, Terça...)
                    var diaSemana = dataLocal.ToString("dddd", new CultureInfo("pt-BR"));
                    if (!stats.ConsumoPorDiaSemana.ContainsKey(diaSemana))
                        stats.ConsumoPorDiaSemana[diaSemana] = 0;
                    stats.ConsumoPorDiaSemana[diaSemana] += diferenca;

                    // 2. Análise: Mês (Novembro/2025)
                    var mes = dataLocal.ToString("MMMM/yyyy", new CultureInfo("pt-BR"));
                    if (!stats.ConsumoPorMes.ContainsKey(mes))
                        stats.ConsumoPorMes[mes] = 0;
                    stats.ConsumoPorMes[mes] += diferenca;

                    // 3. Análise: Turno (Dia vs Noite)
                    var hora = dataLocal.Hour;
                    if (hora >= 6 && hora < 18)
                    {
                        stats.ConsumoDiaKg += diferenca; // Dia (06h às 18h)
                    }
                    else
                    {
                        stats.ConsumoNoiteKg += diferenca; // Noite (18h às 06h)
                    }
                }
            }

            // --- C. Fechamento e Totais ---
            double consumoTotalPeriodo = stats.ConsumoDiaKg + stats.ConsumoNoiteKg;

            stats.MediaConsumoDiarioKg = Math.Round(consumoTotalPeriodo / stats.DiasDeUso, 3);

            stats.TurnoMaisConsumidor =
                stats.ConsumoNoiteKg > stats.ConsumoDiaKg ? "Noite 🌑" : "Dia ☀️";

            // Arredondamentos finais para exibição limpa
            stats.ConsumoDiaKg = Math.Round(stats.ConsumoDiaKg, 2);
            stats.ConsumoNoiteKg = Math.Round(stats.ConsumoNoiteKg, 2);

            // Arredonda os dicionários também
            foreach (var key in stats.ConsumoPorDiaSemana.Keys.ToList())
                stats.ConsumoPorDiaSemana[key] = Math.Round(stats.ConsumoPorDiaSemana[key], 2);

            return Ok(stats);
        }
    }
}

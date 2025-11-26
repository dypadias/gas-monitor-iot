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
            var query = _contexto.Medicoes.AsQueryable();

            // Variáveis de data para reutilizar nas duas queries (Medições e Trocas)
            DateTime? dataInicioUtc = null;
            DateTime? dataFimUtc = null;

            if (inicio.HasValue)
            {
                dataInicioUtc = DateTime.SpecifyKind(inicio.Value, DateTimeKind.Utc);
                query = query.Where(m => m.DataHoraRegisto >= dataInicioUtc.Value);
            }

            if (fim.HasValue)
            {
                // TRUQUE DO FUSO HORÁRIO:
                // Adicionamos 2 dias ao fim para garantir que pegamos as horas noturnas do Brasil
                // que já viraram "amanhã" no UTC.
                dataFimUtc = DateTime.SpecifyKind(fim.Value.Date.AddDays(2), DateTimeKind.Utc);
                query = query.Where(m => m.DataHoraRegisto < dataFimUtc.Value);
            }

            var historico = await query.OrderBy(m => m.DataHoraRegisto).ToListAsync();

            // Inicializa resposta vazia se não houver medições, mas continua para ver se há trocas
            var stats = new EstatisticasResponse();

            if (historico.Any())
            {
                stats.DataAquisicao = historico.First().DataHoraRegisto;
                stats.DiasDeUso = (int)
                    (historico.Last().DataHoraRegisto - stats.DataAquisicao).TotalDays;
                if (stats.DiasDeUso == 0)
                    stats.DiasDeUso = 1;

                // Loop de Análise de Consumo (Mantém lógica igual)
                for (int i = 1; i < historico.Count; i++)
                {
                    var leituraAnterior = historico[i - 1];
                    var leituraAtual = historico[i];
                    double diferenca = leituraAnterior.PesoKg - leituraAtual.PesoKg;

                    if (diferenca > 0.005 && diferenca < 5.0)
                    {
                        var dataLocal = leituraAtual.DataHoraRegisto.ToLocalTime();
                        var diaSemana = dataLocal.ToString("dddd", new CultureInfo("pt-BR"));
                        var mes = dataLocal.ToString("MMMM/yyyy", new CultureInfo("pt-BR"));

                        if (!stats.ConsumoPorDiaSemana.ContainsKey(diaSemana))
                            stats.ConsumoPorDiaSemana[diaSemana] = 0;
                        stats.ConsumoPorDiaSemana[diaSemana] += diferenca;

                        if (!stats.ConsumoPorMes.ContainsKey(mes))
                            stats.ConsumoPorMes[mes] = 0;
                        stats.ConsumoPorMes[mes] += diferenca;

                        if (dataLocal.Hour >= 6 && dataLocal.Hour < 18)
                            stats.ConsumoDiaKg += diferenca;
                        else
                            stats.ConsumoNoiteKg += diferenca;
                    }
                }

                // Totais
                double consumoTotal = stats.ConsumoDiaKg + stats.ConsumoNoiteKg;
                stats.MediaConsumoDiarioKg = Math.Round(consumoTotal / stats.DiasDeUso, 3);
                stats.TurnoMaisConsumidor =
                    stats.ConsumoNoiteKg > stats.ConsumoDiaKg ? "Noite 🌑" : "Dia ☀️";
                stats.ConsumoDiaKg = Math.Round(stats.ConsumoDiaKg, 2);
                stats.ConsumoNoiteKg = Math.Round(stats.ConsumoNoiteKg, 2);
            }

            // --- QUERY SEPARADA PARA O HISTÓRICO DE PREÇOS ---
            // Usamos as mesmas datas "generosas" calculadas acima
            var queryTrocas = _contexto.HistoricoTrocas.AsQueryable();

            if (dataInicioUtc.HasValue)
                queryTrocas = queryTrocas.Where(t => t.DataTroca >= dataInicioUtc.Value);

            if (dataFimUtc.HasValue)
                queryTrocas = queryTrocas.Where(t => t.DataTroca < dataFimUtc.Value);

            var trocas = await queryTrocas.OrderBy(t => t.DataTroca).ToListAsync();

            foreach (var troca in trocas)
            {
                stats.HistoricoPrecos.Add(
                    new HistoricoPonto
                    {
                        Data = troca.DataTroca, // O Frontend converte para Local Time
                        Valor = troca.PrecoPago,
                        Etiqueta = troca.NomeProduto,
                    }
                );
            }

            return Ok(stats);
        }
    }
}

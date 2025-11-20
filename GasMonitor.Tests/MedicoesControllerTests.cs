using GasMonitor.Api.Controllers;
using GasMonitor.Api.Data;
using GasMonitor.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit; // A biblioteca de testes

namespace GasMonitor.Tests
{
    public class MedicoesControllerTests
    {
        // Esta função cria um "Banco de Dados Falso" na memória RAM
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Cria um nome único
                .Options;
            
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact] // Indica que isto é um teste
        public async Task Deve_Calcular_Porcentagem_Corretamente_Botijao_Cheio()
        {
            // --- 1. ARRANGE (Preparar o Cenário) ---
            var dbContext = GetDatabaseContext();
            
            // Vamos inserir uma medição falsa no banco em memória
            // Tara (15kg) + Gás (13kg) = 28kg (Isto deve dar 100%)
            dbContext.Medicoes.Add(new Medicao 
            { 
                IdDispositivo = "TESTE_01", 
                PesoKg = 28.0, 
                DataHoraRegisto = DateTime.Now 
            });
            await dbContext.SaveChangesAsync();

            // Instancia o Controller passando o banco falso
            var controller = new MedicoesController(dbContext);

            // --- 2. ACT (Executar a Ação) ---
            // Chamamos o método que queremos testar
            var resultado = await controller.GetTodasMedicoes();

            // --- 3. ASSERT (Verificar o Resultado) ---
            
            // Extraímos o valor de dentro da resposta "Ok"
            var actionResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var listaRetornada = Assert.IsAssignableFrom<IEnumerable<MedicaoResponse>>(actionResult.Value);
            
            // Pegamos o primeiro item da lista
            var medicaoProcessada = listaRetornada.First();

            // A VERIFICAÇÃO FINAL:
            // Se o peso é 28kg, a percentagem TEM de ser 100.
            Assert.Equal(100, medicaoProcessada.PorcentagemGas);
            Assert.Equal("Normal", medicaoProcessada.Status);
        }

        [Fact]
        public async Task Deve_Calcular_Zero_Se_Peso_Menor_Que_Tara()
        {
            // --- ARRANGE ---
            var dbContext = GetDatabaseContext();
            // Peso 10kg (Menor que a tara de 15kg). Deve dar 0% e não negativo.
            dbContext.Medicoes.Add(new Medicao { IdDispositivo = "TESTE_02", PesoKg = 10.0, DataHoraRegisto = DateTime.Now });
            await dbContext.SaveChangesAsync();

            var controller = new MedicoesController(dbContext);

            // --- ACT ---
            var resultado = await controller.GetTodasMedicoes();

            // --- ASSERT ---
            var actionResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<MedicaoResponse>>(actionResult.Value);
            
            Assert.Equal(0, lista.First().PorcentagemGas);
            Assert.Equal("Vazio", lista.First().Status);
        }
    }
}
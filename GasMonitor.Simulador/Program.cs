using System.Net.Http.Json;

// --- CONFIGURAÇÃO ---
// IMPORTANTE: Verifique a porta no seu Swagger quando rodar a API!
const string URL_API = "http://localhost:5092/api/medicoes"; 
const string ID_DISPOSITIVO = "SIMULADOR_BOTIJAO_01";
const int INTERVALO_SEGUNDOS = 3; // Envia dados a cada 3 segundos

// --- ESTADO DO BOTIJÃO ---
// Vamos simular um botijão P13 Cheio:
// Tara (15kg) + Gás (13kg) = 28kg Total
double pesoAtual = 28.0; 

// Cria o cliente HTTP (o carteiro que vai levar a mensagem)
using var httpClient = new HttpClient();

Console.WriteLine("--- INICIANDO SIMULADOR DE ESP32 ---");
Console.WriteLine($"Alvo: {URL_API}");
Console.WriteLine($"ID: {ID_DISPOSITIVO}");
Console.WriteLine("Pressione Ctrl+C para parar.");
Console.WriteLine("------------------------------------");

var random = new Random();

while (true)
{
    // 1. Simular consumo de gás
    // Vamos tirar entre 100g (0.1) e 500g (0.5) a cada ciclo para ser rápido
    double consumo = random.NextDouble() * (0.5 - 0.1) + 0.1;
    pesoAtual -= consumo;

    // Não deixar baixar do peso da tara (15kg)
    if (pesoAtual < 15.0) 
    {
        pesoAtual = 15.0;
        Console.WriteLine("Botijão VAZIO! Reiniciando simulação...");
        pesoAtual = 28.0; // Enche o botijão de novo magicamente
    }

    // 2. Criar o objeto de dados (igual ao MedicaoInput da API)
    var dados = new
    {
        idDispositivo = ID_DISPOSITIVO,
        pesoKg = Math.Round(pesoAtual, 2) // Arredonda para 2 casas decimais
    };

    try
    {
        // 3. Enviar para a API (POST)
        Console.Write($"Enviando leitura {dados.pesoKg} Kg... ");
        
        var resposta = await httpClient.PostAsJsonAsync(URL_API, dados);

        if (resposta.IsSuccessStatusCode)
        {
            Console.WriteLine("Sucesso! (200 OK)");
        }
        else
        {
            Console.WriteLine($"Erro: {resposta.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nALERTA: Não foi possível conectar à API. Ela está rodando?");
        Console.WriteLine($"Erro: {ex.Message}");
    }

    // 4. Esperar X segundos antes da próxima leitura
    await Task.Delay(INTERVALO_SEGUNDOS * 1000);
}
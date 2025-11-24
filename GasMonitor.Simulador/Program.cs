using System.Net.Http.Json;

// --- CONFIGURAÇÃO ---
// IMPORTANTE: Verifique a porta da sua API!
const string URL_API = "http://localhost:5092/api/medicoes";
const string ID_DISPOSITIVO = "SIMULADOR_CASA_01";
const int INTERVALO_SEGUNDOS = 2; // Envia dados a cada 2 segundos

// --- ESTADO INICIAL ---
double pesoRealDoBotijao = 28.0; // Começa cheio (13kg Gás + 15kg Tara)
bool fogaoLigado = false;
bool modoVazamento = false;
int ciclosParaMudarEstado = 0; // Contador para decidir quando ligar/desligar fogão

using var httpClient = new HttpClient();
var random = new Random();

Console.Clear();
Console.WriteLine("==============================================");
Console.WriteLine("🔥 SIMULADOR DE BOTIJÃO IoT (V2.0 Realista)");
Console.WriteLine($"📡 Alvo: {URL_API}");
Console.WriteLine("==============================================");
Console.WriteLine("COMANDOS:");
Console.WriteLine(" [V] - Simular/Parar VAZAMENTO DE GÁS 🚨");
Console.WriteLine(" [R] - Trocar Botijão (Recarregar para 28kg) 🔄");
Console.WriteLine(" [Espaço] - Ligar/Desligar Fogão Manualmente 🔥");
Console.WriteLine("==============================================\n");

// Loop principal de simulação
while (true)
{
    // 1. VERIFICAR TECLAS (Interatividade)
    if (Console.KeyAvailable)
    {
        var tecla = Console.ReadKey(true).Key;
        if (tecla == ConsoleKey.V)
        {
            modoVazamento = !modoVazamento;
            Console.WriteLine(
                $"\n>>> MODO VAZAMENTO: {(modoVazamento ? "ATIVADO ⚠️" : "DESATIVADO")}"
            );
        }
        else if (tecla == ConsoleKey.R)
        {
            pesoRealDoBotijao = 28.0;
            Console.WriteLine("\n>>> BOTIJÃO TROCADO! (Carga completa)");
        }
        else if (tecla == ConsoleKey.Spacebar)
        {
            fogaoLigado = !fogaoLigado;
            ciclosParaMudarEstado = 10; // Mantém o estado manual por um tempo
            Console.WriteLine($"\n>>> FOGÃO {(fogaoLigado ? "LIGADO 🔥" : "DESLIGADO 🛑")}");
        }
    }

    // 2. IA DE COMPORTAMENTO (Simular rotina de casa)
    // Se não houver intervenção manual, o sistema decide sozinho
    if (ciclosParaMudarEstado <= 0)
    {
        // 20% de chance de mudar de estado (Ligar/Desligar)
        if (random.NextDouble() < 0.2)
        {
            fogaoLigado = !fogaoLigado;
            // Define quanto tempo vai ficar neste estado (entre 5 e 15 ciclos)
            ciclosParaMudarEstado = random.Next(5, 15);
        }
    }
    else
    {
        ciclosParaMudarEstado--;
    }

    // 3. CÁLCULO DO CONSUMO FÍSICO
    double consumoNesteCiclo = 0;

    if (fogaoLigado)
    {
        // Fogo alto consome rápido (entre 50g e 100g por ciclo acelerado)
        consumoNesteCiclo = (random.NextDouble() * 0.05) + 0.05;
    }

    if (modoVazamento)
    {
        // Vazamento soma ao consumo (perda constante)
        consumoNesteCiclo += 0.02;
    }

    pesoRealDoBotijao -= consumoNesteCiclo;

    // Limite físico (não pode pesar menos que 14.5kg se for ferro puro)
    if (pesoRealDoBotijao < 14.5)
        pesoRealDoBotijao = 14.5;

    // 4. SIMULAÇÃO DO SENSOR (Ruído)
    // Células de carga reais oscilam. O valor lido nunca é exato.
    // Adicionamos um "ruído" aleatório de +/- 5 gramas
    double ruidoSensor = (random.NextDouble() * 0.010) - 0.005;
    double pesoLidoPeloSensor = pesoRealDoBotijao + ruidoSensor;

    // 5. ENVIAR PARA A API
    var dados = new
    {
        idDispositivo = ID_DISPOSITIVO,
        pesoKg = Math.Round(pesoLidoPeloSensor, 3), // 3 casas decimais
        temVazamento = modoVazamento, // Envia o sinal do sensor MQ-2
    };

    try
    {
        // Mostra status visual no console
        string iconeStatus = fogaoLigado ? "🔥 Cozinhando" : "💤 Standby";
        if (modoVazamento)
            iconeStatus = "🚨 VAZANDO!";

        Console.Write($"[{DateTime.Now:HH:mm:ss}] Peso: {dados.pesoKg:F2}kg | {iconeStatus} | ");

        var resposta = await httpClient.PostAsJsonAsync(URL_API, dados);

        if (resposta.IsSuccessStatusCode)
        {
            Console.WriteLine("✅ Enviado");
        }
        else
        {
            Console.WriteLine($"❌ Erro API: {resposta.StatusCode}");
        }
    }
    catch
    {
        Console.WriteLine("❌ Erro: API Offline?");
    }

    // Espera X segundos antes da próxima leitura
    await Task.Delay(INTERVALO_SEGUNDOS * 1000);
}

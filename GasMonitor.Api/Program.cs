using GasMonitor.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Tenta pegar a string de conexão das Variáveis de Ambiente (Render/Supabase)
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// 2. Se vier vazia (estamos no PC local), pega do appsettings.json
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddControllers();

// Configurar Banco de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// --- ALTERAÇÃO 1: Adicionar o Serviço de CORS ---
// Estamos a criar uma política chamada "PermitirTudo" (útil para desenvolvimento)
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "PermitirTudo",
        policy =>
        {
            policy
                .AllowAnyOrigin() // Aceita pedidos de qualquer site/IP
                .AllowAnyMethod() // Aceita GET, POST, PUT, DELETE
                .AllowAnyHeader(); // Aceita qualquer tipo de cabeçalho
        }
    );
});

// ------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ALTERAÇÃO 2: Ativar o CORS ---
// IMPORTANTE: Tem de ser ANTES de "UseAuthorization" e "MapControllers"
app.UseCors("PermitirTudo");

// ----------------------------------

app.UseAuthorization();

app.MapControllers();

// Isto cria um "escopo" temporário para aceder ao banco de dados no arranque
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var contexto = services.GetRequiredService<ApplicationDbContext>();
        // Este comando cria o banco e as tabelas se elas não existirem
        contexto.Database.EnsureCreated();
        Console.WriteLine("Banco de dados verificado/criado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar o banco de dados: {ex.Message}");
    }
}

app.Run();

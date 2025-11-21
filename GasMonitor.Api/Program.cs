using GasMonitor.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ler a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();

// Configurar Banco de Dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- ALTERAÇÃO 1: Adicionar o Serviço de CORS ---
// Estamos a criar uma política chamada "PermitirTudo" (útil para desenvolvimento)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo",
        policy =>
        {
            policy.AllowAnyOrigin()  // Aceita pedidos de qualquer site/IP
                  .AllowAnyMethod()  // Aceita GET, POST, PUT, DELETE
                  .AllowAnyHeader(); // Aceita qualquer tipo de cabeçalho
        });
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

app.Run();
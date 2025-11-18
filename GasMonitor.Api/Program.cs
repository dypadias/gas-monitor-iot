using GasMonitor.Api.Data;      // <--- ADICIONADO PARA CORRIGIR O ERRO CS0246
using Microsoft.EntityFrameworkCore; // <--- ADICIONADO PARA CORRIGIR O ERRO CS1061

var builder = WebApplication.CreateBuilder(args);

// Vamos ler a string de conexão do nosso ficheiro appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- ADICIONADO ---
// 1. Adiciona os serviços para os "Controllers".
// Isto faz com que o .NET procure e reconheça o seu "MedicoesController.cs".
builder.Services.AddControllers();

// Diz ao .NET para usar o EF Core com o PostgreSQL, usando a string de conexão
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// --- ADICIONADO ---
// 2. Mapeia (ativa) os Controllers que foram encontrados.
// Isto ativa as rotas (URLs) definidas no seu "MedicoesController".
app.MapControllers();

app.Run();
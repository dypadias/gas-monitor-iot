# ⚖️ GasMonitor IoT

Sistema profissional de monitorização de nível de gás para botijões domésticos (P13). O projeto recolhe dados de peso em tempo real, armazena num banco de dados e apresenta o consumo e alertas numa dashboard web.

## 🚀 Arquitetura do Projeto

O sistema segue uma arquitetura moderna e desacoplada:

* **Hardware (Simulado):** Dispositivo IoT que lê uma célula de carga e envia o peso via HTTP POST.
* **Backend (API):** ASP.NET Core 8 Web API. Responsável por validar, processar regras de negócio (cálculo de %) e persistir dados.
* **Database:** PostgreSQL com Entity Framework Core (Code-First).
* **Frontend:** HTML5, CSS3 e Chart.js. Consome a API para apresentar gráficos em tempo real.
* **Testes:** xUnit com Banco em Memória para validação da lógica de negócios.

## 🛠️ Tecnologias Utilizadas

* **C# / .NET 8**
* **PostgreSQL** (Docker ou Local)
* **Entity Framework Core**
* **HTML / JavaScript / Chart.js**
* **Git / GitHub**

## ⚙️ Como Rodar o Projeto

### Pré-requisitos
* .NET SDK 8.0
* PostgreSQL a rodar (Porta 5432)

### 1. Configurar o Banco de Dados
Certifique-se de que a ConnectionString no `appsettings.json` da API aponta para o seu Postgres.
```bash
cd GasMonitor.Api
dotnet ef database update
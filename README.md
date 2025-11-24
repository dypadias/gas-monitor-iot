# 🔥 SmartGás Monitor IoT

> **Plataforma inteligente de gestão de consumo e segurança para gás residencial (GLP).**

O **SmartGás** é uma solução Full-Stack IoT que transforma um botijão de gás comum num dispositivo inteligente. Através de sensores e uma dashboard moderna, o sistema monitoriza o peso em tempo real, detecta vazamentos, calcula custos financeiros e prevê a data de término do gás.

---

## 🚀 Funcionalidades Principais

### 📊 Dashboard Comercial
* **Monitoramento em Tempo Real:** Visualização gráfica do nível de gás com atualização via WebSocket/Timer.
* **Burndown Chart:** Gráfico comparativo entre o consumo **Real** vs **Ideal/Médio**.
* **Visualização Física:** Componente visual animado (CSS Puro) que representa o nível do líquido.

### 💰 Inteligência Financeira & BI
* **Cálculo de ROI:** Acompanhamento do **Valor Gasto** e **Valor Restante** (R$) em tempo real.
* **Previsão de Término:** Estimativa de quantos dias o gás dura baseado na média de consumo diário.
* **Relatórios:** Análise de consumo por **Dia da Semana** e **Turno** (Dia vs Noite).

### 🛡️ Segurança Ativa
* **Detecção de Vazamento:** Integração (preparada) para sensor MQ-2.
* **Alerta Crítico:** Interface visual de emergência (Overlay Vermelho) em caso de perigo.

### ⚙️ Flexibilidade
* **Multi-Produto:** Configurável para P13, P45, ou qualquer recipiente (Tara/Líquido personalizáveis).
* **Histórico de Trocas:** Registo de datas e preços pagos em cada recarga.

---

## 🛠️ Stack Tecnológica

O projeto foi desenvolvido seguindo as melhores práticas de arquitetura moderna:

* **Backend:** .NET 8 (ASP.NET Core Web API)
* **Frontend:** Blazor WebAssembly (SPA)
* **Banco de Dados:** PostgreSQL
* **ORM:** Entity Framework Core (Code-First)
* **Visualização:** Chart.js (integrado via JS Interop)
* **Hardware (Simulação):** Console App em .NET simulando ESP32 + Célula de Carga + Sensor de Gás.

---

## 📦 Como Rodar o Projeto

### Pré-requisitos
* .NET SDK 8.0
* PostgreSQL (Docker ou Local) rodando na porta 5432.

### 1. Configuração do Banco
Certifique-se que a ConnectionString está configurada (ou use a padrão de dev).
```bash
cd GasMonitor.Api
dotnet ef database update
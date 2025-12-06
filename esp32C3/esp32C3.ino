#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "HX711.h"

// ==========================================
// 1. CONFIGURAÇÕES DE REDE (EDITAR AQUI)
// ==========================================
const char* ssid     = "Senha123";
const char* password = "parangarico";

// ==========================================
// 2. CONFIGURAÇÃO DA API
// ==========================================
const char* serverUrl = "https://gas-monitor-iot.onrender.com/api/medicoes";
const char* idDispositivo = "ESP32_C3_COZINHA"; // Nome que vai aparecer no site

// ==========================================
// 3. HARDWARE E CALIBRAÇÃO
// ==========================================
const int PIN_MQ2_ANALOG = 3;   // Sensor de Gás
const int PIN_LOADCELL_DT = 6;  // HX711 Data
const int PIN_LOADCELL_SCK = 7; // HX711 Clock

// O TEU NÚMERO MÁGICO DESCOBERTO:
const float CALIBRATION_FACTOR = 24810.0; 

HX711 scale;

void setup() {
  Serial.begin(115200);
  delay(3000); 

  // --- INICIAR SENSORES ---
  Serial.println("\n--- INICIANDO SISTEMA GAS MONITOR ---");
  pinMode(PIN_MQ2_ANALOG, INPUT);
  
  scale.begin(PIN_LOADCELL_DT, PIN_LOADCELL_SCK);
  
  Serial.println("Calibrando Balança...");
  scale.set_scale(CALIBRATION_FACTOR);
  
  // IMPORTANTE: Como não temos a madeira fixa, fazemos o Zero aqui.
  // A balança deve estar vazia ao ligar!
  scale.tare(); 
  
  Serial.println("Balança Zerada! Pode colocar o botijão.");

  // --- CONEXÃO WIFI ---
  Serial.print("Conectando ao Wi-Fi: ");
  Serial.println(ssid);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  int tentativas = 0;
  while (WiFi.status() != WL_CONNECTED && tentativas < 20) {
    delay(500);
    Serial.print(".");
    tentativas++;
  }
  
  if(WiFi.status() == WL_CONNECTED){
    Serial.println("\nWi-Fi Conectado!");
    Serial.print("IP: ");
    Serial.println(WiFi.localIP());
  } else {
    Serial.println("\nFalha no Wi-Fi. O sistema tentará reconectar no loop.");
  }
}

void loop() {
  // 1. LER PESO
  float pesoLido = 0.0;
  
  if (scale.is_ready()) {
    pesoLido = scale.get_units(10); // Média de 10 leituras para estabilidade
    if (pesoLido < 0) pesoLido = 0.0; // Evita negativos leves
  } else {
    Serial.println("ERRO: HX711 desconectado!");
  }

  // 2. LER GÁS
  int gasRaw = analogRead(PIN_MQ2_ANALOG);
  // Ajuste este valor (2000) testando com um isqueiro perto do sensor
  bool temVazamento = (gasRaw > 2500); 

  Serial.printf("[SENSOR] Peso: %.3f kg | Gás: %d | Vazamento: %s\n", 
                pesoLido, gasRaw, temVazamento ? "SIM 🚨" : "NÃO");

  // 3. ENVIAR PARA A NUVEM
  if(WiFi.status() == WL_CONNECTED){
    WiFiClientSecure client;
    client.setInsecure(); // Ignora SSL (Necessário para ESP32 simples)
    
    HTTPClient http;
    http.begin(client, serverUrl);
    http.addHeader("Content-Type", "application/json");

    // Criar JSON
    JsonDocument doc;
    doc["idDispositivo"] = idDispositivo;
    doc["pesoKg"] = pesoLido;      // Envia o peso real
    doc["temVazamento"] = temVazamento;

    String jsonOutput;
    serializeJson(doc, jsonOutput);

    // Envio POST
    int httpResponseCode = http.POST(jsonOutput);

    if(httpResponseCode > 0){
      Serial.println("✅ API: Dados enviados com sucesso (" + String(httpResponseCode) + ")");
    } else {
      Serial.print("❌ API: Erro no envio: ");
      Serial.println(http.errorToString(httpResponseCode));
    }
    http.end();
  } else {
    Serial.println("⚠️ Wi-Fi Desconectado. Tentando reconectar...");
    WiFi.reconnect();
  }

  // Espera 5 segundos antes da próxima leitura
  // (Pode aumentar para 30s ou 60s no uso real para poupar bateria/dados)
  delay(5000); 
}
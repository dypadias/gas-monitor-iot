namespace GasMonitor.Api.Models
{
    /// <summary>
    /// Representa os dados brutos que recebemos do dispositivo (ESP32 ou Simulador).
    /// </summary>
    public class MedicaoInput
    {
        /// <summary>
        /// O ID único do dispositivo (para sabermos qual botijão está medindo).
        /// </summary>
        public string IdDispositivo { get; set; } = string.Empty;

        /// <summary>
        /// O peso lido pela célula de carga, em quilogramas (ex: 10.5).
        /// </summary>
        public double PesoKg { get; set; }

        /// <summary>
        /// NOVO: Indica se o sensor MQ-2 detetou gás no ambiente (True = Perigo).
        /// </summary>
        public bool TemVazamento { get; set; }
    }
}

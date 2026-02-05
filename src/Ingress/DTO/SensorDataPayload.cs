namespace Sensors.Models
{
    public class SensorDataPayload
    {
        public string PlotId { get; set; } = string.Empty;
        public double SoilMoisture { get; set; }
        public double Temperature { get; set; }
        public double Precipitation { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
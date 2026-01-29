namespace Sensors.Models
{
    public class SensorConfig
    {
        public KafkaConfig Kafka { get; set; } = new KafkaConfig();
        public string PlotId { get; set; } = "plot-001";
        public int TriggerIntervalMs { get; set; } = 5000;
    }

    public class KafkaConfig
    {
        public string Broker { get; set; } = "kafka:9092";
        public string Topic { get; set; } = "sensor-data";
    }
}

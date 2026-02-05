namespace IngressApi.Models
{
    public class KafkaConfig
    {
        public string Broker { get; set; } = "kafka:9092";
        public string Topic { get; set; } = "sensor-data";
        public string GroupId { get; set; } = "ingress-api-group";
    }
}

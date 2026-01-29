using System.Text.Json;
using Confluent.Kafka;
using Sensors.Models;

namespace Sensors
{
    public class SensorDataProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public SensorDataProducer(string broker, string topic)
        {
            var config = new ProducerConfig { BootstrapServers = broker };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = topic;
        }

        public async Task ProduceAsync(SensorDataRequest data)
        {
            var json = JsonSerializer.Serialize(data);
            await _producer.ProduceAsync(_topic, new Message<string, string> { Key = data.PlotId, Value = json });
        }
    }
}

using Confluent.Kafka;
using IngressApi.Models;
using Sensors.Models;
using System.Text.Json;
using IngressApi.Repositories;

namespace IngressApi.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly ISensorDataRepository _repository;

        public KafkaConsumerService(
            KafkaConfig kafkaConfig,
            ILogger<KafkaConsumerService> logger,
            ISensorDataRepository repository)
        {
            _kafkaConfig = kafkaConfig;
            _logger = logger;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conf = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.Broker,
                GroupId = _kafkaConfig.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
            consumer.Subscribe(_kafkaConfig.Topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    var data = JsonSerializer.Deserialize<SensorDataPayload>(result.Message.Value);
                    _logger.LogInformation("Received message: {Message}", result.Message.Value);
                    _logger.LogInformation("Deserialized data: {@Data}", data);

                    if (data != null)
                    {
                        await _repository.SaveAsync(data, stoppingToken);
                        _logger.LogInformation("Data saved to InfluxDB.");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while consuming Kafka message.");
                }
            }
        }
    }
}
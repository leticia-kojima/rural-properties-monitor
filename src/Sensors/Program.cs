using Sensors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Sensors.Models;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var configRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var sensorConfig = new SensorConfig();
        configRoot.GetSection("SensorConfig").Bind(sensorConfig);

        var producer = new SensorDataProducer(sensorConfig.Kafka.Broker, sensorConfig.Kafka.Topic);
        var emulator = new SensorEmulator(producer, sensorConfig.PlotId, sensorConfig.TriggerIntervalMs);

        Console.WriteLine($"Starting sensor emulator for plot '{sensorConfig.PlotId}' to Kafka broker '{sensorConfig.Kafka.Broker}', topic '{sensorConfig.Kafka.Topic}', interval '{sensorConfig.TriggerIntervalMs}' ms");
        await emulator.RunAsync();
    }
}

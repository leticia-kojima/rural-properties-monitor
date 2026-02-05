using IngressApi.Models;
using Sensors.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace IngressApi.Repositories
{
    public class InfluxSensorDataRepository : ISensorDataRepository
    {
        private readonly InfluxDbConfig _config;
        private readonly InfluxDBClient _client;

        public InfluxSensorDataRepository(InfluxDbConfig config)
        {
            _config = config;
            _client = new InfluxDBClient(_config.Url, _config.Token);
        }

        public async Task SaveAsync(SensorDataPayload data, CancellationToken cancellationToken = default)
        {
            var point = PointData
                .Measurement("sensor_data")
                .Tag("plotId", data.PlotId)
                .Field("soilMoisture", data.SoilMoisture)
                .Field("temperature", data.Temperature)
                .Field("precipitation", data.Precipitation)
                .Timestamp(data.Timestamp, WritePrecision.Ns);

            var writeApi = _client.GetWriteApiAsync();
            await writeApi.WritePointAsync(point, _config.Bucket, _config.Org, cancellationToken);
        }
    }
}
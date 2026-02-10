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

        public async Task<List<SensorDataPayload>> GetByPlotIdsAsync(
            List<string> plotIds, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            var results = new List<SensorDataPayload>();
            var queryApi = _client.GetQueryApi();

            var plotIdFilter = string.Join(" or ", plotIds.Select(id => $"r.plotId == \"{id}\""));

            var fluxQuery = $@"
                from(bucket: ""{_config.Bucket}"")
                |> range(start: ""{startDate:yyyy-MM-ddTHH:mm:ssZ}"", stop: ""{endDate:yyyy-MM-ddTHH:mm:ssZ}"")
                |> filter(fn: (r) => r._measurement == ""sensor_data"")
                |> filter(fn: (r) => {plotIdFilter})
                |> pivot(rowKey: [""_time"", ""plotId""], columnKey: [""_field""], valueColumn: ""_value"")
            ";

            var tables = await queryApi.QueryAsync(fluxQuery, _config.Org, cancellationToken);

            foreach (var table in tables)
            {
                foreach (var record in table.Records)
                {
                    results.Add(new SensorDataPayload
                    {
                        PlotId = record.Values["plotId"]?.ToString() ?? string.Empty,
                        SoilMoisture = Convert.ToDouble(record.Values["soilMoisture"] ?? 0),
                        Temperature = Convert.ToDouble(record.Values["temperature"] ?? 0),
                        Precipitation = Convert.ToDouble(record.Values["precipitation"] ?? 0),
                        Timestamp = record.GetTimeInDateTime() ?? DateTime.UtcNow
                    });
                }
            }

            return results;
        }
    }
}
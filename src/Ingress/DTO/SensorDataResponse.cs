namespace IngressApi.DTO;

// Resposta com dados agregados de sensores
public class SensorDataResponse
{
    public string PlotId { get; set; } = string.Empty;
    public List<HourlyAverage> HourlyAverages { get; set; } = new();
    public PeriodAverage PeriodAverage { get; set; } = new();
}

// Média por hora
public class HourlyAverage
{
    public DateTime Hour { get; set; }
    public double SoilMoisture { get; set; }
    public double Temperature { get; set; }
    public double Precipitation { get; set; }
}

// Média do período completo
public class PeriodAverage
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double SoilMoisture { get; set; }
    public double Temperature { get; set; }
    public double Precipitation { get; set; }
}


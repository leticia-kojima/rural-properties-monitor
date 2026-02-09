namespace IngressApi.DTO;

public class PlotsRequest
{
    public string PlotId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
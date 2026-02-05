using Sensors.Models;
using System.Threading.Tasks;

namespace IngressApi.Repositories
{
    public interface ISensorDataRepository
    {
        Task SaveAsync(SensorDataPayload data, CancellationToken cancellationToken = default);
    }
}
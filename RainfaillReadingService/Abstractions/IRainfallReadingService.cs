using Common.Models;

namespace RainfallReadingService.Abstractions;

public interface IRainfallReadingService
{
    Task<Result> GetReadingByStationId(string stationId, 
        int limit = 10, 
        CancellationToken cancellationToken = default);
}

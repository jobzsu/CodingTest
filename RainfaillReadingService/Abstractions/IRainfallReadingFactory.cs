using Common.Models;

namespace RainfaillReadingService.Abstractions;

public interface IRainfallReadingFactory
{
    Task<Result> GetReadingByStationId(string stationId, 
        int limit = 10, 
        CancellationToken cancellationToken = default);
}

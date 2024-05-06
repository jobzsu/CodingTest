using Common.Models;
using Common.Models.WebAPI;

namespace RainfallReadingService.Abstractions;

public interface IRainfallReadingService
{
    Task<Result<RainfallReadingResponse>> GetReadingByStationId(string stationId, 
        int limit = 10, 
        CancellationToken cancellationToken = default);
}

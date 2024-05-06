using Common.Models;
using Common.Models.Integrations;
using Common.Models.WebAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RainfallReadingService.Abstractions;
using System.Net;
using System.Net.Http.Headers;

namespace RainfallReadingService;

public class RainfallReadingService : IRainfallReadingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RainfallReadingService> _logger;

    public RainfallReadingService(IHttpClientFactory httpClientFactory,
        ILogger<RainfallReadingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Result<RainfallReadingResponse>> GetReadingByStationId(string stationId, 
        int limit = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await _httpClientFactory
                .CreateClient("RainfallClient")
                .GetAsync($"id/stations/{stationId}/readings?_sorted&_limit={limit}");

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonConvert.DeserializeObject<RainfallReadingApiResponseModel>(content);

                var dtoResponse = new RainfallReadingResponse()
                {
                    Readings = new List<RainfallReading>()
                };

                if(apiResponse is not null && apiResponse.Items.Any())
                {
                    apiResponse.Items.ForEach(item =>
                    {
                        dtoResponse.Readings.Add(new RainfallReading
                        {
                            DateMeasured = item.DateTime,
                            AmountMeasured = item.Value
                        });
                    });

                    return new Result<RainfallReadingResponse>().Success(dtoResponse);
                }
                else
                {
                    return ProcessNotSuccessResponse(StatusCodes.Status404NotFound, "No readings found");
                }
            }
            else
            {
                _logger.LogError($"Error occurred while fetching data from the API. Status code: {response.StatusCode}. Content: {content}");

                switch(response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return ProcessNotSuccessResponse(StatusCodes.Status404NotFound, "No readings found");
                    case HttpStatusCode.BadRequest:
                        return ProcessNotSuccessResponse(StatusCodes.Status400BadRequest, "Bad Request");
                    default:
                        throw new Exception("Internal Server Error");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching data from the API");

            throw;
        }
    }

    private static Result<RainfallReadingResponse> ProcessNotSuccessResponse(int statusCode, string message)
    {
        List<ErrorDetail> errDetails = new();

        return new Result<RainfallReadingResponse>().Error(new ErrorResponse() { Details = errDetails, Message = message }, statusCode);
    }
}

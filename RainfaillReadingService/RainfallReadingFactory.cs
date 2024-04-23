using Common.Models;
using Common.Models.Integrations;
using Common.Models.WebAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RainfaillReadingService.Abstractions;
using System.Net;
using System.Net.Http.Headers;

namespace RainfaillReadingService;

public class RainfallReadingFactory : IRainfallReadingFactory
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RainfallReadingFactory> _logger;

    public RainfallReadingFactory(ILogger<RainfallReadingFactory> logger)
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://environment.data.gov.uk/flood-monitoring/"),
            DefaultRequestHeaders =
            {
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
            }
        };

        _logger = logger;
    }

    public async Task<Result> GetReadingByStationId(string stationId, 
        int limit = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"id/stations/{stationId}/readings?_sorted&_limit={limit}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<RainfallReadingApiResponseModel>(content);

                var dtoResponse = new rainfallReadingResponse()
                {
                    Readings = new List<rainfallReading>()
                };

                if(apiResponse is not null && apiResponse.Items.Any())
                {
                    apiResponse.Items.ForEach(item =>
                    {
                        dtoResponse.Readings.Add(new rainfallReading
                        {
                            DateMeasured = item.DateTime,
                            AmountMeasured = item.Value
                        });
                    });

                    return new Result().Success(dtoResponse);
                }
                else
                {
                    return ProcessNotSuccessResponse(StatusCodes.Status404NotFound, "No readings found");
                }
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogError($"Error occurred while fetching data from the API. Status code: {response.StatusCode}. Content: {content}");

                switch(response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return ProcessNotSuccessResponse(StatusCodes.Status404NotFound, "No readings found");
                    case HttpStatusCode.BadRequest:
                        return ProcessNotSuccessResponse(StatusCodes.Status400BadRequest, "Bad Request");
                    default:
                        return ProcessNotSuccessResponse((int)response.StatusCode, "Internal Server Error");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching data from the API");

            return ProcessNotSuccessResponse(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    private static Result ProcessNotSuccessResponse(int statusCode, string message)
    {
        List<errorDetail> errDetails = new();

        return new Result().Error(new error() { Details = errDetails, Message = message }, statusCode);
    }
}

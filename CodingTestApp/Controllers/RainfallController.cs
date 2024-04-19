using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using RainfaillReadingService.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CodingTestApp.Controllers;

[Route("rainfall")]
[ApiController]
public class RainfallController : ControllerBase
{
    private readonly IRainfallReadingFactory _rainfallReadingFactory;

    public RainfallController(IRainfallReadingFactory rainfallReadingFactory)
    {
        _rainfallReadingFactory = rainfallReadingFactory;
    }

    /// <summary>
    ///  Retrieve the latest readings for the specified stationId
    /// </summary>
    /// <param name="stationId">The id of the reading station</param>
    /// <param name="count">The number of readings to return</param>
    /// <returns>A list of rainfall readings successfully retrieved</returns>
    /// <response code="200">A list of rainfall readings successfully retrieved</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">No readings found for the specified stationId</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [Route("id/{stationId}/readings")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(rainfallReadingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(errorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(errorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(errorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReadingsByStationId(string stationId, [Range(1, 100)]int count = 10)
    {
        var errorDetails = new List<errorDetail>();
        if(string.IsNullOrWhiteSpace(stationId))
        {
            errorDetails.Add(new errorDetail
            {
                PropertyName = "stationId",
                Message = "StationId is required"
            });
        }

        if(count < 1 || count > 100)
        {
            errorDetails.Add(new errorDetail
            {
                PropertyName = "count",
                Message = "Count must be between 1 and 100"
            });
        }

        if(errorDetails.Any())
        {
            return BadRequest(new errorResponse
            {
                Message = "Invalid request",
                Details = errorDetails
            });
        }

        var result = await _rainfallReadingFactory.GetReadingByStationId(stationId, count);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        else
        {
            var errorResponse = new errorResponse()
            {
                Message = ((error)result.Data).Message,
                Details = ((error)result.Data).Details
            };

            switch(result.StatusCode)
            {
                case StatusCodes.Status404NotFound:
                    return NotFound(errorResponse);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(errorResponse);
                case StatusCodes.Status500InternalServerError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }
    }
}

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
    private readonly IRainfaillReadingFactory _rainfallReadingFactory;

    public RainfallController(IRainfaillReadingFactory rainfallReadingFactory)
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
        var result = await _rainfallReadingFactory.GetReadingByStationId(stationId, count);

        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        else
        {
            switch(result.StatusCode)
            {
                case StatusCodes.Status404NotFound:
                    return NotFound(result.Data);
                case StatusCodes.Status400BadRequest:
                    return BadRequest(result.Data);
                case StatusCodes.Status500InternalServerError:
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, result.Data);
            }
        }
    }
}

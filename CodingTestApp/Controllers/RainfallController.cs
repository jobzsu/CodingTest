using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using RainfaillReadingService.Abstractions;

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
    /// <param name="stationId"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RainfallReadingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    [Route("id/{stationId}/readings")]
    public async Task<IActionResult> GetReadingsByStationId(string stationId, int count = 10)
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

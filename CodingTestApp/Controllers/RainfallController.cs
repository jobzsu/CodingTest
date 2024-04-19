using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using RainfaillReadingService.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;

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

    [HttpGet]
    [Route("id/{stationId}/readings")]
    [SwaggerOperation(OperationId = "get-rainfall", 
        Summary = "Get rainfall readings by station Id", 
        Description = "Retrieve the latest readings for the specified stationId",
        Tags = ["Rainfall"])]
    [SwaggerResponse(statusCode: StatusCodes.Status200OK, StatusCode = StatusCodes.Status200OK, 
        ContentTypes = ["application/json"], 
        Description = "A list of rainfall readings successfully retrieved", 
        Type = typeof(rainfallReadingResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest,
        ContentTypes = ["application/json"],
        Description = "Invalid request",
        Type = typeof(errorResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound,
        ContentTypes = ["application/json"],
        Description = "No readings found for the specified stationId",
        Type = typeof(errorResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status500InternalServerError, StatusCode = StatusCodes.Status500InternalServerError,
        ContentTypes = ["application/json"],
        Description = "Internal server error",
        Type = typeof(errorResponse))]
    public async Task<IActionResult> GetReadingsByStationId(
        [SwaggerParameter("The id of the reading station", Required = true)]string stationId,
        [Range(1, 100)]int count = 10)
    {
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

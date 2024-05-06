using CodingTestApp.CustomFilter;
using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using RainfallReadingService.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace CodingTestApp.Controllers;

[Route("rainfall")]
[ApiController]
[SwaggerTag(description: "Operations relating to rainfall")]
public class RainfallController : ControllerBase
{
    private readonly IRainfallReadingService _rainfallReadingFactory;
    private readonly ILogger<RainfallController> _logger;

    public RainfallController(IRainfallReadingService rainfallReadingFactory, ILogger<RainfallController> logger)
    {
        _rainfallReadingFactory = rainfallReadingFactory;
        _logger = logger;
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
        Type = typeof(RainfallReadingResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, StatusCode = StatusCodes.Status400BadRequest,
        ContentTypes = ["application/json"],
        Description = "Invalid request",
        Type = typeof(ErrorResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status404NotFound, StatusCode = StatusCodes.Status404NotFound,
        ContentTypes = ["application/json"],
        Description = "No readings found for the specified stationId",
        Type = typeof(ErrorResponse))]
    [SwaggerResponse(statusCode: StatusCodes.Status500InternalServerError, StatusCode = StatusCodes.Status500InternalServerError,
        ContentTypes = ["application/json"],
        Description = "Internal server error",
        Type = typeof(ErrorResponse))]
    public async Task<IActionResult> GetReadingsByStationId(
        [SwaggerParameter("The id of the reading station", Required = true)]string stationId,
        [Range(1, 100)]int count = 10)
    {
        var result = await _rainfallReadingFactory.GetReadingByStationId(stationId, count);

        try
        {
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                switch (result.StatusCode)
                {
                    case StatusCodes.Status404NotFound:
                        return NotFound(result.ErrorResponse);
                    case StatusCodes.Status400BadRequest:
                        return BadRequest(result.ErrorResponse);
                    default:
                        throw new Exception("Internal Server Error");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return StatusCode(StatusCodes.Status500InternalServerError, 
                new ErrorResponse() { Details = new List<ErrorDetail>(), Message = "Internal Server Error" });
        }
    }
}

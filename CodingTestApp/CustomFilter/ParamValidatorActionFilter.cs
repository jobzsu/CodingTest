using Common.Models.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CodingTestApp.CustomFilter;

public class ParamValidatorActionFilter : IActionFilter
{
    // Initialize error details
    List<ErrorDetail> errs = new();

    public void OnActionExecuted(ActionExecutedContext context)
    {
        errs.Clear();

        // Validate stationId parameter
        object? stationId;
        if (context.HttpContext.Request.RouteValues.TryGetValue("stationId", out stationId))
        {
            if (stationId == null || string.IsNullOrWhiteSpace(stationId.ToString()))
            {
                errs.Add(new ErrorDetail
                {
                    PropertyName = "stationId",
                    Message = "StationId is required"
                });
            }
            else
            {
                var stationIdStr = stationId.ToString();

                if (Regex.Matches(stationIdStr, @"[^0-9]").Count > 0)
                {
                    errs.Add(new ErrorDetail
                    {
                        PropertyName = "stationId",
                        Message = "Invalid stationId"
                    });
                }
            }
        }

        // Validate count parameter
        StringValues countValues = new StringValues();
        if (context.HttpContext.Request.Query.TryGetValue("count", out countValues))
        {
            int count;
            if (int.TryParse(countValues.First(), out count))
            {
                if (count < 1 || count > 100)
                {
                    errs.Add(new ErrorDetail
                    {
                        PropertyName = "count",
                        Message = "Count must be between 1 and 100"
                    });
                }
            }
            else
            {
                errs.Add(new ErrorDetail
                {
                    PropertyName = "count",
                    Message = "Invalid count parameter"
                });
            }
        }

        if (errs.Any())
        {
            var err = new ErrorResponse()
            {
                Message = "Internal Server Error",
                Details = errs
            };

            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var serializedResponse = JsonSerializer.Serialize(err, jsonSerializerOptions);


            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.HttpContext.Response.ContentType = "application/json";

            context.Result = new BadRequestObjectResult(err);
            return;
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        
    }
}


using Common.Models.WebAPI;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CodingTestApp.CustomMiddleware;

public class GlobalErrorHandlerMiddleware : IMiddleware
{
	private readonly ILogger<GlobalErrorHandlerMiddleware> _logger;

    public GlobalErrorHandlerMiddleware(ILogger<GlobalErrorHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		if((!context.Request.Path.Value?.StartsWith("/rainfall/id/") ?? true))
		{ 
			await next(context);
			return;
		}

		// Initialize error details
		List<ErrorDetail> errs = new();
		int statusCode = StatusCodes.Status500InternalServerError;

		try
		{
			// Validate stationId parameter
			object? stationId;
			if(context.Request.RouteValues.TryGetValue("stationId", out stationId))
			{
				if(stationId == null || string.IsNullOrWhiteSpace(stationId.ToString()))
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

					if(Regex.Matches(stationIdStr, @"[^0-9]").Count > 0)
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
			if(context.Request.Query.TryGetValue("count", out countValues))
			{
				int count;
				if(int.TryParse(countValues.First(), out count))
				{
					if(count < 1 || count > 100)
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

			if(errs.Any())
			{
				statusCode = StatusCodes.Status400BadRequest;
                throw new Exception("Bad Request");
            }

			await next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);

			var err = new ErrorResponse()
			{
				Message = statusCode == StatusCodes.Status400BadRequest ? "Bad Request" : "Internal Server Error",
				Details = errs
			};

            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            var serializedResponse = JsonSerializer.Serialize(err, jsonSerializerOptions);

            context.Response.StatusCode = statusCode != StatusCodes.Status500InternalServerError ?
				statusCode : StatusCodes.Status500InternalServerError;
			context.Response.ContentType = "application/json";

			await context.Response.WriteAsync(serializedResponse);
		}
    }
}

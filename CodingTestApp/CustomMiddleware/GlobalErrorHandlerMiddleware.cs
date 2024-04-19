
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

		List<errorDetail> errs = new();
		int statusCode = StatusCodes.Status500InternalServerError;

		try
		{
			object? stationId;
			if(context.Request.RouteValues.TryGetValue("stationId", out stationId))
			{
				if(stationId == null || string.IsNullOrWhiteSpace(stationId.ToString()))
				{
                    errs.Add(new errorDetail
					{
                        PropertyName = "stationId",
                        Message = "StationId is required"
                    });

                    statusCode = StatusCodes.Status400BadRequest;
                }
				else
				{
					var stationIdStr = stationId.ToString();

					if(Regex.Matches(stationIdStr, @"[^0-9]").Count > 0)
					{
                        errs.Add(new errorDetail
						{
                            PropertyName = "stationId",
                            Message = "Invalid stationId"
                        });

                        statusCode = StatusCodes.Status400BadRequest;
					}
				}
			}

            StringValues countValues = new StringValues();
			if(context.Request.Query.TryGetValue("count", out countValues))
			{
				int count;
				if(int.TryParse(countValues.First(), out count))
				{
					if(count < 1 || count > 100)
					{
						errs.Add(new errorDetail
						{
                            PropertyName = "count",
                            Message = "Count must be between 1 and 100"
                        });

						statusCode = StatusCodes.Status400BadRequest;
					}
				}
				else
				{
                    errs.Add(new errorDetail
                    {
                        PropertyName = "count",
                        Message = "Invalid count parameter"
                    });

                    statusCode = StatusCodes.Status400BadRequest;
                }
			}

			if(errs.Any())
			{
                throw new Exception("Bad Request");
            }

			await next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);

			var err = new error()
			{
				Message = ex.Message,
				Details = errs
			};

            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                // Enable pascal casing
                PropertyNamingPolicy = null,
                // Ignores properties that are null
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

﻿
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
		List<errorDetail> errs = new();
		int statusCode = StatusCodes.Status500InternalServerError;

		try
		{
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

						throw new Exception("Bad Request");
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

                    throw new Exception("Bad Request");
                }
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

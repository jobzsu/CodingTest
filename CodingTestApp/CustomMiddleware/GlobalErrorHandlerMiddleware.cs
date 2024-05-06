
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
		// Initialize error details
		List<ErrorDetail> errs = new();

		try
		{
			await next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);

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

			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			context.Response.ContentType = "application/json";

			await context.Response.WriteAsync(serializedResponse);
		}
    }
}

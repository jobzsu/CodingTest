using Common.Models.WebAPI;
using Microsoft.AspNetCore.Http;

namespace Common.Models;

public class Result
{
    public bool IsSuccess { get; set; }

    public int StatusCode { get; set; }

    public RainfallReadingResponse? RainfallReadingResponse { get; set; }

    public ErrorResponse? ErrorResponse { get; set; }

    public Result Success(RainfallReadingResponse data)
    {
        IsSuccess = true;
        StatusCode = StatusCodes.Status200OK;
        RainfallReadingResponse = data;
        ErrorResponse = null;

        return this;
    }

    public Result Error(ErrorResponse error, int status)
    {
        IsSuccess = false;
        StatusCode = status;
        ErrorResponse = error;
        RainfallReadingResponse = null;

        return this;
    }
}

using Common.Models.WebAPI;
using Microsoft.AspNetCore.Http;

namespace Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; set; }

    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public ErrorResponse? ErrorResponse { get; set; }

    public Result<T> Success(T data)
    {
        IsSuccess = true;
        StatusCode = StatusCodes.Status200OK;
        Data = data;

        return this;
    }

    public Result<T> Error(ErrorResponse error, int status)
    {
        IsSuccess = false;
        StatusCode = status;
        ErrorResponse = error;

        return this;
    }
}

using Microsoft.AspNetCore.Http;

namespace Common.Models;

public class Result
{
    public bool IsSuccess { get; set; }

    public int StatusCode { get; set; }

    public object Data { get; set; }

    public Result Success(object data)
    {
        IsSuccess = true;
        StatusCode = StatusCodes.Status200OK;
        Data = data;

        return this;
    }

    public Result Error(object data, int status)
    {
        IsSuccess = false;
        StatusCode = status;
        Data = data;

        return this;
    }
}

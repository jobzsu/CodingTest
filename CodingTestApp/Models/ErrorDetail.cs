using Newtonsoft.Json;

namespace CodingTestApp.Models;

public class ErrorDetail
{
    [JsonProperty("propertyName")]
    public string PropertyName { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}

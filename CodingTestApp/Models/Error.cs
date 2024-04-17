using Newtonsoft.Json;

namespace CodingTestApp.Models;

public class Error
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("details")]
    public List<ErrorDetail> Details { get; set; }
}

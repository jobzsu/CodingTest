using Newtonsoft.Json;

namespace CodingTestApp.Models;

public class RainfallReadingResponse
{
    [JsonProperty("readings")]
    public List<RainfallReading> Readings { get; set; }
}

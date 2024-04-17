using Newtonsoft.Json;

namespace CodingTestApp.Models;

public class RainfallReading
{
    [JsonProperty("dateMeasured")]
    public DateTime DateMeasured { get; set; }

    [JsonProperty("amountMeasured")]
    public double AmountMeasured { get; set; }
}

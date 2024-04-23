using Newtonsoft.Json;

namespace Core.Models;

public class RainfallAPIReadingResponseModel
{
    [JsonProperty("@context")]
    public string Context { get; set; }

    [JsonProperty("meta")]
    public Meta Meta { get; set; }

    [JsonProperty("items")]
    public List<Item> Items { get; set; }
}

public class Item
{
    [JsonProperty("@id")]
    public string Id { get; set; }

    [JsonProperty("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonProperty("measure")]
    public string Measure { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }
}

public class Meta
{
    [JsonProperty("publisher")]
    public string Publisher { get; set; }

    [JsonProperty("licence")]
    public string Licence { get; set; }

    [JsonProperty("documentation")]
    public string Documentation { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("hasFormat")]
    public List<string> HasFormat { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }
}


using Newtonsoft.Json;

namespace ExchangeService.BusinessLogic.Context;
public class Response
{
    [JsonProperty("base", NullValueHandling = NullValueHandling.Ignore)]
    public string? Base { get; set; }
    [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
    public string? Date { get; set; }
    [JsonProperty("start_date", NullValueHandling = NullValueHandling.Ignore)]
    public string? StartDate { get; set; }
    [JsonProperty("end_date", NullValueHandling = NullValueHandling.Ignore)]
    public string? EndDate { get; set; }

    [JsonProperty("rates", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Rates { get; set; }
    [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Success { get; set; } = true;
    [JsonProperty("fluctuation", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Fluctuation { get; set; }
    [JsonProperty("historical", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Historical { get; set; }
    [JsonProperty("info", NullValueHandling = NullValueHandling.Ignore)]
    public object? Info { get; set; }
    [JsonProperty("query", NullValueHandling = NullValueHandling.Ignore)]
    public object? Query { get; set; }
    [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
    public string? TimeStamp { get; set; }
    [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
    public string? Result { get; set; }
    [JsonProperty("symbols", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, string>? Symbols { get; set; }
    [JsonProperty("timeseries", NullValueHandling = NullValueHandling.Ignore)]
    public bool? TimeSeries { get; set; }
}

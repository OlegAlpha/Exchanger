using ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
using ExchangeService.BusinessLogic.Context;
using Newtonsoft.Json;
using RestSharp;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess;
public class ApiService : IApiService
{
    private const string ApiConfigurationKey = "API_KEY";
    private const string ApiUrlKey = "API_URL";
    private const string ApiKeyHeader = "apikey";
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public ApiService(IConfiguration configuration)
    {
        _apiKey = configuration[ApiConfigurationKey];
        _apiUrl = configuration[ApiUrlKey];
    }

    public  async Task<string> PostAllRatesInRangeFromServer(DateTime endDate, DateTime startDate, string? @base, string? symbols)
    {
        var urlBuilder = new StringBuilder(_apiUrl)
            .Append($"/timeseries?end_date={endDate.ToString("MM/dd/yyyy")}")
            .Append($"&start_date={startDate.ToString("MM/dd/yyyy")}")
            .AppendIf($"&base={@base}", @base is not null)
            .AppendIf($"&symbols={symbols}", symbols is not null);

        var client = new RestClient(urlBuilder.ToString());

        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);

        var response = await client.ExecuteAsync(request);
        return response.Content;
    }
    public  async Task<Response> PostLatestRatesWithUncachedData(string newSymbols, string? @base, string? symbols)
    {
        Response responseBody;

        if (string.IsNullOrEmpty(newSymbols))
        {
            responseBody = new Response()
            {
                Base = @base,
                Date = DateTime.UtcNow.ToString("MM/dd/yyyy"),
                Success = true,
                TimeStamp = DateTime.UtcNow.Millisecond.ToString()
            };
        }
        else
        {
            responseBody = await GetLatestUncachedRates(@base, newSymbols);
        }

        return responseBody;
    }
    private  async Task<Response> GetLatestUncachedRates(string? @base, string newSymbols)
    {
        var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
            .AppendIf($"symbols={newSymbols}&", !String.IsNullOrWhiteSpace(newSymbols))
            .AppendIf($"base={@base}", @base is not null);

        var client = new RestClient(urlBuilder.ToString());
        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteAsync(request);

        return JsonConvert.DeserializeObject<Response>(response.Content);
    }
    public async Task<string> PostAvailableCurrencies()
    {
        var client = new RestClient($"{_apiUrl}/symbols");
        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteAsync(request);
        return response.Content;
    }
    public  async Task<Response> PostRequestToExchange(int userId, decimal amount, string from, string to)
    {
        string url = new StringBuilder($"{_apiUrl}/convert?")
                .Append($"to={to}&")
                .Append($"from={from}&")
                .Append($"amount={amount}")
                .ToString();
        var client = new RestClient(url);
        var request = new RestRequest();
        request.Method = Method.Get;
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteGetAsync(request);

        var responseBody = JsonConvert.DeserializeObject<Response>(response.Content);
        var rate = decimal.Parse(responseBody.Info.GetPropertyValue<string>("Rate"));
        

        return responseBody;
    }
    public async Task<Response> PostUncachedFluctuation(DateTime start, DateTime end, string? baseCurrency, IEnumerable<string>? currencies)
    {
        string currenciesRequest = string.Join(',', currencies ?? Enumerable.Empty<string>());

        var urlBuilder = new StringBuilder($"{_apiUrl}/latest?")
            .Append($"start_date={start.ToString("yyyy-MM-dd")}&")
            .Append($"end_date={end.ToString("yyyy-MM-dd")}&")
            .AppendIf($"base={baseCurrency}&", !string.IsNullOrWhiteSpace(baseCurrency))
            .AppendIf($"symbols={currenciesRequest}", !string.IsNullOrWhiteSpace(currenciesRequest));

        var client = new RestClient(urlBuilder.ToString());
        var request = new RestRequest();
        request.AddHeader(ApiKeyHeader, _apiKey);
        var response = await client.ExecuteAsync(request);

        return JsonConvert.DeserializeObject<Response>(response.Content);
    }
}

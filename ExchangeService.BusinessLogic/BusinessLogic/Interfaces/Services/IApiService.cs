﻿using ExchangeService.BusinessLogic.Context;

namespace ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
public interface IApiService
{

    public Task<string> GetAllRatesInRangeFromServer(DateTime endDate, DateTime startDate, string? @base, string? symbols);
    public Task<Response> GetLatestRatesWithUncachedData(string newSymbols, string? @base, string? symbols);
    public Task<string> GetAvailableCurrencies();
    public Task<Response> RequestToExchange(int userId, decimal amount, string from, string to);
    public Task<Response> GetUncachedFluctuation(DateTime start, DateTime end, string? baseCurrency, IEnumerable<string>? currencies);

}

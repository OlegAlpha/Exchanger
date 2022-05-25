﻿using ExchangeService.BusinessLogic.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
public interface IApiService
{

    public Task<string> PostAllRatesInRangeFromServer(DateTime endDate, DateTime startDate, string? @base, string? symbols);
    public Task<Response> PostLatestRatesWithUncachedData(string newSymbols, string? @base, string? symbols);
    public Task<string> PostAvailableCurrencies();
    public Task<Response> PostRequestToExchange(int userId, decimal amount, string from, string to);
    public Task<Response> PostUncachedFluctuation(DateTime start, DateTime end, string? baseCurrency, IEnumerable<string>? currencies);

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeService.BusinessLogic.BusinessLogic.Interfaces.Services;
public interface IRedirectRequests
{
    public Task<string> ExchageProcess(int userId, decimal amount, string from, string to);
    public Task<string> LatestRatesProcess(string? @base, string? symbols);
    public Task<string> RatesWithinProcess(DateTime endDate, DateTime startDate, string? @base, string? symbols);
    public Task<string> FluctuationProcessing(DateTime start, DateTime end, string baseCurrency, params string[] currencies);
    Task<string?> GetAvailableCurrencies();
}

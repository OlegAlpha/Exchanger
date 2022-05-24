﻿using System.Collections.Concurrent;
using System.Text;
using ExchangeService.BusinessLogic.Context;
using ExchangeService.DataAccessLayer.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ExchangeService;
using RestSharp;

namespace ExchangeService.BusinessLogic.BusinessLogic.RequestProcess
{
    public class CacheService : ICacheService
    {
      
        private readonly IHistoryService _storyService;


        private const string RateLifetimeKey = "RateLifetimeInCache";
        private readonly int _rateLifetimeInCache;
        private static readonly ConcurrentDictionary<ExchangeRate, DateTime> s_cachedRates = new();

        public CacheService(IConfiguration configuration, IHistoryService storyService)
        {
            _rateLifetimeInCache = Int32.Parse(configuration[RateLifetimeKey]);
            _storyService = storyService;
        }

        public bool IsCreatedExchangeRate(string from, string to, DateTime? date = null)
        {
            ExchangeRate? rate;

            if (date is null)
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);
            }
            else
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to && r.Date.HasValue && (r.Date - date).Value.Days == 0);
            }

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return false;
            }

            return true;
        }

        public ExchangeRate? GetExchangeRateOrDefault(string from, string to, DateTime? date = null)
        {
            ExchangeRate? rate;

            if (date is null)
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to);
            }
            else
            {
                rate = s_cachedRates.Keys.FirstOrDefault(r => r.From == from && r.To == to && r.Date.HasValue && (r.Date - date).Value.Days == 0);
            }

            if (rate is null || (DateTime.UtcNow - s_cachedRates[rate]).Milliseconds >= _rateLifetimeInCache)
            {
                return null;
            }

            return rate;
        }

        public void SetExchangeRate(string from, string to, decimal rate, DateTime? date = null)
        {
            var exchangeRate = new ExchangeRate()
            {
                From = from,
                To = to,
                Date = date ?? DateTime.UtcNow,
                Rate = (double)rate
            };

            s_cachedRates[exchangeRate] = DateTime.UtcNow;
        }
        public Response GetExchangeFromCache(int userId, decimal amount, string from, string to)
        {
            var responseBody = new Response();
            ExchangeRate exchangeRate = GetExchangeRateOrDefault(from, to);
            _storyService.StoreExchange(userId, exchangeRate);
            responseBody.Result = (exchangeRate.Rate * (double)amount).ToString();
            responseBody.Query = new
            {
                amount,
                from,
                to
            };
            responseBody.Success = true;
            responseBody.Date = exchangeRate.Date?.ToString("MM/dd/yyyy");
            return responseBody;
        }
        public bool IsCachedRatesWithin(DateTime startDate, DateTime endDate, string[] currencies, string @base)
        {
            for (var i = startDate; i < endDate; i = i.AddDays(1))
            {
                foreach (var currency in currencies)
                {
                    if (!IsCreatedExchangeRate(@base, currency, i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public async Task<Response> GetAllRatesInRangeFromCache(DateTime endDate, DateTime startDate, string? @base, string[] currencies)
        {
            Response responseBody = new Response();

            for (var i = startDate; i < endDate; i = i.AddDays(1))
            {
                string ratesJsonObject = "{";

                foreach (var currency in currencies)
                {
                    ExchangeRate rate = GetExchangeRateOrDefault(@base, currency, i);
                    string rateJson = string.Format("\"{0}\":{1},", currency, rate.Rate.ToString());
                    ratesJsonObject = string.Concat(ratesJsonObject, rate);
                }

                ratesJsonObject = ratesJsonObject.Substring(0, ratesJsonObject.Length - 1);
                ratesJsonObject = string.Concat(ratesJsonObject, "},");
                responseBody.Rates[i.ToString("yyyy-MM-dd")] = ratesJsonObject;
            }

            responseBody.Rates[endDate.ToString("yyyy-MM-dd")] =
            responseBody.Rates[endDate.ToString("yyyy-MM-dd")]
            .Substring(0, responseBody.Rates[endDate.ToString("yyyy-MM-dd")].Length - 1);

            return responseBody;
        }
        public async Task<Response> GetCachedFluctuation(string baseCurrency, DateTime start, DateTime end, List<string> cachedCurrencies, Response responseBody)
        {
           
            foreach (var currency in cachedCurrencies)
            {
                ExchangeRate startRate = GetExchangeRateOrDefault(baseCurrency, currency, start);
                ExchangeRate endRate = GetExchangeRateOrDefault(baseCurrency, currency, end);

                string jsonObject = string.Format(
                    "{ \"change\":\"{0}\" \r\n \"change_pct\":\"{1}\" \r\n \"end_rate\":\"{2}\" \r\n \"start_rate\":\"{3}\" \r\n}",
                    startRate.Rate / endRate.Rate, startRate.Rate / endRate.Rate, start, end);

                responseBody.Rates.Add(currency, jsonObject);
            }

            return responseBody;
        }

        
    }
}
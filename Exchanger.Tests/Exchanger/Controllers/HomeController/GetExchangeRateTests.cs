using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exchanger.Tests.ExchangerTests.Controllers.HomeController;
public class GetExchangeRateTests
{

    private Exchanger.Controllers.HomeController controller = new();

    [Fact]
    public void GetActualData()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.success.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.False(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
    }
    [Fact]
    public void GetActualDatas()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", "EUR"};

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.success.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.False(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
        Assert.NotNull(result.rates.EUR);
    }
    [Fact]
    public void GetFailedActualDatas()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea", "EUR" };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetFailedActualData()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea" };

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetHistoricData()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", };
        DateTime dateTime = DateTime.Today.AddDays(-1); 

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
    }
    [Fact]
    public void GetHistoricDatas()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "UAH", "EUR" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.Equal("USD", result.info.@base.ToString());
        Assert.True(bool.Parse(result.info.historical.ToString()));
        Assert.NotNull(result.rates.UAH);
        Assert.NotNull(result.rates.EUR);
    }
    [Fact]
    public void GetFailedHistoricDatas()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea", "EUR" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
    [Fact]
    public void GetFailedHistoricData()
    {
        string baseCurrency = "USD";
        string[] resultCurrency = new[] { "dea" };
        DateTime dateTime = DateTime.Today.AddDays(-1);

        string resultJSON = controller.GetExchangeRate(baseCurrency, resultCurrency, dateTime);

        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.False(bool.Parse(result.info.historical.ToString()));
    }
}

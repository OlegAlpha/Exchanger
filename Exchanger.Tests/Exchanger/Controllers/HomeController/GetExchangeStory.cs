using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exchanger.Tests.ExchangerTests.Controllers.HomeController;
public class GetExchangeStory
{
    private Exchanger.Controllers.HomeController controller = new();

    public void GetStory()
    {
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.GetExchangeStory(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.timeseries.ToString()));
        Assert.NotEqual("", result.rates.ToString());
    }

    public void GetFailedStory()
    {
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.GetExchangeStory(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.timeseries.ToString()));
        Assert.Equal("", result.rates.ToString());
    }
}

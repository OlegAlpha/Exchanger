using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exchanger.Tests.ExchangerTests.Controllers.HomeController;
public class Fluctuation
{
    private Exchanger.Controllers.HomeController controller = new();

    public void GetFluctuation()
    {
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "UAH";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.Fluctuation(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.fluctuation.ToString()));
        Assert.NotEqual("", result.rates.ToString());
    }

    public void GetIncorrectFluctuation()
    {
        DateTime start = DateTime.Today.AddDays(-8);
        DateTime end = DateTime.Today.AddDays(-1);
        string baseCurrency = "fse";
        string[] currencies = new[] { "EUR" };

        string resultJSON = controller.Fluctuation(start, end, baseCurrency, currencies);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.True(bool.Parse(result.fluctuation.ToString()));
        Assert.Equal("", result.rates.ToString());
    }
}

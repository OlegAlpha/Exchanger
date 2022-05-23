using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Exchanger.Tests.ExchangerTests.Controllers.HomeController;
public class ExchangeTests
{
    private Exchanger.Controllers.HomeController controller = new();

    [Fact]
    public void Exchange()
    {
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "EUR";

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.True(bool.Parse(result.success.ToString()));
        Assert.NotEqual("", result.result.ToString());
    }
    [Fact]
    public void FailExchange()
    {
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "cdsv";

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.Equal("", result.result.ToString());
    }
    [Fact]
    public void SpamExchange()
    {
        const int spamCount = 10;
        int userId = 1;
        decimal amount = 100;
        string from = "UAH";
        string to = "EUR";

        for (int i = 0; i < spamCount; ++i){
            controller.Exchange(userId, amount, from, to);
        }

        string resultJSON = controller.Exchange(userId, amount, from, to);
        dynamic result = JsonConvert.DeserializeObject<dynamic>(resultJSON);

        Assert.NotNull(result);
        Assert.False(bool.Parse(result.success.ToString()));
        Assert.Equal("",result.result.ToString());
    }
}

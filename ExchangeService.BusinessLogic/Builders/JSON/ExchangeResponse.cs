using ExchangeService.BusinessLogic.Builders.JSON.Components;

namespace ExchangeService.BusinessLogic.Builders.JSON;
public class ExchangeResponse: JSONBuilder
{
    public ExchangeResponse(DateTime dateTime, InfoComponent info,bool isSuccess , object? result = null)
    {
        AddComponent(info);
        AddComponent("date", dateTime.ToString());
        AddComponent("Success", isSuccess.ToString());

        if (result != null)
        {
            AddComponent("result", result.ToString());
        }
    }
}

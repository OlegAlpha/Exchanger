using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.Builders.JSON.Extends;

namespace ExchangeService.BusinessLogic.Builders.JSON.Components;

public class InfoComponent :  JSONBaseComponent
{
    public InfoComponent(long time, bool? isHistorical = null): base("info")
    {
        InitializeData(time, isHistorical);
    }

    private void InitializeData(long time, bool? isHistorical)
    {
        string requiredData = "";
        requiredData = requiredData.AddJSONComponent("timestamp", time.ToString());

        if (isHistorical != null)
        {
            requiredData = requiredData.AddJSONComponent("historical", isHistorical.Value.ToString());
        }

        Data = new StringBuilder(requiredData);
    }
}

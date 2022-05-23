using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.Builders.JSON.Extends;

namespace ExchangeService.BusinessLogic.Builders.JSON.Components;
public class QueryComponent : JSONBaseComponent
{
    public QueryComponent(string from, string to) : base("query")
    {
        InitializeData(from, to);
    }

    private void InitializeData(string from, string to)
    {
        string requiredData = "";

        requiredData = requiredData.AddJSONComponent("from", from);
        requiredData = requiredData.AddJSONComponent("to", to);
        Data = new StringBuilder(requiredData);
    }
}

using IntermediateLayer.Builders.JSON.Components.BaseInterface;
using IntermediateLayer.Builders.JSON.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON.Components;
public class QueryComponent : JSONBaseComponent
{
    public QueryComponent(string from, string to) : base("query")
    {
        InitializeData(from, to);
    }

    private void InitializeData(string from, string to)
    {
        string reguiredData = "";

        reguiredData = reguiredData.AddJSONComponent("from", from);
        reguiredData = reguiredData.AddJSONComponent("to", to);
        Data = new StringBuilder(reguiredData);
    }
}

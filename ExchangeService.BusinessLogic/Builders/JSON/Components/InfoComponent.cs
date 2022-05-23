using IntermediateLayer.Builders.JSON.Components.BaseInterface;
using IntermediateLayer.Builders.JSON.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON.Components;

public class InfoComponent :  JSONBaseComponent
{
    public InfoComponent(long time, bool? isHistorical = null): base("info")
    {
        InitializeData(time, isHistorical);
    }

    private void InitializeData(long time, bool? isHistorical)
    {
        string reguiredData = "";
        reguiredData = reguiredData.AddJSONComponent("timestamp", time.ToString());

        if (isHistorical != null)
        {
            reguiredData = reguiredData.AddJSONComponent("historical", isHistorical.Value.ToString());
        }

        Data = new StringBuilder(reguiredData);
    }
}

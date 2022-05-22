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
    public InfoComponent(long time, bool? isHisorical = null): base("info")
    {
        InitializeData(time, isHisorical);
    }

    private void InitializeData(long time, bool? isHisorical)
    {
        string reguiredData = "";
        reguiredData = reguiredData.AddJSONComponent("timestamp", time.ToString());

        if (isHisorical != null)
        {
            reguiredData = reguiredData.AddJSONComponent("historical", isHisorical.Value.ToString());
        }

        Data = new StringBuilder(reguiredData);
    }
}

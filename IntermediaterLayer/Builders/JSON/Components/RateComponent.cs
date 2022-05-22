using IntermediateLayer.Builders.JSON.Components.BaseInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON.Components;
public class RateComponent: JSONBaseComponent
{

    public RateComponent(string currency,decimal startRate,decimal endRate):base(currency)
    {
        Initialize(startRate,endRate);
    }

    private void Initialize(decimal startRate, decimal endRate)
    {
        decimal change = decimal.Round(startRate / endRate, 4);

        AddComponent("change", change.ToString());
        AddComponent("end_rate", endRate.ToString());
        AddComponent("start_rate", startRate.ToString());
    }
}

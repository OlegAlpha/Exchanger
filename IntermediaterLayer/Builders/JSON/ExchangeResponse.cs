using IntermediateLayer.Builders.JSON.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON;
public class ExchangeResponse: JSONBuilder
{
    public ExchangeResponse(DateTime dateTime, InfoComponent info,bool isSuccess , object? result = null)
    {
        AddComponent(info);
        AddComponent("date", dateTime.ToString());
        AddComponent("success", isSuccess.ToString());

        if (result != null)
        {
            AddComponent("result", result.ToString());
        }
    }
}

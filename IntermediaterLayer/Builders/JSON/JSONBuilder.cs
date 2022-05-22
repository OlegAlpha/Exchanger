using IntermediateLayer.Builders.JSON.Components.BaseInterface;
using IntermediateLayer.Builders.JSON.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON;
public class JSONBuilder
{
    protected List<JSONBaseComponent> components = new List<JSONBaseComponent>();
    protected string builtComponents = "";

   

    public virtual StringBuilder Build()
    {
        string result = builtComponents;

        foreach (var component in components)
        {
            result = result.AddJSONComponent(component);
        }

        return new StringBuilder(result);
    }


    public virtual bool AddComponent(JSONBaseComponent component)
    {
        if (component is null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        components.Add(component);

        return true;
    }
    public virtual bool AddComponent(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        builtComponents = builtComponents.AddJSONComponent(name, value);

        return true;
    }
}

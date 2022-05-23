using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
using ExchangeService.BusinessLogic.Builders.JSON.Extends;

namespace ExchangeService.BusinessLogic.Builders.JSON;
public class JSONBuilder
{
    protected List<JSONBaseComponent> _components = new List<JSONBaseComponent>();
    protected string _builtComponents = "";

   

    public virtual StringBuilder Build()
    {
        string result = _builtComponents;

        foreach (var component in _components)
        {
            result = result.AddJSONComponent(component);
        }

        result = result.Substring(0, result.Length - 1);

        return new StringBuilder(string.Concat('{',result,'}'));
    }


    public virtual bool AddComponent(JSONBaseComponent component)
    {
        if (component is null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        _components.Add(component);

        return true;
    }
    public virtual bool AddComponent(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        _builtComponents = _builtComponents.AddJSONComponent(name, value);

        return true;
    }
}

using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Extends;

namespace ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;
public class JSONBaseComponent
{
    public string Name { get; }
    public StringBuilder Data { get; protected set; }

    public JSONBaseComponent(string name)
    {
        Name = name;
        Data = new StringBuilder();
    }

    /// <summary>
    /// building at once 
    /// you need add add component with all elements
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool AddComponent(JSONBaseComponent component)
    {
        if (component is null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        string result = "";
        Data.AppendLine(result.AddJSONComponent(component));

        return true;
    }
    public bool AddComponent(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        string result = "";
        Data.AppendLine(result.AddJSONComponent(name, value));

        return true;
    }
}

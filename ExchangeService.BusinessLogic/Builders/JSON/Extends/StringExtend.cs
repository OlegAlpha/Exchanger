using System.Text;
using ExchangeService.BusinessLogic.Builders.JSON.Components.BaseComponent;

namespace ExchangeService.BusinessLogic.Builders.JSON.Extends;
public static class StringExtend
{
    private static string WrapComponent(string str)
    {
        return string.Format("\"{0}\"", str);
    }

    private static string WrapComponentsGroup(string str)
    {
        return string.Concat("{\r\n", str, "\r\n}");
    }

    public static string AddJSONComponent(this string source, JSONBaseComponent component)
    {
        if (component is null)
        {
            throw new ArgumentNullException(nameof(component));
        }

        string right = "";
        if (String.IsNullOrWhiteSpace(component.Data.ToString()) == false) 
        {
             right = component.Data.Remove(component.Data.Length - 2, 1).ToString();
        }
        StringBuilder result = new StringBuilder(source ?? "");
        string left = WrapComponent(component.Name);
        
        right = WrapComponentsGroup(right);

        result = result.AppendJSONElement(left, right);

        return result.ToString();
    }
    public static string AddJSONComponent(this string source, string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        StringBuilder result = new StringBuilder(source ?? "");
        string left = WrapComponent(name);
        string right = WrapComponent(value);

        result = result.AppendJSONElement(left, right);

        return result.ToString();
    }
}

using System.Text;

namespace ExchangeService.BusinessLogic
{
    internal static class Extensions
    {
        public static StringBuilder AppendIf(this StringBuilder sb, string? s, bool condition)
        {
            if (condition)
            {
                sb.Append(s);
            }

            return sb;
        }

        public static T GetPropertyValue<T>(this object o, string name)
        {
            object? p = o.GetType().GetProperty(name)?.GetValue(o);
            if (p is null or not T)
            {
                throw new InvalidOperationException();
            }

            return (T)p;
        }
    }
}

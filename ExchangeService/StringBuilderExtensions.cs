using System.Text;

namespace ExchangeService
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIf(this StringBuilder sb, string? s, bool condition)
        {
            if (condition)
            {
                sb.Append(s);
            }

            return sb;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntermediateLayer.Builders.JSON;
internal static class StringBuilderExtend
{
    public static StringBuilder AppendJSONElement(this StringBuilder source, string left, string right)
    {
        const char colon = ':';
        const char coma = ',';

        source = source.Append(left);
        source = source.Append(colon);
        source = source.Append(right);
        source = source.Append(coma);
        source = source.AppendLine();

        return source;    
    }

}

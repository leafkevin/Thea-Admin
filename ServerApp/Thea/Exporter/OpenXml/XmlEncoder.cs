using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Thea.ExcelExporter;

class XmlEncoder
{
    private static readonly Regex xHHHHRegex = new Regex("_(x[\\dA-Fa-f]{4})_", RegexOptions.Compiled);
    private static readonly Regex Uppercase_X_HHHHRegex = new Regex("_(X[\\dA-Fa-f]{4})_", RegexOptions.Compiled);

    public static string Encode(string encodeStr)
    {
        if (encodeStr == null) return null;
        encodeStr = xHHHHRegex.Replace(encodeStr, "_x005F_$1_");
        var builder = new StringBuilder(encodeStr.Length);
        foreach (var ch in encodeStr)
        {
            if (XmlConvert.IsXmlChar(ch))
                builder.Append(ch);
            else builder.Append(XmlConvert.EncodeName(ch.ToString()));
        }
        var result = builder.ToString();
        result = result.Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
        return result;
    }
    public static string Decode(string decodeStr)
    {
        if (string.IsNullOrEmpty(decodeStr)) return string.Empty;
        // https://github.com/ClosedXML/ClosedXML/issues/1154
        decodeStr = Uppercase_X_HHHHRegex.Replace(decodeStr, "_x005F_$1_");
        return XmlConvert.DecodeName(decodeStr);
    }
}

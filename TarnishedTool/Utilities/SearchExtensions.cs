using System.Globalization;
using System.Text;

namespace TarnishedTool.Utilities;

public static class SearchExtensions
{
    private static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;
    private const CompareOptions Options =
        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols;

    public static bool ContainsFuzzy(this string source, string search)
    {
        if (string.IsNullOrEmpty(search)) return true;
        if (string.IsNullOrEmpty(source)) return false;
        return CompareInfo.IndexOf(source, search, Options) >= 0;
    }
    
    public static string NormalizeForAlias(this string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
            }
            else if (char.IsWhiteSpace(c) && sb.Length > 0 && sb[sb.Length - 1] != ' ')
            {
                sb.Append(' ');
            }
        }
        return sb.ToString().Trim();
    }
}
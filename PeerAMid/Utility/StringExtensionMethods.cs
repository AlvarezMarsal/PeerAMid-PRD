using System.Globalization;
using System.Text;

namespace PeerAMid.Utility;

public static class StringExtensionMethods
{
    public static string Quoted(this string s)
    {
        return Quoted(s, "\"");
    }

    public static string Quoted(this string s, string quote)
    {
        quote = quote ?? "\"";
        return quote + (s ?? "") + quote;
    }

    public static string Quoted(this string s, char quote)
    {
        return quote + (s ?? "") + quote;
    }

    public static string Left(this string s, int length)
    {
        if (s == null)
            return "";
        if (s.Length > length)
            return s.Substring(0, length);
        return s;
    }

    public static string LeftAndTrim(this string s, int length)
    {
        if (s == null)
            return "";
        s = s.TrimStart();
        if (s.Length > length)
            return s.Substring(0, length).TrimEnd();
        return s;
    }

    public static string OxfordComma(IEnumerable<string> items)
    {
        return items == null ? "" : OxfordComma(new List<string>(items));
    }

    public static string OxfordComma(IList<string> items)
    {
        if (items == null)
            return "";

        switch (items.Count)
        {
            case 0:
                return "";

            case 1:
                return items[0];

            case 2:
                return items[0] + " and " + items[1];
        }

        var b = new StringBuilder();
        b.Append(items[0]).Append(", ").Append(items[1]);
        var stop = items.Count - 1;
        for (var i = 2; i < stop; ++i)
            b.Append(", ").Append(items[i]);
        b.Append(" and ").Append(items[stop]); // not really an oxford comma, is it?
        return b.ToString();
    }

    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }
}

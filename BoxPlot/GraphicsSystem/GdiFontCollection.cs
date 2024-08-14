using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;


internal class GdiFontCollection : IEnumerable<string>
{
    private readonly Dictionary<string, Font> _fonts = new(StringComparer.InvariantCultureIgnoreCase);

    public Font this[string font]
    {
        get
        {
            if (_fonts.TryGetValue(font, out var f))
                return f;
            _fonts[font] = f = CreateFont(font);
            return f;
        }
    }

    private static Font CreateFont(string code)
    {
        var parts = code.Split(';');
        var family = parts[0];
        var size = (parts.Length > 1) ? float.Parse(parts[1]) : 8;

        var style = FontStyle.Regular;
        var i = 2;
        while (i < parts.Length)
        {
            if (parts[i] == "bold")
                style |= FontStyle.Bold;
            else if (parts[i] == "italic")
                style = FontStyle.Italic;
            ++i;
        }

        var fontFamily = new FontFamily(family);
        var ascent = (float)fontFamily.GetCellAscent(style);
        var descent = (float)fontFamily.GetCellDescent(style);
        var em = (float)fontFamily.GetEmHeight(style);
        var ratio = (ascent + descent) / em;
        var trialSize = size / ratio;
        var f = new Font(family, trialSize, style);

        return f;
    }

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var kvp in _fonts)
            yield return kvp.Key;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var kvp in _fonts)
            yield return kvp.Key;
    }
}
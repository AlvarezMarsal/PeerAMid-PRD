using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BoxPlot.GraphicsSystem;

internal class SvgGraphicsSystem : BaseGraphicsSystem
{
    private readonly StringBuilder _svg = new();
    private readonly GdiFontCollection _fonts = new();
    private int _width;
    private int _height;
    public override int Height => _height;
    public override int Width => _width;

    private StringBuilder StartElement(string name, params object[] args)
    {
        _svg.Append('<').Append(name);
        var i = 0;
        while (i < args.Length)
        {
            _svg.Append(' ').Append(args[i]).Append(" = ");
            ++i;
            _svg.Append('"').Append(args[i]).Append('"');
            ++i;
        }
        _svg.Append('>').AppendLine();
        return _svg;
    }

    private StringBuilder EndElement(string name)
    {
        _svg.Append("</").Append(name).Append('>').AppendLine();
        return _svg;
    }

    private StringBuilder AddElement(string name, params object[] args)
    {
        _svg.Append('<').Append(name);
        var i = 0;
        while (i < args.Length)
        {
            _svg.Append(' ').Append(args[i]).Append(" = ");
            ++i;
            _svg.Append('"').Append(args[i]).Append('"');
            ++i;
        }
        _svg.Append("/>").AppendLine();
        return _svg;
    }

    private int TransformY(int y) => Height - y;


    public override void Initialize(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public override void Close()
    {
        _svg.Clear();
    }

    public override string Save(string? filename)
    {
        var prolog = new StringBuilder();
        prolog.AppendLine($"<svg version=\"1.1\" width=\"{Width}\" height=\"{Height}\" xmlns=\"http://www.w3.org/2000/svg\">");
        var epilog = "</svg>";
        prolog.AppendLine("<style>");
        prolog.AppendLine(".anear { text-anchor: start; dominant-baseline: start; }");
        prolog.AppendLine(".afar { text-anchor: end; dominant-baseline: end; }");
        prolog.AppendLine(".acenter { text-anchor: middle; dominant-baseline: middle; }");
        foreach (var code in _fonts)
        {
            var parts = code.Split(';');
            prolog.Append('.').Append(code.Replace(';', '_')).AppendLine(" {");
            prolog.Append("font: ").Append(parts[1]).Append("px ").Append(parts[0]).AppendLine(";");
            prolog.AppendLine("}");
        }
        prolog.AppendLine("</style>");
        var temp = Path.GetTempFileName();
        File.WriteAllText(temp, prolog + _svg.ToString() + epilog);
        if (string.IsNullOrEmpty(filename))
            return temp;
        if (File.Exists(filename!))
            File.Delete(filename);
        File.Move(temp, filename);
        return filename!;
    }

    public override void FillRectangle(string color, int left, int top, int width, int height)
    {
        AddElement("rect", "x", left, "y", top, "width", width, "height", height, "fill", color);
    }

    public override void DrawRectangle(string color, int left, int top, int width, int height)
    {
        AddElement("rect", "x", left,
                           "y", top,
                           "width", width,
                           "height", height,
                           "fill", "none",
                           "stroke", color,
                           "stroke-width", "1px"
                           /*"vector-effect", "non-scaling-stroke"*/);

    }

    public override void DrawEllipse(string color, Rectangle rect)
    {
        var t = TransformY(rect.Top);
        var b = TransformY(rect.Bottom);
        AddElement("ellipse", "cx", (rect.Left + rect.Right) / 2,
                              "cy", (t + b) / 2,
                              "rx", rect.Width / 2,
                              "rh", rect.Height / 2,
                              "fill", "none",
                              "stroke", color,
                              "stroke-width", "1px"
                              /*"vector-effect", "non-scaling-stroke"*/);
    }


    public override void FillPolygon(string color, params int[] coords)
    {
        var b = new StringBuilder();
        for (var i = 0; i < coords.Length; ++i)
        {
            if (b.Length > 0)
                b.Append(' ');
            b.Append(coords[i++]).Append(' ').Append(TransformY(coords[i++]));
        }
        AddElement("polygon", "points", b.ToString(),
                              "fill", color);

    }

    public override void DrawString(string text, string font, string color, int x, int y, StringFormat format)
    {
        //var f = _fonts[font];

        var style = format.Alignment switch
        {
            StringAlignment.Near => "anear",
            StringAlignment.Far => "afar",
            _ => ""
        };

        StartElement("text", "x", x,
                             "y", TransformY(y),
                             "class", font.Replace(';', '_') + " " + style);
        _svg.Length -= 1;
        _svg.Append(text);
        EndElement("text");
    }

    public override void DrawString(string text, string font, string color, int x, int y, int w, int h, StringFormat format)
    {
        //var f = _fonts[font];

        var style = format.Alignment switch
        {
            StringAlignment.Near => "anear",
            StringAlignment.Far => "afar",
            _ => ""
        };

        StartElement("text", "x", x,
                             "y", TransformY(y),
                             "class", font.Replace(';', '_') + " " + style);
        _svg.Length -= 1;
        _svg.Append(text);
        EndElement("text");
    }

#if false
    public override void DrawString(string text, string font, string color, Rectangle rect, StringFormat format)
    {
        DrawString(text, font, color, rect.Left, TransformY(rect.Top), format);
        /*
        var gct = _colors[color];
        var f = _fonts[font];
        var pts = new Point[] { new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Bottom) };
        //_graphics.Transform.TransformPoints(pts);

        _graphics.ScaleTransform(1,-1);
        _graphics.TranslateTransform(0, -_bitmap.Height);
        _graphics.DrawString(text, f, gct.SolidBrush, Rectangle.FromLTRB(pts[0].X, pts[0].Y, pts[1].X, pts[1].Y), format);
        _graphics.TranslateTransform(0, _bitmap.Height);
        _graphics.ScaleTransform(1,-1);
        */
    }
#endif

    public override void DrawLine(string color, int width, int x1, int y1, int x2, int y2)
    {
        AddElement("line", "x1", x1,
                           "y1", TransformY(y1),
                           "x2", x2,
                           "y2", TransformY(y2),
                           "stroke", color,
                           "stroke-width", "1px"
                          /*"vector-effect", "non-scaling-stroke"*/);
    }

    public override SizeF MeasureString(string text, string font, Size area, StringFormat format)
    {
        var f = _fonts[font];

        var flags = TextFormatFlags.Default;
        if (format.Alignment == StringAlignment.Near)
            flags |= TextFormatFlags.Left;
        else if (format.Alignment == StringAlignment.Center)
            flags |= TextFormatFlags.HorizontalCenter;
        else if (format.Alignment == StringAlignment.Far)
            flags |= TextFormatFlags.Right;
        if (format.LineAlignment == StringAlignment.Near)
            flags |= TextFormatFlags.Top;
        else if (format.LineAlignment == StringAlignment.Center)
            flags |= TextFormatFlags.VerticalCenter;
        else if (format.LineAlignment == StringAlignment.Far)
            flags |= TextFormatFlags.Bottom;

        var s = TextRenderer.MeasureText(text, f, area, flags);
        Debug.WriteLine("SVG: " + font + " : " + text + " => " + s.Width + " x " + s.Height);
        return new SizeF(s.Width, s.Height);
    }

}

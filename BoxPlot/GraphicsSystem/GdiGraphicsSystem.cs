using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace BoxPlot.GraphicsSystem;

internal class GdiGraphicsSystem : BaseGraphicsSystem
{
    private Bitmap? _bitmap;
    private Graphics? _graphics;
    private readonly GdiColorToolCollection _colors = new();
    private readonly GdiFontCollection _fonts = new();
    private Graphics Graphics => _graphics ?? throw new InvalidOperationException("Graphics not initialized");
    private Bitmap Bitmap => _bitmap ?? throw new InvalidOperationException("Bitmap not initialized");

    public override int Height => Bitmap.Height;
    public override int Width => Bitmap.Width;


    public override void Initialize(int width, int height)
    {
        _bitmap = new Bitmap(width, height);
        _graphics = Graphics.FromImage(_bitmap);

        _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        _graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        //_graphics.TranslateTransform(0, _bitmap.Height);
        //_graphics.ScaleTransform(1, -1);
    }

    private int TransformY(int y) => _bitmap!.Height - y;

    public override void Close()
    {
        _graphics?.Dispose();
        _graphics = null;
        _bitmap?.Dispose();
        _bitmap = null;
    }

    public override string Save(string? filename)
    {
        var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
        Bitmap.Save(temp);
        if (string.IsNullOrEmpty(filename))
            return temp;
        filename = Path.GetFullPath(filename);
        File.Copy(temp, filename, true);
        File.Delete(temp);
        return filename!;
    }

    public override void FillRectangle(string color, int left, int top, int width, int height)
    {
        if ((width > 0) && (height > 0))
        {
            var gct = _colors[color];
            top = TransformY(top); // 0,0 is in the lower-left corner
            Graphics.FillRectangle(gct.SolidBrush, left, top, width, height);
        }
    }

    public override void DrawRectangle(string color, int left, int top, int width, int height)
    {
        var gct = _colors[color];
        var pen = /*(width == 0) ? */ gct.ZeroWidthPen /*: new Pen(gct.Color, width)*/;
        top = TransformY(top); // 0,0 is in the lower-left corner
        Graphics.DrawRectangle(pen, left, top, width, height);

    }

    public override void DrawEllipse(string color, Rectangle rect)
    {
        var gct = _colors[color];
        var pen = /*(width == 0) ? */ gct.ZeroWidthPen /*: new Pen(gct.Color, width)*/;
        var top = TransformY(rect.Top); // 0,0 is in the lower-left corner
        Graphics.DrawEllipse(pen, rect.Left, top, rect.Width, rect.Height);
    }


    public override void FillPolygon(string color, params int[] coords)
    {
        var gct = _colors[color];
        var p = new Point[coords.Length / 2];
        for (int i = 0, j = 0; i < p.Length; ++i)
        {
            var x = coords[j++];
            var y = TransformY(coords[j++]);
            p[i] = new Point(x, y);
        }
        Graphics.FillPolygon(gct.SolidBrush, p);

    }

    public override void DrawString(string text, string font, string color, int x, int y, StringFormat format)
    {
        var gct = _colors[color];
        var f = _fonts[font];
        var pts = new Point[] { new(x, TransformY(y)) };

        //Graphics.ScaleTransform(1, -1);
        //Graphics.TranslateTransform(0, -Bitmap.Height);
        Graphics.DrawString(text, f, gct.SolidBrush, pts[0], format);
        //Graphics.TranslateTransform(0, Bitmap.Height);
        //Graphics.ScaleTransform(1, -1);

    }

    public override void DrawString(string text, string font, string color, int left, int top, int width, int height, StringFormat format)
    {
        //       DrawString(text, font, color, rect.Left, rect.Top, format);
        //       DrawRectangle("red", rect);
        //       DrawLine("blue", new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Bottom));
        //       DrawLine("blue", new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top));

        var gct = _colors[color];
        var f = _fonts[font];
        var pts = new Point[] { new(left, TransformY(top)), new(left + width, TransformY(top + height)) };
        var r = Rectangle.FromLTRB(pts[0].X, pts[0].Y, pts[1].X, pts[1].Y);
        //Graphics.Transform.TransformPoints(pts);

        //Graphics.ScaleTransform(1, -1);
        //Graphics.TranslateTransform(0, -Bitmap.Height);
        Graphics.DrawString(text, f, gct.SolidBrush, r, format);
        //Graphics.TranslateTransform(0, Bitmap.Height);
        //Graphics.ScaleTransform(1, -1);

    }

    public override void DrawLine(string color, int width, int x1, int y1, int x2, int y2)
    {
        var gct = _colors[color];
        var pen = (width == 0) ? gct.ZeroWidthPen : new Pen(gct.Color, width);
        Graphics.DrawLine(pen, x1, TransformY(y1), x2, TransformY(y2));
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
        Debug.WriteLine("GDI: " + font + " : " + text + " => " + s.Width + " x " + s.Height);
        return new SizeF(s.Width, s.Height);
    }

    public override void Clear(string color)
    {
        Graphics.Clear(_colors[color].Color);
    }
}

internal class GdiColorTools
{
    public readonly Color Color;
    public readonly Pen ZeroWidthPen;
    public readonly Brush SolidBrush;

    public GdiColorTools(Color color)
    {
        Color = color;
        ZeroWidthPen = new Pen(color);
        SolidBrush = new SolidBrush(Color);
    }
}

internal class GdiColorToolCollection
{
    private readonly Dictionary<string, GdiColorTools> _gct = new(StringComparer.InvariantCultureIgnoreCase);

    public GdiColorToolCollection()
    {
        _gct.Add("white", new GdiColorTools(Color.White));
        _gct.Add("black", new GdiColorTools(Color.Black));
        _gct.Add("gray", new GdiColorTools(Color.Gray));
    }

    public GdiColorTools this[string color]
    {
        get
        {
            if (_gct.TryGetValue(color, out var gct))
                return gct;

            if (color[0] == '#')
            {
                var r = int.Parse(color.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(color.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(color.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                _gct[color] = gct = new GdiColorTools(Color.FromArgb(r, g, b));
                return gct;
            }

            var type = Color.White.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var property in properties)
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(property.Name, color) == 0)
                {
                    gct = new GdiColorTools((Color)property.GetValue(null));
                    _gct[color] = gct;
                    return gct;

                }
            }

            throw new Exception();

        }
    }

}



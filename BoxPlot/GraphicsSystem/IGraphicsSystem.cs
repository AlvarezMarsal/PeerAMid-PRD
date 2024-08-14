using System.Drawing;

namespace BoxPlot.GraphicsSystem;

/// <summary>
/// All coordinates given to IGraphics system are based on a
/// system where the lower-left corner of the output is 0,0.
/// Increasing Y goes UP the image; increasing X goes right.
/// </summary>
public interface IGraphicsSystem
{
    int Width { get; }
    int Height { get; }

    void Initialize(int width, int height);
    void Close();

    string Save(string? filename);

    void FillRectangle(string color, int left, int top, int width, int height);
    void DrawRectangle(string color, int left, int top, int width, int height);
    void DrawEllipse(string color, Rectangle rect);
    void FillPolygon(string color, params int[] coords);

    void DrawString(string text, string font, string color, int x, int y, StringFormat format);
    void DrawString(string text, string font, string color, int x, int y, int w, int h, StringFormat format);

    void DrawLine(string color, int width, int x1, int y1, int x2, int y2);

    int GetFontHeight(string font);
    SizeF MeasureString(string text, string font, Size area, StringFormat format);

    void Clear(string color);
}

public static class GraphicSystemExtensionMethods
{
    public static SizeF MeasureString(this IGraphicsSystem igs, string text, string font, SizeF area, StringFormat format)
        => igs.MeasureString(text, font, new Size((int)(area.Width + 0.9), (int)(area.Height + 0.9)), format);

    public static void DrawLine(this IGraphicsSystem igs, string color, Point p0, Point p2)
        => igs.DrawLine(color, 0, p0.X, p0.Y, p2.X, p2.Y);
    public static void DrawLine(this IGraphicsSystem igs, string color, int width, Point p0, Point p2)
        => igs.DrawLine(color, width, p0.X, p0.Y, p2.X, p2.Y);
    public static void DrawLine(this IGraphicsSystem igs, string color, int x1, int y1, int x2, int y2)
        => igs.DrawLine(color, 0, x1, y1, x2, y2);

    public static void FillRectangle(this IGraphicsSystem igs, string color, Rectangle rect)
        => igs.FillRectangle(color, rect.Left, rect.Top, rect.Width, rect.Height);
    public static void DrawRectangle(this IGraphicsSystem igs, string color, Rectangle rect)
        => igs.DrawRectangle(color, rect.Left, rect.Top, rect.Width, rect.Height);

    public static void FillRectangleLTRB(this IGraphicsSystem igs, string color, int left, int top, int right, int bottom)
        => igs.FillRectangle(color, left, top, right - left, bottom - top);
    public static void DrawRectangleLTRB(this IGraphicsSystem igs, string color, int left, int top, int right, int bottom)
        => igs.DrawRectangle(color, left, top, right - left, bottom - top);

    public static void DrawString(this IGraphicsSystem igs, string text, string font, string color, Rectangle rect, StringFormat format)
        => igs.DrawString(text, font, color, rect.Top, rect.Left, rect.Width, rect.Height, format);
    public static void DrawStringLTRB(this IGraphicsSystem igs, string text, string font, string color, int left, int top, int right, int bottom, StringFormat format)
        => igs.DrawString(text, font, color, left, top, right - left, bottom - top, format);

    public static void FillPolygon(this IGraphicsSystem igs, string color, params Point[] points)
    {
        var coords = new int[points.Length * 2];
        var index = 0;
        foreach (var point in points)
        {
            coords[index++] = point.X;
            coords[index++] = point.Y;
        }
        igs.FillPolygon(color, coords);
    }
}

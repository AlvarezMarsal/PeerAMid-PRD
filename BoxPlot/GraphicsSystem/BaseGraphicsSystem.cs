using System.Drawing;

namespace BoxPlot.GraphicsSystem;

internal abstract class BaseGraphicsSystem : IGraphicsSystem
{
    public abstract int Width { get; }
    public abstract int Height { get; }

    public abstract void Initialize(int width, int height);

    public abstract void Close();

    public abstract string Save(string? filename);

    public abstract void FillRectangle(string color, int left, int top, int width, int height);

    public abstract void DrawRectangle(string color, int left, int top, int width, int height);

    public abstract void DrawEllipse(string color, Rectangle rect);

    public abstract void FillPolygon(string color, params int[] coords);

    public abstract void DrawString(string text, string font, string color, int x, int y, StringFormat format);

    public abstract void DrawString(string text, string font, string color, int x, int y, int w, int h, StringFormat format);

    public abstract void DrawLine(string color, int width, int x1, int y1, int x2, int y2);

    public virtual int GetFontHeight(string font)
    {
        var parts = font.Split(';');
        if (parts.Length > 1)
        {
            if (float.TryParse(parts[1], out var h))
                return (int)(h + 0.999);

        }
        //Debugger.Break();
        return 8;
    }
    public abstract SizeF MeasureString(string text, string font, Size area, StringFormat format);

    public virtual void Clear(string color)
    {
        FillRectangle(color, 0, 0, Width, Height);
    }
}

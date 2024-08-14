using BoxPlot.GraphicsSystem;

namespace BoxPlot;

public static class GraphicsSystems
{
    public static IGraphicsSystem Gdi = new GdiGraphicsSystem();
    public static IGraphicsSystem Svg = new SvgGraphicsSystem();
}

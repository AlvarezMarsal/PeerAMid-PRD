using BoxPlot.GraphicsSystem;
using System;
using System.Drawing;
using System.Linq;

namespace BoxPlot;

internal class Axis
{
    public Orientation Orientation { get; set; }
    public readonly ValueCollection Values;
    private int _divisions;
    private int _rangeDivisionSize;
    private int _lowestTickValue;
    private int _highestTickValue;
    private int _tickCount;
    private int _axisPixelsPerRangeDivision;
    public Rectangle Bounds { get; private set; }
    public string LabelFormat = "N";


    public Axis(Orientation orientation, ValueCollection values, int divisions = -1)
    {
        Orientation = orientation;
        Values = new ValueCollection(values);
        _divisions = (divisions < 1) ? 8 : divisions;
    }


    public int Divisions
    {
        get => _divisions;
        set
        {
            if (value < 1)
                throw new ArgumentException("Divisions must be greater than zero.");
            _divisions = value;
            Calculate();
        }
    }


    private void Calculate()
    {
        var range = Values.Range;
        var minimumTickSpacing = range / Divisions;
        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimumTickSpacing, 10)));
        var residual = minimumTickSpacing / magnitude;
        if (residual > 5)
            _rangeDivisionSize = (int)(10 * magnitude);
        else if (residual > 2)
            _rangeDivisionSize = (int)(5 * magnitude);
        else if (residual > 1)
            _rangeDivisionSize = (int)(2 * magnitude);
        else
            _rangeDivisionSize = (int)magnitude;

        // Desperation
        if (_rangeDivisionSize < 1)
            _rangeDivisionSize = 1;

        _lowestTickValue = Round(Values.Min / _rangeDivisionSize) * _rangeDivisionSize;
        if (Values.Min < _lowestTickValue)
            _lowestTickValue -= _rangeDivisionSize;
        _highestTickValue = Round(Values.Max / _rangeDivisionSize) * _rangeDivisionSize;
        _tickCount = 1 + ((_highestTickValue - _lowestTickValue) / _rangeDivisionSize);

        // Desperation
        if (_tickCount < 2)
            _tickCount = 2;

        var labelDecimalPlaces = 0;
        if (range < 5)
            labelDecimalPlaces = 2;
        else if (range < 20)
            labelDecimalPlaces = 1;
        LabelFormat = "N" + labelDecimalPlaces;
    }


    public Rectangle Draw(Settings settings, int left)
    {
        Calculate();

        var graphics = settings.GraphicsSystem;

        // fill in the labels
        //var temp = new SortedList<double, string>();
        //var quartiles = new Quartiles(Values.ValidValues);
        foreach (var lv in Values.ValidValues.ToList()) // ignoring the outliers
        {
            lv.SetTag(lv.Value.ToString(LabelFormat));
        }

        var axisLabelSize = CalculateMaxAxisLabelSize(settings, graphics);

        var h = graphics.GetFontHeight(settings.AxisLabelFont);
        var alength = settings.Height - Round(h) * 2;
        _axisPixelsPerRangeDivision = alength / (_tickCount - 1); // there are x ticks, so there are x-1 gaps between ticks
        var axisLength = _axisPixelsPerRangeDivision * (_tickCount - 1);

        var axisBottomY = (settings.Height - axisLength) / 2;
        var axisTopY = axisBottomY + axisLength;
        var axisX = left + axisLabelSize.Width + axisLabelSize.Width + settings.AxisLabelGapPixels + settings.AxisTickLengthPixels;
        Bounds = Rectangle.FromLTRB(axisX, axisTopY, axisX + axisLabelSize.Width + settings.AxisLineWidth + settings.AxisLabelGapPixels, axisBottomY);

        // Draw the axis
        graphics.DrawLine(settings.AxisPenColor, settings.AxisLineWidth, axisX, axisBottomY, axisX, axisTopY);

        // draw the ticks
        var tickValue = _lowestTickValue;
        //pen = new Pen(_axis.PenColor, _axis.TickWidthPixels);
        var style = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        while (tickValue <= _highestTickValue)
        {
            var tickY = ApplyScale(tickValue);
            if (settings.AxisTickLengthPixels > 0)
                graphics.DrawLine(settings.AxisPenColor, 0, axisX, tickY, axisX - settings.AxisTickLengthPixels, tickY);
            graphics.DrawString(tickValue.ToString(LabelFormat), settings.AxisLabelFont, settings.AxisPenColor,
                axisX - settings.AxisTickLengthPixels, tickY, style);
            tickValue += _rangeDivisionSize;
        }


        return Bounds;
    }


    private static int Round(double value)
    {
        var r = Math.Abs(value) + 0.999999;
        return (int)(value < 0 ? -r : r);
    }


    public int ApplyScale(double value)
    {
        var a = value - _lowestTickValue;
        a /= _rangeDivisionSize;
        a *= _axisPixelsPerRangeDivision;
        a += Bounds.Bottom;
        return (int)(a + 0.5);
    }


    private Size CalculateMaxAxisLabelSize(Settings settings, IGraphicsSystem graphics)
    {
        SizeF maxSize = new(0, 0);
        foreach (var lv in Values.ValidValues)
        {
            var size = CalculateAxisLabelSize(settings, graphics, lv.Value);
            maxSize = maxSize with { Width = Math.Max(size.Width, maxSize.Width), Height = Math.Max(size.Height, maxSize.Height) };
        }
        return new Size(Round(maxSize.Width), Round(maxSize.Height));
    }


    private SizeF CalculateAxisLabelSize(Settings settings, IGraphicsSystem graphics, double value)
    {
        var label = value.ToString(LabelFormat);
        var style = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        var sizef = new SizeF(settings.Width, settings.Height);
        var size = graphics.MeasureString(label, settings.AxisLabelFont, sizef, style);
        return size;
    }
}

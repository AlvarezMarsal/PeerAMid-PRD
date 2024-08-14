using BoxPlot.GraphicsSystem;
using System;
using System.Drawing;


namespace BoxPlot;

public class Generator
{
    public static void Generate(Settings settings)
    {
        try
        {
            settings.Exception = null;
            settings.GraphicsSystem.Initialize(settings.Width, settings.Height);

            var g = new Generator(settings);
            g.GenerateBoxPlot();

            settings.Filename = settings.GraphicsSystem.Save(settings.Filename);
            settings.GraphicsSystem.Close();
        }
        catch (Exception ex)
        {
            settings.Exception = ex;
            //Debugger.Break();
        }
    }

    #region Actual drawing

    private readonly Settings _settings;
    private IGraphicsSystem Graphics => _settings.GraphicsSystem;
    private Axis _axis;
    //private int _lowestTickValue;
    //private int _highestTickValue;
    //private int _rangeDivision;
    //private int _tickCount;
    //private int _axisPixelsPerRangeDivision;
    //private int _axisBottomY;
    private int _axisX;
    private int _labelledValuesMaxX;
    private int _centerLineX;
    private Quartiles? _quartiles;

    private Generator(Settings settings)
    {
        _settings = settings;
        _axis = new Axis(Orientation.Vertical, _settings.Values);
    }

    private void GenerateBoxPlot()
    {
        Graphics.Clear("white");

        CalculateLabelledValuesArea();
        DrawAxis();
        _axisX = _axis.Bounds.Left;

        // Figure out where the center line is
        var horizontalSpaceLeft = _settings.Width - _axisX;
        _centerLineX = _axisX + (horizontalSpaceLeft / 2);

        // Box right edge
        var boxWidth = Round(_settings.BoxWidthFraction * horizontalSpaceLeft);
        var boxLeftEdge = _centerLineX - boxWidth / 2;
        var boxRightEdge = boxLeftEdge + boxWidth;

        // quartile lines
        _quartiles = new Quartiles(_settings.Values);
        var maxY = _axis.ApplyScale(_quartiles.Max);
        var bottomQuartileY = _axis.ApplyScale(_quartiles.P75);   // bottom quartile label -- top of yellow area
        var medianY = _axis.ApplyScale(_quartiles.P50);           // median label -- bottom of yellow area, top of green area
        var topQuartileY = _axis.ApplyScale(_quartiles.P25);      // top quartile label -- bottom of green area 
        var minY = _axis.ApplyScale(_quartiles.Min);

        var topOfUpperBox = bottomQuartileY;
        var bottomOfUpperBox = medianY;
        var topOfLowerBox = bottomOfUpperBox;
        var bottomOfLowerBox = topQuartileY;

        // Draw upper box
        var top = topOfUpperBox;
        var bottom = bottomOfUpperBox;
        //var rect = RectangleFromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        Graphics.FillRectangle(_settings.UpperBoxFillColor, boxLeftEdge, top, boxWidth, top - bottom);
        Graphics.DrawRectangle(_settings.UpperBoxEdgeColor, boxLeftEdge, top, boxWidth, top - bottom);

        // Draw lower box
        top = topOfLowerBox;
        bottom = bottomOfLowerBox;
        Graphics.FillRectangle(_settings.LowerBoxFillColor, boxLeftEdge, top, boxWidth, top - bottom);
        Graphics.DrawRectangle(_settings.LowerBoxEdgeColor, boxLeftEdge, top, boxWidth, top - bottom);


        // Draw limits
        var capLeftEdge = _centerLineX - _settings.RangeCapWidth / 2;
        var capRightEdge = _centerLineX + _settings.RangeCapWidth / 2;

        Graphics.FillRectangle(_settings.RangeCapColor, capLeftEdge, maxY, _settings.RangeCapWidth, _settings.RangeThickness);
        Graphics.DrawLine(_settings.RangeCapColor, _settings.RangeThickness, _centerLineX, maxY - _settings.RangeThickness, _centerLineX, topOfUpperBox);
        var h = Round(Graphics.GetFontHeight(_settings.LabelFont));
        Graphics.DrawStringLTRB(_quartiles.Max.ToString(LabelFormat), _settings.ValueFont, _settings.ValueColor, _axisX, maxY + h, _centerLineX - 8, maxY - h, new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
        Graphics.DrawStringLTRB("Max", _settings.LabelFont, _settings.LabelColor, _centerLineX + 8, maxY + h, _settings.Width, maxY - h, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

        Graphics.FillRectangle(_settings.RangeCapColor, capLeftEdge, minY, _settings.RangeCapWidth, _settings.RangeThickness);
        Graphics.DrawLine(_settings.RangeCapColor, _settings.RangeThickness, _centerLineX, minY - _settings.RangeThickness, _centerLineX, bottomOfLowerBox);
        Graphics.DrawStringLTRB(_quartiles.Min.ToString(LabelFormat), _settings.ValueFont, _settings.ValueColor, _axisX, minY + 10, _centerLineX - 8, minY - 10, new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
        Graphics.DrawStringLTRB("Min", _settings.LabelFont, _settings.LabelColor, _centerLineX + 8, minY + h, _settings.Width, minY - h, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

        // Draw Quartile labels
        Graphics.DrawStringLTRB(_quartiles.P75.ToString(LabelFormat), _settings.ValueFont, _settings.ValueColor, _centerLineX - 20, topOfUpperBox - 2, _centerLineX + 20, topOfUpperBox - 20, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
        Graphics.DrawStringLTRB("Bottom Quartile", _settings.LabelFont, _settings.LabelColor, boxRightEdge, topOfUpperBox - 2, _settings.Width, topOfUpperBox - 20, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far });

        Graphics.DrawStringLTRB(_quartiles.P50.ToString(LabelFormat), _settings.ValueFont, _settings.ValueColor, _centerLineX - 20, bottomOfUpperBox + 20, _centerLineX + 20, bottomOfUpperBox, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far });
        Graphics.DrawStringLTRB("Median", _settings.LabelFont, _settings.LabelColor, boxRightEdge, bottomOfUpperBox + 20, _settings.Width, bottomOfUpperBox - 20, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

        Graphics.DrawStringLTRB(_quartiles.P50.ToString(LabelFormat), _settings.ValueFont, _settings.AlternateValueColor, _centerLineX - 20, bottomOfLowerBox + 20, _centerLineX + 20, bottomOfLowerBox, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far });
        Graphics.DrawString("Top Quartile", _settings.LabelFont, _settings.LabelColor, boxRightEdge, bottomOfLowerBox, /*, Settings.Width, bottomOfLowerBox-20,*/ new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far });

        DrawLabelledValues();

#if false
        //var pen2 = new Pen(Color.Black, 1);
        _graphics.DrawLine("black", 1, Transform(centerLineX, validMaxY), Transform(centerLineX, topOfYellowBox));
        _graphics.DrawLine("black", 1, Transform(centerLineX, validMinY), Transform(centerLineX, bottomOfBlueBox));

        DrawValueLabel(_settings.Quartiles.Median, "Median", boxRightEdge + 2);
        DrawValueLabel(_settings.Quartiles.ValidMin, "Min", boxRightEdge + 2);

        var h = GetFontHeight(_settings.LabelFont);

        var overdrawLimit = (h * _rangeDivision) / _axisPixelsPerRangeDivision;

        var drawBottomQuartile = (Difference(_settings.Quartiles.BottomQuartile, _settings.Quartiles.ValidMax) > overdrawLimit)
                                    && (Difference(_settings.Quartiles.BottomQuartile, _settings.Quartiles.Median) > overdrawLimit);
        var drawBottomDecile = drawBottomQuartile ? (Difference(_settings.Quartiles.BottomDecile, _settings.Quartiles.BottomQuartile) > overdrawLimit)
                                                        && (Difference(_settings.Quartiles.BottomDecile, _settings.Quartiles.BottomQuartile) > overdrawLimit)
                                                  : (Difference(_settings.Quartiles.BottomDecile, _settings.Quartiles.ValidMax) > overdrawLimit)
                                                        && (Difference(_settings.Quartiles.BottomDecile, _settings.Quartiles.Median) > overdrawLimit);

        var drawTopDecile = (Difference(_settings.Quartiles.TopQuartile, _settings.Quartiles.Median) > overdrawLimit)
                                    && (Difference(_settings.Quartiles.TopQuartile, _settings.Quartiles.ValidMin) > overdrawLimit);
        var drawTopQuartile = drawTopDecile ? (Difference(_settings.Quartiles.TopDecile, _settings.Quartiles.TopQuartile) > overdrawLimit)
                                                        && (Difference(_settings.Quartiles.TopDecile, _settings.Quartiles.ValidMin) > overdrawLimit)
                                                  : (Difference(_settings.Quartiles.TopDecile, _settings.Quartiles.Median) > overdrawLimit)
                                                        && (Difference(_settings.Quartiles.TopDecile, _settings.Quartiles.ValidMin) > overdrawLimit);
        if (drawBottomDecile)
            DrawValueLabel(_settings.Quartiles.BottomDecile, "Bottom Decile", boxRightEdge + 2);
        if (drawBottomQuartile)
            DrawValueLabel(_settings.Quartiles.BottomQuartile, "Bottom Quartile", boxRightEdge + 2);
        if (drawTopQuartile)
            DrawValueLabel(_settings.Quartiles.TopQuartile, "Top Quartile", boxRightEdge + 2);
        if (drawTopDecile)
            DrawValueLabel(_settings.Quartiles.TopDecile, "Top Decile", boxRightEdge + 2);

        var labelFormat = "0." + new string('0', _labelDecimalPlaces);

        DrawValue(_settings.Quartiles.ValidMax, _settings.Quartiles.ValidMax.ToString(labelFormat), boxLeftEdge - 2);
        DrawValue(_settings.Quartiles.Median, _settings.Quartiles.Median.ToString(labelFormat), boxLeftEdge - 2);
        DrawValue(_settings.Quartiles.ValidMin, _settings.Quartiles.ValidMin.ToString(labelFormat), boxLeftEdge - 2);

        if (drawBottomDecile)
            DrawValue(_settings.Quartiles.BottomDecile, _settings.Quartiles.BottomDecile.ToString(labelFormat), boxLeftEdge - 2);
        if (drawBottomQuartile)
            DrawValue(_settings.Quartiles.BottomQuartile, _settings.Quartiles.BottomQuartile.ToString(labelFormat), boxLeftEdge - 2);
        if (drawTopQuartile)
            DrawValue(_settings.Quartiles.TopQuartile, _settings.Quartiles.TopQuartile.ToString(labelFormat), boxLeftEdge - 2);
        if (drawTopDecile)
            DrawValue(_settings.Quartiles.TopDecile, _settings.Quartiles.TopDecile.ToString(labelFormat), boxLeftEdge - 2);


        var cy = ApplyAxisScale(_settings.CompanyValue);
        var companyPoint = Transform(centerLineX, cy);
        _graphics.FillPolygon("black", [
            new Point(companyPoint.X,     companyPoint.Y + 5), 
            new Point(companyPoint.X - 5, companyPoint.Y - 5),
            new Point(companyPoint.X + 5, companyPoint.Y - 5)
        ]);

        var companyLabelStyle = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None,
            FormatFlags = 0
        };
        var companyLabel = _settings.Company + "\r\n" + _settings.CompanyValue.ToString(labelFormat);

        var rb = new Point(boxLeftEdge - 5, companyPoint.Y + 20);
        var tl = new Point(_axisX + 5, companyPoint.Y - 20);

        if (rb.Y > _settings.Image.HeightPixels)
        {
            rb = new Point(rb.X, _settings.Image.HeightPixels);
            tl = new Point(tl.X, rb.Y - 40);
        }

        var innerLabelRect = RectangleFromLTRB(tl.X, tl.Y, rb.X, rb.Y);

        gs.DrawLine("gray", new Point((innerLabelRect.Left + innerLabelRect.Right) / 2, (innerLabelRect.Top + innerLabelRect.Bottom) / 2), companyPoint);
        //g.FillRectangle(new SolidBrush(Color.White), innerLabelRect);
        DrawString(companyLabel, _settings.LabelFont, "black", innerLabelRect, companyLabelStyle);


        if (!settings.OmitOutliers)
        {
            foreach (var value in settings.Quartiles.Values)
            {
                if (settings.Quartiles.IsOutlier(value))
                {
                    var c = ApplyAxisScale(value);
                    gs.DrawEllipse("gray", new Rectangle(centerLineX - 1, c - 1, 3, 3));
                }
            }
        }
#endif
    }

    private void CalculateLabelledValuesArea()
    {
        if (_settings.LabeledValues.Count == 0)
        {
            _labelledValuesMaxX = 0;
            return;
        }

        _labelledValuesMaxX = Round(0.25 * _settings.Width);

        var width = 0;
        foreach (var lv in _settings.LabeledValues)
        {
            var text = lv.Tag + "\n" + lv.Value.ToString(LabelFormat);
            var size = Graphics.MeasureString(text, _settings.LabelFont, new Size(_labelledValuesMaxX, 1000), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near, FormatFlags = StringFormatFlags.FitBlackBox });
            if (size.Width > width)
                width = Round(size.Width);
        }

        _labelledValuesMaxX = width;
    }

    private void DrawAxis()
    {
        _axis.Draw(_settings, _labelledValuesMaxX);
        // Maximum number oof tick marks:   _settings.RangeDivisions
        // Calculate total range of values
        /*
        var range = _axis.Values.Range;

        var minimumTickSpacing = range / _settings.RangeDivisions;
        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimumTickSpacing, 10)));
        var residual = minimumTickSpacing / magnitude;
        if (residual > 5)
            _rangeDivision = (int)(10 * magnitude);
        else if (residual > 2)
            _rangeDivision = (int)(5 * magnitude);
        else if (residual > 1)
            _rangeDivision = (int)(2 * magnitude);
        else
            _rangeDivision = (int) magnitude;

        _lowestTickValue = Round(_settings.Quartiles.Min / _rangeDivision) * _rangeDivision;
        if (_settings.Quartiles.Min < _lowestTickValue)
            _lowestTickValue -= _rangeDivision;
        _highestTickValue = Round(_settings.Quartiles.Max / _rangeDivision) * _rangeDivision;
        _tickCount = 1 + ((_highestTickValue - _lowestTickValue) / _rangeDivision);

        var labelDecimalPlaces = 0;
        if (range < 5)
            labelDecimalPlaces = 2;
        else if (range < 20)
            labelDecimalPlaces = 1;
        _labelFormat = "N" + labelDecimalPlaces;
        */

        /*
        // We need to know the maximum width and height of the axis labels.  We assume
        // that the range of values in settings.Value also applies to the axis labels.
        SizeF axisLabelSizeF = CalculateMaxAxisLabelSize();

        var axisLabelSize = new Size(Round(axisLabelSizeF.Width), Round(axisLabelSizeF.Height));

        var h = GetFontHeight(_settings.AxisLabelFont);
        var alength = _settings.Height - Round(h) * 2;
        _axisPixelsPerRangeDivision = alength / (_tickCount - 1); // there are x ticks, so there are x-1 gaps between ticks
        var axisLength = _axisPixelsPerRangeDivision * (_tickCount - 1);

        _axisBottomY = (_settings.Height - axisLength) / 2;
        var axisTopY = _axisBottomY + axisLength;
        _axisX = _labelledValuesMaxX + axisLabelSize.Width + _settings.AxisLabelGapPixels + _settings.AxisTickLengthPixels;

        // Draw the axis
        Graphics.DrawLine(_settings.AxisPenColor, _settings.AxisLineWidth, _axisX, _axisBottomY, _axisX, axisTopY);

        // draw the ticks
        var tickValue = _lowestTickValue;
        //pen = new Pen(_axis.PenColor, _axis.TickWidthPixels);
        var style = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
        while (tickValue <= _highestTickValue)
        {
            var tickY = ApplyAxisScale(tickValue);
            if (_settings.AxisTickLengthPixels > 0)
                Graphics.DrawLine(_settings.AxisPenColor, 0, _axisX, tickY, _axisX - _settings.AxisTickLengthPixels, tickY);
            DrawString(tickValue.ToString(/*_axis.LabelFormat* /), _settings.AxisLabelFont, _settings.AxisPenColor,
                _axisX - _settings.AxisTickLengthPixels, tickY, style);
            tickValue += _rangeDivision;
        }
        */
    }

    private string LabelFormat
    {
        get
        {
            if (!string.IsNullOrEmpty(_settings.LabelFormat))
                return _settings.LabelFormat;
            if (!string.IsNullOrEmpty(_axis?.LabelFormat))
                return _axis!.LabelFormat;
            return "N";
        }
    }

    /*
    private SizeF CalculateMaxAxisLabelSize()
    {
        var size = CalculateAxisLabelSize(Round(_settings.Quartiles.Max));
        var s = CalculateAxisLabelSize(Round(_settings.Quartiles.P10));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        s = CalculateAxisLabelSize(Round(_settings.Quartiles.P25));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        s = CalculateAxisLabelSize(Round(_settings.Quartiles.P50));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        s = CalculateAxisLabelSize(Round(_settings.Quartiles.P75));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        s = CalculateAxisLabelSize(Round(_settings.Quartiles.P90));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        s = CalculateAxisLabelSize(Round(_settings.Quartiles.Min));
        size = size with {Width = Math.Max(size.Width, s.Width), Height = Math.Max(size.Height, s.Height) };
        return size;
    }


    private SizeF CalculateAxisLabelSize(double value)
    {
        var label = value.ToString(_labelFormat);
        var style = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        var sizef = new SizeF(_settings.Width, _settings.Height);
        var size = Graphics.MeasureString(label, _settings.AxisLabelFont, sizef, style);
        return size;
    }

    private static float GetFontHeight(string font)
    {
        var parts = font.Split(';');
        if (parts.Length > 1)
        {
            if (float.TryParse(parts[1], out var h))
                return h;

        }
        return 8;
    }

   private int ApplyAxisScale(double y)
        {
            var a = y - _lowestTickValue;
            a /= _rangeDivision;
            a *= _axisPixelsPerRangeDivision;
            a += _axisBottomY;
            return (int)(a + 0.5);
        }

        private void DrawString(string text, string font, string color, int x, int y, StringFormat style)
        {
            // Undo the transform
            //g.ScaleTransform(1,-1);
            //g.TranslateTransform(0, -_settings.Image.HeightPixels);

            // Draw the text
            Graphics.DrawString(text, font, color, x, y, style);

            //Redo the transform
            //g.TranslateTransform(0, _settings.Image.HeightPixels);
            //g.ScaleTransform(1,-1);
        }
    */



    private void DrawString(string text, string font, string color, int x, int y, int w, int h, StringFormat style)
    {
        // Undo the transform
        //g.ScaleTransform(1,-1);
        //g.TranslateTransform(0, -_settings.Image.HeightPixels);

        // Draw the text
        Graphics.DrawString(text, font, color, x, y, w, h, style);

        //Redo the transform
        //g.TranslateTransform(0, _settings.Image.HeightPixels);
        //g.ScaleTransform(1,-1);
    }

    private void DrawLabelledValues()
    {
        foreach (var lv in _settings.LabeledValues)
        {
            var markerX = _centerLineX;
            var markerY = _axis.ApplyScale(Round(lv.Value));
            var s = _settings.MarkerSize / 2;
            var labelY = markerY + 25;
            Graphics.DrawLine(_settings.LabelColor, _labelledValuesMaxX, labelY, markerX, markerY);
            Graphics.FillPolygon(_settings.MarkerColor, markerX, markerY + s, markerX + s, markerY - s, markerX - s, markerY - s);
            Graphics.DrawString(lv.Tag + "\n" + lv.Value.ToString(LabelFormat), _settings.LabelFont, _settings.LabelColor, 0, labelY, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        //_labelledValuesMaxX = width;
    }

    /*
    private void DrawValueLabel(double value, string label, int x)
    {
        var style = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        var y = ApplyAxisScale(value);
        DrawString(label, _settings.LabelFont, _settings.ValueLabelColor, x, y, style);

    }
    */

    static int Round(double d)
    {
        var e = (int)(Math.Abs(d) + 0.999999);
        return (d < 0.0) ? -e : e;
    }

    #endregion

}









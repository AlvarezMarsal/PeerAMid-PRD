using BoxPlot.GraphicsSystem;
using System;

namespace BoxPlot;

public class Settings
{
    public readonly ValueCollection Values = new ValueCollection();
    //private bool _generatedQuartiles = false;
    //private Quartiles? _quartiles = null;
    private int? _rangeDivisions = null;

    public Settings() { }

    public IGraphicsSystem GraphicsSystem { get; set; } = GraphicsSystems.Gdi;
    public int Width { get; set; } = 1000;          // pixels
    public int Height { get; set; } = 1000;   // pixels
    public string Filename { get; set; } = "";

    /*
    public Quartiles Quartiles 
    { 
        get
        {
            if (_quartiles == null)
            {
                if (_values.Count == 0)
                    throw new InvalidOperationException("No values or quartiles provided");
                _quartiles ??= CalculateQuartiles(); 
                _generatedQuartiles = true;
            }
            return _quartiles;
        }

        set 
        { 
            _quartiles = value; 
            _generatedQuartiles = false;
        }
    }
    */

    //public bool OmitOutliers { get => _omitOutliers; set { _omitOutliers = value; if (_generatedQuartiles) _quartiles = null; } }
    public Exception? Exception { get; internal set; } = null;

    public string AxisPenColor { get; set; } = "#A3A1A1";

    public int RangeDivisions { get => _rangeDivisions ?? CalculateDefaultRangeDivisions(); set => _rangeDivisions = value; }

    public double BoxWidthFraction { get; set; } = 0.25;
    public string UpperBoxFillColor { get; set; } = "#FFC000";
    public string UpperBoxEdgeColor { get; set; } = "#FFC000";
    public string LowerBoxFillColor { get; set; } = "#29702A";
    public string LowerBoxEdgeColor { get; set; } = "#29702A";
    //public string BlueColor { get; set; } = "blue";

    public string LabelFont = "Arial;10;bold";
    public string LabelColor { get; set; } = "black";
    public string AlternateLabelColor { get; set; } = "white";

    public string ValueFont = "Arial;10;bold";
    public string ValueColor { get; set; } = "black";
    public string AlternateValueColor { get; set; } = "white";

    public string AxisLabelFont = "Arial;10;bold";
    public string AxisLabelColor { get; set; } = "black";
    private int? _axisTickLengthPixels;
    public int AxisTickLengthPixels { get => _axisTickLengthPixels ?? (Width / 100); set => _axisTickLengthPixels = value; }
    public int AxisLabelGapPixels { get; set; } = 0;
    public int AxisLineWidth { get; set; } = 1;

    public int RangeCapWidth { get; set; } = 6;
    public string RangeCapColor { get; set; } = "#C00000";
    public int RangeThickness { get; set; } = 2;

    public string MarkerColor { get; set; } = "blue";
    public int MarkerSize { get; set; } = 12;

    public readonly ValueCollection LabeledValues = new ValueCollection();

    public string LabelFormat = "";
    /*
    public string? Company { get; set; }
    public double CompanyValue { get; set; }
    */

    //private Quartiles CalculateQuartiles()
    //{
    //    if (_values == null)
    //        throw new ArgumentException("You must provide values or quartiles");
    //    return new Quartiles(_values);
    //}

    private int CalculateDefaultRangeDivisions()
        => 8;
}

/*
public class AxisSettings
{
    //public bool Visible { get; set; } = true;
    public int LabelGapPixels { get; set; } = 5;
    public int TickWidthPixels { get; set; } = 1;
    public int TickLengthPixels { get; set; } = 5;
    public int LineWidthPixels { get; set; } = 1;
    public string LabelFormat { get; set; } = "";
    public string PenColor { get; set; } = "gray";
    public string LabelFont = "Arial;12";
}


public class FontSettings
{
    private string _fontFamily; // = "Calibria";
    private Font? _font; // = null;
    private int _fontSize; // = 9;
    private FontStyle _fontStyle; // = FontStyle.Regular;


    public FontSettings(string family, int height, FontStyle style)
    {
        _fontFamily = family;
        _fontSize = height;
        _fontStyle = style;
    }

    public string Family
    {
        get => _fontFamily;
        set { _fontFamily = value; _font = null; }
    }

    public int Height
    {
        get => _fontSize;
        set { _fontSize = value; _font = null; }
    }

    public FontStyle Style
    {
        get => _fontStyle;
        set { _fontStyle = value; _font = null; }
    }

    public Font Font => _font ??= new Font(_fontFamily, _fontSize, _fontStyle);

    public static implicit operator Font(FontSettings settings) => settings.Font;

}
*/
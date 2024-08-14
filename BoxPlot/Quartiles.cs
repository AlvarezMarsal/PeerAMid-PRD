using System.Collections.Generic;
using System.Diagnostics;

namespace BoxPlot;

public class Quartiles
{
    private double _percentile90;
    private double _percentile75;
    private double _percentile50;
    private double _percentile25;
    private double _percentile10;

    public bool Inverted { get; set; }
    /*
    public bool Inverted => ValidMin > ValidMax;
    public double Max => _values[_values.Length - 1]; // includes outliers
    public double ValidMax { get; set; }  // valid => not an outlier

    */
    public double Min => Inverted ? Values.Max : Values.Min;
    public double P90 => Inverted ? _percentile10 : _percentile90;
    public double P75 => Inverted ? _percentile25 : _percentile75;
    // ReSharper disable once ConvertToAutoProperty
    public double P50 => _percentile50;
    public double P25 => Inverted ? _percentile75 : _percentile25;
    public double P10 => Inverted ? _percentile90 : _percentile10;
    public double Max => Inverted ? Values.Min : Values.Max;
    public double Range => Inverted ? Min - Max : Max - Min;

    public readonly ValueCollection Values;
    //public IReadOnlyList<double> Values => _values;
    //private readonly List<double> _outliers = [];
    //public IReadOnlyList<double> Outliers => _outliers;

    public Quartiles(ValueCollection values)
        : this(values.EnumerateValues())
    {
    }

    public Quartiles(IEnumerable<double> values)
    {
        Values = new ValueCollection(values);
        CalculateBreakdown();
    }

    private void CalculateBreakdown()
    {
        // We do all this to determine what the valid range is
        _percentile90 = Percentile(0.90);
        _percentile75 = Percentile(0.75);
        _percentile50 = Percentile(0.50);
        _percentile25 = Percentile(0.25);
        _percentile10 = Percentile(0.10);

        Debug.WriteLine(this);

        /*
        if (OmitOutliers)
        {
            var interquartileRange = _percentile75 - _percentile25;
            var fudgedInterquartileRange = interquartileRange * 1.5;
            var reduced25 = _percentile25 - fudgedInterquartileRange;
            var reduced75 = _percentile75 + fudgedInterquartileRange;

            var oldValues = new List<double>(_values);
            _values.Clear();
            foreach (var value in oldValues)
                if ((value < reduced25) || (value > reduced75))
                    _outliers.Add(value);
                else
                    _values.Add(value);

            _percentile90 = Percentile(0.90);
            _percentile75 = Percentile(0.75);
            _percentile50 = Percentile(0.50);
            _percentile25 = Percentile(0.25);
            _percentile10 = Percentile(0.10);

            Debug.WriteLine(this);
        }
        */
    }

    public override string ToString()
    {
        return $"INV = {Inverted}\nVAL = " + string.Join(", ", Values) + "\nOUT = " + string.Join(", ", Values.Outliers)
            + $"\nMIN = {Values.Min}\nP10 = {P10}\nP25 = {P25}\nP50 = {P50}\nP75 = {P75}\nP90 = {P90}\nMAX = {Values.Max}";
    }

    private double Percentile(double percentile)
    {
        return Values.Percentile(percentile);
    }

    /*
    public bool IsOutlier(double value)
        => (value <= ValidRangeMin) || (value >= ValidRangeMax);
    */
}
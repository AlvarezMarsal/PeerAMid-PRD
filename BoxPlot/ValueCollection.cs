using BoxPlot;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace BoxPlot;


public struct ValueCollectionEntry : IComparable<ValueCollectionEntry>
{
    public double Value { get; }
    public string? Tag { get; set; }

    public ValueCollectionEntry(double value, string? tag = null)
    {
        Value = value;
        Tag = tag;
    }

    public readonly int CompareTo(ValueCollectionEntry other)
    {
        if (this.Value < other.Value)
            return -1;
        if (this.Value > other.Value)
            return 1;
        return 0;
    }

    public override readonly string ToString() => $"{Value} {Tag}";

    public static implicit operator ValueCollectionEntry(double value) => new(value);
    public static implicit operator ValueCollectionEntry((double value, string? tag) tuple) => new(tuple.value, tuple.tag);
    public static implicit operator double(ValueCollectionEntry entry) => entry.Value;

    public void SetTag(string tag)
    {
        Tag = tag;
    }
}


public class ValueCollection : IReadOnlyList<ValueCollectionEntry>
{
    private readonly List<ValueCollectionEntry> _values;
    private ValueCollection? _validValues;
    private ValueCollection? _outliers;
    private bool _filterOutOutliers = true;
    private readonly bool _outliersHaveBeenFilteredOut = false;

    public ValueCollection()
    {
        _values = [];
    }


    public ValueCollection(IEnumerable values)
        : this()
    {
        Reset(values);
    }


    public ValueCollection(ValueCollection other)
        : this()
    {
        Reset(other);
        _outliersHaveBeenFilteredOut = other._outliersHaveBeenFilteredOut;
    }


    public bool FilterOutOutliers
    {
        get => _filterOutOutliers;
        set
        {
            if (_filterOutOutliers != value)
            {
                if (value && _outliersHaveBeenFilteredOut)
                    throw new InvalidOperationException("Cannot filter out outliers after they have been filtered out");
                _filterOutOutliers = value;
                _validValues = null;
                _outliers = null;
            }
        }
    }


    public IEnumerable<double> EnumerateValues()
    {
        foreach (var value in _values)
            yield return value.Value;
    }


    public void Reset(IEnumerable? values)
    {
        _validValues = null;
        _outliers = null;

        if (values != null)
        {
            string? tag = null;
            foreach (var value in values)
            {
                switch (value)
                {
                    case ValueCollectionEntry d:
                        tag = d.Tag;
                        Add1(d.Value, ref tag);
                        break;

                    case double d:
                        Add1(d, ref tag);
                        break;

                    case float d:
                        Add1(d, ref tag);
                        break;

                    case int d:
                        Add1(d, ref tag);
                        break;

                    case string s:
                        if (tag != null)
                            throw new ArgumentException("Tag already set");
                        tag = s;
                        break;

                    default:
                        throw new ArgumentException("Invalid value type");
                }
            }
        }

        _values.Sort((a, b) => a.Value.CompareTo(b.Value));

        return;

        void Add1(double value1, ref string? tag1)
        {
            _values.Add(new ValueCollectionEntry(value1, tag1));
        }
    }


    public ValueCollectionEntry Min => (_values.Count > 0) ? _values[0] : throw new InvalidOperationException();
    public ValueCollectionEntry Max => (_values.Count > 0) ? _values[_values.Count - 1] : throw new InvalidOperationException();
    public double Range => (_values.Count > 1) ? (Max - Min) : 0;

    public ValueCollection ValidValues
    {
        get
        {
            if (_validValues == null)
                FindOutliers();
            return _validValues!;
        }
    }

    public ValueCollection Outliers
    {
        get
        {
            if (_outliers == null)
                FindOutliers();
            return _outliers!;
        }
    }

    private void FindOutliers()
    {
        if (FilterOutOutliers && _outliersHaveBeenFilteredOut)
        {
            var p25 = Percentile(_values, 0.25);
            var p75 = Percentile(_values, 0.75);

            var interquartileRange = p75 - p25;
            var fudgedInterquartileRange = interquartileRange * 1.5;
            var reduced25 = p25 - fudgedInterquartileRange;
            var reduced75 = p75 + fudgedInterquartileRange;

            var valid = new List<ValueCollectionEntry>();
            var outliers = new List<ValueCollectionEntry>();

            foreach (var vce in _values)
            {
                if ((vce.Value < reduced25) || (vce.Value > reduced75))
                    outliers.Add(vce);
                else
                    valid.Add(vce);
            }

            _validValues = new ValueCollection(valid);
            _outliers = new ValueCollection(outliers);
        }
        else
        {
            _validValues = new ValueCollection(_values);
            _outliers = [];
        }
    }

    public IEnumerator<ValueCollectionEntry> GetEnumerator()
    {
        foreach (var value in _values)
            yield return value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var value in _values)
            yield return value;
    }

    public int Count => _values.Count;

    public ValueCollectionEntry this[int index] => _values[index];

    public double Percentile(double percentile) // (0..1)
    {
        return Percentile(ValidValues, percentile);
    }

    private static double Percentile(IReadOnlyList<ValueCollectionEntry> sortedValues, double percentile) // (0..1)
    {
        if (percentile is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(percentile));

        var n = (sortedValues.Count - 1) * percentile + 1;
        var ceiling = Math.Ceiling(n);
        if (n.IsVeryCloseTo(ceiling))
            return sortedValues[(int)ceiling];
        var floor = Math.Floor(n);
        if (n.IsVeryCloseTo(floor))
            return sortedValues[(int)floor];

        var k = (int)n;
        var d = n - k;
        //var dd = Arithmetic.Cast(d);
        var diff = sortedValues[k] - sortedValues[k - 1];
        return sortedValues[k - 1] + d * diff;
    }

    public void Add(double value, string? tag = null)
    {
        _values.Add(new ValueCollectionEntry(value, tag));
        _values.Sort();
        _validValues = null;
        _outliers = null;
    }

    public void Add(params double[] values)
    {
        foreach (var value in values)
            _values.Add(new ValueCollectionEntry(value));
        _values.Sort();
        _validValues = null;
        _outliers = null;
    }
}




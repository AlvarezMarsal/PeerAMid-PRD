using System;
using System.Collections.Generic;

namespace BoxPlot;

public class LabeledValueCollection : TaggedValueCollection
{
    public event EventHandler<EventArgs>? Changed;

    //private readonly List<LabeledValue> _values = [];
    private Quartiles? _quartiles;

    public LabeledValueCollection(IEnumerable<double> values)
        : base(values)
    {
    }

    private void OnChange(bool sort = true)
    {
        if (sort)
            _values.Sort((a, b) => a.Value.CompareTo(b.Value));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerable<double> Values
    {
        get
        {
            foreach (var lv in this)
                yield return lv.Value;
        }
    }

    public Quartiles GetQuartiles(bool omitOutliers = true)
    {
        if ((_quartiles == null) || (_quartiles.OmitOutliers != omitOutliers))
            _quartiles = new Quartiles(Values, omitOutliers);
        return _quartiles;
    }

    public IEnumerator<LabeledValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /*
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_values).GetEnumerator();
    }
    */
    public void Add(LabeledValue item)
    {
        _values.Add(item);
        OnChange();
    }

    public void Add(double value, string label) => Add(new LabeledValue(value, label));

    public void Clear()
    {
        _values.Clear();
        OnChange(false);
    }

    public bool Contains(LabeledValue item)
    {
        return _values.Contains(item);
    }

    public bool Contains(string label) => Find(label) != -1;

    public void CopyTo(LabeledValue[] array, int arrayIndex)
    {
        _values.CopyTo(array, arrayIndex);
    }

    public int Find(string label)
    {
        for (var i=0; i< _values.Count; i++)
            if (_values[i].Label == label)
                return i;
        return -1;
    }


    public bool Remove(LabeledValue item)
    {
        var removed = _values.Remove(item);
        if (removed)
            OnChange(false);
        return removed;
    }

    public bool Remove(string label)
    {
        var index = Find(label);
        if (index < 0)
            return false;
        RemoveAt(index);
        return true;
    }


    public int Count => _values.Count;

    public bool IsReadOnly => false;

    public int IndexOf(LabeledValue item)
    {
        return _values.IndexOf(item);
    }

    public int IndexOf(string label) => Find(label);

    /*
    public void Insert(int index, LabeledValue item)
    {
        _values.Insert(index, item);
        OnChange(true);
    }
    */

    public void RemoveAt(int index)
    {
        _values.RemoveAt(index);
        OnChange(false);
    }

    /*public LabeledValue this[int index] => _values[index];

    public double this[string label]
    {
        get
        {
            foreach (var lv in _values)
                if (lv.Label == label)
                    return lv.Value;
            throw new ArgumentException("Label not found", nameof(label));
        }
    }
    */
    //public double Min => _values[0].Value;
    //public double Max => _values[_values.Count-1].Value;
    public double Range => Max - Min;
}

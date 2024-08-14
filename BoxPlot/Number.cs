using System;
using System.Collections.Generic;

namespace BoxPlot;

public readonly partial struct NumericValue<T> where T : notnull
{
    public static readonly Arithmetic<T> Arithmetic;

    static NumericValue()
    {
        if (typeof(T) == typeof(double))
            Arithmetic = (Arithmetic<T>) (object) Arithmetics.Double;
        else
            throw new NotImplementedException();
    }
    
    public NumericValue(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public static implicit operator T(NumericValue<T> nv) => nv.Value;
    public static implicit operator NumericValue<T>(T value) => new(value);

    public static NumericValue<T> operator +(NumericValue<T> a, NumericValue<T> b) => new(Arithmetic.Add(a.Value, b.Value));
    public static NumericValue<T> operator -(NumericValue<T> a, NumericValue<T> b) => new(Arithmetic.Subtract(a.Value, b.Value));
    public static NumericValue<T> operator *(NumericValue<T> a, NumericValue<T> b) => new(Arithmetic.Multiply(a.Value, b.Value));
    public static NumericValue<T> operator /(NumericValue<T> a, NumericValue<T> b) => new(Arithmetic.Divide(a.Value, b.Value));
    public static NumericValue<T> operator +(NumericValue<T> a, T b) => new(Arithmetic.Add(a.Value, b));
    public static NumericValue<T> operator -(NumericValue<T> a, T b) => new(Arithmetic.Subtract(a.Value, b));
    public static NumericValue<T> operator *(NumericValue<T> a, T b) => new(Arithmetic.Multiply(a.Value, b));
    public static NumericValue<T> operator /(NumericValue<T> a, T b) => new(Arithmetic.Divide(a.Value, b));
    public static NumericValue<T> operator +(T a, NumericValue<T> b) => new(Arithmetic.Add(a, b.Value));
    public static NumericValue<T> operator -(T a, NumericValue<T> b) => new(Arithmetic.Subtract(a, b.Value));
    public static NumericValue<T> operator *(T a, NumericValue<T> b) => new(Arithmetic.Multiply(a, b.Value));
    public static NumericValue<T> operator /(T a, NumericValue<T> b) => new(Arithmetic.Divide(a, b.Value));
    public static bool operator <(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) < 0;
    public static bool operator >(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) > 0;
    public static bool operator <=(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) <= 0;
    public static bool operator >=(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) >= 0;
    public static bool operator ==(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) == 0;
    public static bool operator !=(NumericValue<T> a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) != 0;
    public static bool operator <(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) < 0;
    public static bool operator >(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) > 0;
    public static bool operator <=(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) <= 0;
    public static bool operator >=(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) >= 0;
    public static bool operator ==(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) == 0;
    public static bool operator !=(T a, NumericValue<T> b) => Arithmetic.Compare(a.Value, b.Value) != 0;
    public static bool operator <(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) < 0;
    public static bool operator >(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) > 0;
    public static bool operator <=(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) <= 0;
    public static bool operator >=(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) >= 0;
    public static bool operator ==(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) == 0;
    public static bool operator !=(NumericValue<T> a, T b) => Arithmetic.Compare(a.Value, b.Value) != 0;

    public readonly override bool Equals(object? obj)
    {
        return obj is NumericValue<T> other && Value.Equals(other.Value);
    }

    public readonly bool Equals(NumericValue<T> other)
    {
        return Value.Equals(other.Value);
    }

    public readonly override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}


public struct NumericValue<TValue, TTag>  where TValue : notnull
{
    public static readonly Arithmetic<TValue> Arithmetic;

    static NumericValue()
    {
        if (typeof(TValue) == typeof(double))
            Arithmetic = (Arithmetic<TValue>) (object) Arithmetics.Double;
        else
            throw new NotImplementedException();
    }
    
    public NumericValue(TValue value)
    {
        Value = value;
        Tag = default!;
    }

    public NumericValue(TValue value, TTag? tag)
    {
        Value = value;
        Tag = tag;
    }

    public TValue Value { get; }
    public TTag? Tag { get; set; }

    public static implicit operator TValue(NumericValue<TValue, TTag> nv) => nv.Value;
    public static implicit operator NumericValue<TValue,TTag>(TValue value) => new(value);

    public static NumericValue<TValue, TTag> operator +(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => new (Arithmetic.Add(a.Value, b.Value));
    public static NumericValue<TValue, TTag> operator -(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => new (Arithmetic.Subtract(a.Value, b.Value));
    public static NumericValue<TValue, TTag> operator *(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => new (Arithmetic.Multiply(a.Value, b.Value));
    public static NumericValue<TValue, TTag> operator /(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => new (Arithmetic.Divide(a.Value, b.Value));
    public static NumericValue<TValue, TTag> operator +(NumericValue<TValue, TTag> a, TValue b) => new (Arithmetic.Add(a.Value, b));
    public static NumericValue<TValue, TTag> operator -(NumericValue<TValue, TTag> a, TValue b) => new (Arithmetic.Subtract(a.Value, b));
    public static NumericValue<TValue, TTag> operator *(NumericValue<TValue, TTag> a, TValue b) => new (Arithmetic.Multiply(a.Value, b));
    public static NumericValue<TValue, TTag> operator /(NumericValue<TValue, TTag> a, TValue b) => new (Arithmetic.Divide(a.Value, b));
    public static NumericValue<TValue, TTag> operator +(TValue a, NumericValue<TValue, TTag> b) => new(Arithmetic.Add(a, b.Value));
    public static NumericValue<TValue, TTag> operator -(TValue a, NumericValue<TValue, TTag> b) => new(Arithmetic.Subtract(a, b.Value));
    public static NumericValue<TValue, TTag> operator *(TValue a, NumericValue<TValue, TTag> b) => new(Arithmetic.Multiply(a, b.Value));
    public static NumericValue<TValue, TTag> operator /(TValue a, NumericValue<TValue, TTag> b) => new(Arithmetic.Divide(a, b.Value));
    public static bool operator <(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) < 0;
    public static bool operator >(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) > 0;
    public static bool operator <=(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) <= 0;
    public static bool operator >=(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) >= 0;
    public static bool operator ==(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) == 0;
    public static bool operator !=(NumericValue<TValue, TTag> a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a.Value, b.Value) != 0;
    public static bool operator <(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) < 0;
    public static bool operator >(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) > 0;
    public static bool operator <=(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) <= 0;
    public static bool operator >=(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) >= 0;
    public static bool operator ==(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) == 0;
    public static bool operator !=(TValue a, NumericValue<TValue, TTag> b) => Arithmetic.Compare(a, b.Value) != 0;
    public static bool operator <(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a.Value, b) < 0;
    public static bool operator >(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a.Value, b) <= 0;
    public static bool operator <=(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a, b) <= 0;
    public static bool operator >=(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a.Value, b) >= 0;
    public static bool operator ==(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a.Value, b) == 0;
    public static bool operator !=(NumericValue<TValue, TTag> a, TValue b) => Arithmetic.Compare(a.Value, b) != 0;

    public readonly override bool Equals(object? obj)
    {
        return obj is NumericValue<TValue, TTag> other && Value.Equals(other.Value);
    }

    public readonly bool Equals(NumericValue<TValue, TTag> other)
    {
        return Value.Equals(other.Value);
    }

    public readonly override int GetHashCode()
    {
        return EqualityComparer<TValue>.Default.GetHashCode(Value);
    }

}


public interface Arithmetic<T>
{
    T Add(T a, T b);
    T Subtract(T a, T b);
    T Multiply(T a, T b);
    T Divide(T a, T b);
    T Cast(int i);
    T Cast(double d);
    T Cast(decimal d);
    int ToInt(T t);
    double ToDouble(T t);
    decimal ToDecimal(T t);
    int Compare(T a, T b);
}

public abstract class BaseArithmetic<T> : Arithmetic<T>
{
    public abstract T Add(T a, T b);
    public abstract T Subtract(T a, T b);
    public abstract T Multiply(T a, T b);
    public abstract T Divide(T a, T b);
    public abstract T Cast(int i);
    public abstract T Cast(double d);
    public abstract T Cast(decimal d);
    public abstract int ToInt(T t);
    public abstract double ToDouble(T t);
    public abstract decimal ToDecimal(T t);
    public abstract int Compare(T a, T b);
}

public class DoubleArithmetic : BaseArithmetic<double>
{
    public override double Add(double a, double b) => a + b;

    public override double Subtract(double a, double b) => a - b;

    public override double Multiply(double a, double b) => a * b;

    public override double Divide(double a, double b) => a / b;

    public override double Cast(int i) => (double) i;

    public override double Cast(double d) => d;

    public override double Cast(decimal d) => (double) d;
    
    public override int ToInt(double t) => (int) t;

    public override double ToDouble(double t) => t;
    
    public override decimal ToDecimal(double t) => (decimal) t; 

    public override int Compare(double a, double b) => a.CompareTo(b);
}


public static class Arithmetics
{
    public static readonly DoubleArithmetic Double = new DoubleArithmetic();
}
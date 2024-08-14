using System.Collections;
using System.Text;

namespace PeerAMid.Utility;

public static class JavaScriptConverter
{
    public static string ToJavaScript(object obj, string? quote = null)
    {
        var converter = new Converter();
        if (quote != null)
            converter.Quote = quote;
        converter.Convert(obj);
        return converter.ToString();
    }

    private class Converter
    {
        private readonly HashSet<object> _alreadyConverted = new();
        private readonly StringBuilder _str = new();
        public string Quote = "\"";

        public void Convert(object? value, Type? type = null)
        {
            try
            {
                if (value == null)
                {
                    // Log.Debug("Convert null");
                    _str.Append("null");
                }
                else
                {
                    type ??= value.GetType();
                    if (type.FullName == "System.String")
                    {
                        ConvertString((string)value);
                    }
                    else if (type.IsArray)
                    {
                        // Log.Debug("Convert array");
                        ConvertEnumerable((Array)value);
                    }
                    else
                    {
                        switch (type.FullName)
                        {
                            case "System.Int32":
                                _str.Append((int)value);
                                break;

                            case "System.Boolean":
                                _str.Append((bool)value ? "true" : "false");
                                break;

                            case "System.Decimal":
                                _str.Append((decimal)value);
                                break;

                            case "System.Double":
                                _str.Append((double)value);
                                break;

                            default:
                                if (type.Name.Contains("List`"))
                                    // Log.Debug("Converting list");
                                    ConvertEnumerable((IList)value);
                                else if (type.Name.Contains("Dictionary`"))
                                    // Log.Debug("Converting list");
                                    ConvertDictionary((IDictionary)value);
                                else if (type.IsPrimitive)
                                    // Log.Debug("Converting primitive");
                                    _str.Append(value);
                                else
                                    // Log.Debug("Converting object");
                                    ConvertObject(value, type);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e);
                throw;
            }
        }

        private void ConvertObject(object obj, Type type)
        {
            //Log.Debug("Converting object of type " + type.Name);
            if (_alreadyConverted.Add(obj))
            {
                _str.Append("{");
                var first = true;
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var skip = false;
                    foreach (var a in property.CustomAttributes)
                    {
                        if (a.AttributeType.Name.Contains("JsonIgnore"))
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip)
                        continue;

                    //Log.Debug("Converting property " + property.Name);
                    if (first) first = false;
                    else _str.Append(',');
                    var v = property.GetValue(obj);
                    ConvertString(property.Name);
                    _str.Append(":");
                    Convert(v);
                }

                _str.Append("}");
            }
        }

        private void ConvertEnumerable(IEnumerable enumerable)
        {
            Type? type = null;
            if (_alreadyConverted.Add(enumerable))
            {
                // Log.Debug("Converting enumerable");
                _str.Append("[");
                foreach (var e in enumerable)
                {
                    if (type == null)
                        type = e.GetType();
                    else
                        _str.Append(',');
                    Convert(e, type);
                }

                _str.Append("]");
            }
        }

        private void ConvertDictionary(IDictionary dictionary)
        {
            if (_alreadyConverted.Add(dictionary))
            {
                // Log.Debug("Converting dictionary");
                _str.Append("{");
                var first = true;
                foreach (DictionaryEntry e in dictionary)
                {
                    if (first)
                        first = false;
                    else
                        _str.Append(',');
                    Convert(e.Key);
                    _str.Append(':');
                    Convert(e.Value);
                }

                _str.Append("}");
            }
        }

        private void ConvertString(string? str)
        {
            if (str == null)
            {
                _str.Append("null");
            }
            else
            {
                var s = str.Replace(Quote, "\\" + Quote);
                _str.Append(Quote).Append(s).Append(Quote);
            }
        }

        public override string ToString()
        {
            return _str.ToString();
        }
    }
}
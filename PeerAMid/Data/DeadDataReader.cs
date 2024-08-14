using System.Data;

#nullable enable

namespace PeerAMid.Data;

public class DeadDataReader : IDataReader
{
    public DeadDataReader()
    {
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool b)
    {
    }

    public string GetName(int i)
    {
        throw new InvalidOperationException();
    }

    public string GetDataTypeName(int i)
    {
        throw new InvalidOperationException();
    }

    public Type GetFieldType(int i)
    {
        throw new InvalidOperationException();
    }

    public object GetValue(int i)
    {
        throw new InvalidOperationException();
    }

    public int GetValues(object[] values)
    {
        throw new InvalidOperationException();
    }

    public int GetOrdinal(string name)
    {
        throw new InvalidOperationException();
    }

    public bool GetBoolean(int i)
    {
        throw new InvalidOperationException();
    }

    public byte GetByte(int i)
    {
        throw new InvalidOperationException();
    }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
        throw new InvalidOperationException();
    }

    public char GetChar(int i)
    {
        throw new InvalidOperationException();
    }

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
        throw new InvalidOperationException();
    }

    public Guid GetGuid(int i)
    {
        throw new InvalidOperationException();
    }

    public short GetInt16(int i)
    {
        throw new InvalidOperationException();
    }

    public int GetInt32(int i)
    {
        throw new InvalidOperationException();
    }

    public long GetInt64(int i)
    {
        throw new InvalidOperationException();
    }

    public float GetFloat(int i)
    {
        throw new InvalidOperationException();
    }

    public double GetDouble(int i)
    {
        throw new InvalidOperationException();
    }

    public string GetString(int i)
    {
        throw new InvalidOperationException();
    }

    public decimal GetDecimal(int i)
    {
        throw new InvalidOperationException();
    }

    public DateTime GetDateTime(int i)
    {
        throw new InvalidOperationException();
    }

    public IDataReader GetData(int i)
    {
        throw new InvalidOperationException();
    }

    public bool IsDBNull(int i)
    {
        throw new InvalidOperationException();
    }

    public int FieldCount => 0;

#pragma warning disable CA1065
    public object this[int i] => throw new IndexOutOfRangeException();
#pragma warning restore CA1065

#pragma warning disable CA1065
    public object this[string name] => throw new IndexOutOfRangeException();
#pragma warning restore CA1065

    public void Close()
    {
    }

    public DataTable GetSchemaTable()
    {
        throw new NotImplementedException();
    }

    public bool NextResult()
    {
        return false;
    }

    public bool Read()
    {
        return false;
    }

    public int Depth => 0;
    public bool IsClosed => true;
    public int RecordsAffected => 0;
}

using System.Data;

namespace PeerAMid.DataAccess;

public static class DbTypeExtensionMethods
{
    public static SqlDbType ToSqlDbType(this DbType type)
    {
        return type switch
        {
            DbType.AnsiString => SqlDbType.Text,
            DbType.AnsiStringFixedLength => SqlDbType.Text,
            DbType.Binary => SqlDbType.VarBinary,
            DbType.Boolean => SqlDbType.Bit,
            DbType.Byte => SqlDbType.TinyInt,
            DbType.Currency => SqlDbType.Money,
            DbType.Date => SqlDbType.Date,
            DbType.DateTime => SqlDbType.DateTime,
            DbType.DateTime2 => SqlDbType.DateTime2,
            DbType.DateTimeOffset => SqlDbType.DateTimeOffset,
            DbType.Decimal => SqlDbType.Decimal,
            DbType.Double => SqlDbType.Float,
            DbType.Guid => SqlDbType.UniqueIdentifier,
            DbType.Int16 => SqlDbType.SmallInt,
            DbType.Int32 => SqlDbType.Int,
            DbType.Int64 => SqlDbType.BigInt,
            DbType.Object => throw new NotImplementedException(),
            DbType.SByte => SqlDbType.Int,
            DbType.Single => SqlDbType.Float,
            DbType.String => SqlDbType.Text,
            DbType.StringFixedLength => SqlDbType.Text,
            DbType.Time => SqlDbType.Time,
            DbType.UInt16 => SqlDbType.Int,
            DbType.UInt32 => SqlDbType.BigInt,
            DbType.UInt64 => SqlDbType.BigInt,
            DbType.VarNumeric => SqlDbType.Float,
            DbType.Xml => SqlDbType.Xml,
            _ => throw new NotImplementedException()
        };
    }
}
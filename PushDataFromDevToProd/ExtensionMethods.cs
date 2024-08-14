using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PushDataFromDevToProd
{
    public static class ExtensionMethods
    {
        public static string GetValueAsString(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return "NULL";
            var value = reader.GetValue(index);
            if (value == null)
                return "NULL";

            switch (reader.GetDataTypeName(index))
            {
                case "varchar":
                case "nvarchar":
                    if (value.ToString() == "NULL")
                        return "NULL";
                    return "'" + value.ToString().Replace("'", "''") + "'";

                case "datetime":
                case "datetime2":
                    return "'" + value.ToString().Replace("'", "''") + "'";

                case "int":
                case "float":
                    return value.ToString();

                default:
                    Debug.WriteLine(reader.GetDataTypeName(index));
                    return "'" + value.ToString().Replace("'", "''") + "'";
            }
        }

        public static bool IsNull(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return true;
            var value = reader.GetValue(index);
            return (value == null);
        }

        public static bool DoRecordsMatch(this SqlDataReader thisReader, SqlDataReader thatReader)
        {
            for (var i = 0; i < thisReader.FieldCount; i++)
            {
                var thisValue = thisReader.GetValue(i);
                var thisIsNull = (thisValue == null) || DBNull.Value.Equals(thisValue) || ((thisValue is string t1) && (t1 == "NULL"));
                var name = thisReader.GetName(i);
                var j = thatReader.GetOrdinal(name);
                var thatValue = thatReader.GetValue(j);
                var thatIsNull = (thatValue == null) || DBNull.Value.Equals(thatValue); // || ((thatValue is string t2) && (t2 == "NULL"));

                if (thisIsNull)
                {
                    if (thatIsNull)
                        continue;
                    return false;
                }
                else if (thatIsNull)
                {
                    return false;
                }

                if ((thisValue is double thisDouble) && (thatValue is double thatDouble))
                {
                    var diff = Math.Abs(thisDouble - thatDouble);
                    if ((diff / Math.Abs(thisDouble)) > 0.0000001)
                        return false;
                    if ((diff / Math.Abs(thatDouble)) > 0.0000001)
                        return false;
                }
                else if ((thisValue is double thisFloat) && (thatValue is double thatFloat))
                {
                    var diff = Math.Abs(thisFloat - thatFloat);
                    if ((diff / Math.Abs(thisFloat)) > 0.0000001)
                        return false;
                    if ((diff / Math.Abs(thatFloat)) > 0.0000001)
                        return false;
                }
                else if ((thisValue is DateTime thisDate) && (thatValue is DateTime thatDate))
                {
                    if ((name == "UpdatedOn") || (name == "CreatedOn"))
                        continue;
                    if (!thisValue.Equals(thatValue))
                        return false;
                }
                else
                {
                    if (!thisValue.Equals(thatValue))
                        return false;
                }
            }
            return true;
        }
    }
}
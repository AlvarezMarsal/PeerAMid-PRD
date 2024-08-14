using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PushDataFromDevToProd
{
    internal class Program
    {
        private const string DevConnectionString = "Data Source=USE1DEVICRSQL01;Initial Catalog=PeerAMid;UID=peeramiduser;PASSWORD=a";
        private const string PrdConnectionString = "Data Source=USE1PRDICRSQL01;Initial Catalog=PeerAMid;UID=peeramiduser;PASSWORD=a";
        private const bool execute = true;
        private const bool compareRecords = true;
        private static readonly DateTime? cutoffDate = new DateTime(2022, 10, 26);

        private static void Main(string[] args)
        {
            try
            {
                var program = new Program();
                program.Run(args);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Console.WriteLine(e);
            }
        }

        private void Run(string[] args)
        {
            using (var devConnection = new SqlConnection(DevConnectionString))
            {
                devConnection.Open();
                using (var prdConnection = new SqlConnection(PrdConnectionString))
                {
                    prdConnection.Open();
                    Push(devConnection, prdConnection);
                }
            }

            Console.WriteLine("Push completed");
        }

        private void Push(SqlConnection devConnection, SqlConnection prdConnection)
        {
            PushTable(devConnection, prdConnection, "dbo.PhrasesVersion", false, "CurrentVersion");
            PushTable(devConnection, prdConnection, "dbo.Phrases", false, "LogicID");
            PushTable(devConnection, prdConnection, "dbo.Regions", false, "UID");
            PushTable(devConnection, prdConnection, "dbo.CountryRegions", false, "COUNTRY");
            PushTable(devConnection, prdConnection, "dbo.SIC2D", false, "SIC2D");
            PushTable(devConnection, prdConnection, "dbo.SICINDS", false, "SIC_Code");
            PushTable(devConnection, prdConnection, "dbo.Customers", true, "UID");
            PushTable(devConnection, prdConnection, "dbo.FACTS", true, "FACTID");
        }

        private void PushTable(SqlConnection devConnection, SqlConnection prdConnection, string table, bool hasDateInfo, string uidColumn)
        {
            var prdUid = new HashSet<string>();
            using (var prdCommand = prdConnection.CreateCommand())
            {
                prdCommand.CommandText = "SELECT " + uidColumn + " FROM " + table;
                //if (hasDateInfo && cutoffDate.HasValue)
                //    prdCommand.CommandText += " WHERE COALESCE(UpdatedOn, CreatedOn) >= '" + cutoffDate + "'";
                using (var reader = prdCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var uid = reader.GetValue(0).ToString().Replace("''", "'");
                        if (!prdUid.Add(uid)) throw new Exception("Duplicate uid in " + table);
                    }
                }
            }

            using (var devCommand = devConnection.CreateCommand())
            {
                devCommand.CommandText = "SELECT * FROM " + table;
                if (hasDateInfo && cutoffDate.HasValue)
                    devCommand.CommandText += " WHERE COALESCE(UpdatedOn, CreatedOn) >= '" + cutoffDate + "'";
                using (var devReader = devCommand.ExecuteReader())
                {
                    var devMap = new ColumnMap(devReader);
                    var uidColumnIndex = devMap[uidColumn];

                    while (devReader.Read())
                    {
                        var uid = devReader.GetValue(uidColumnIndex).ToString();
                        var uidq = "'" + uid.Replace("'", "''") + "'";
                        Console.WriteLine("-- Examining " + table + " record " + uid);
                        using (var prdCommand = prdConnection.CreateCommand())
                        {
                            string command;
                            if (prdUid.Contains(uid))
                            {
                                if (compareRecords)
                                {
                                    using (var prdCommand1 = prdConnection.CreateCommand())
                                    {
                                        prdCommand1.CommandText = "SELECT * FROM " + table + " WHERE [" + uidColumn + "] = " + uidq;
                                        using (var prdReader = prdCommand1.ExecuteReader())
                                        {
                                            if (prdReader.Read() && devReader.DoRecordsMatch(prdReader))
                                                continue;
                                        }
                                    }
                                }

                                command = "UPDATE " + table + " SET ";
                                var first = true;
                                for (var i = 0; i < devReader.FieldCount; ++i)
                                {
                                    if (devReader.GetName(i) == uidColumn)
                                        continue;
                                    if (first) first = false;
                                    else command += ", ";
                                    command += $" [{devReader.GetName(i)}]={devReader.GetValueAsString(i)}";
                                }
                                command += $" WHERE [{uidColumn}] = {uidq}"; // AND (";
                                /*
                                first = true;
                                for (var i = 0; i < devReader.FieldCount; ++i)
                                {
                                    if (devReader.GetName(i) == uidColumn)
                                        continue;
                                    if (first) first = false;
                                    else command += " OR ";
                                    if (devReader.IsNull(i))
                                        command += $"[{devReader.GetName(i)}] IS NOT NULL";
                                    else
                                        command += $"[{devReader.GetName(i)}]!={devReader.GetValueAsString(i)}";
                                }
                                command += ")";*/
                            }
                            else
                            {
                                command = "INSERT INTO " + table + " (";
                                var first = true;
                                for (var i = 0; i < devReader.FieldCount; ++i)
                                {
                                    if (first) first = false;
                                    else command += ',';
                                    command += '[' + devReader.GetName(i) + ']';
                                }
                                first = true;
                                command += ") VALUES (";
                                for (var i = 0; i < devReader.FieldCount; ++i)
                                {
                                    if (first) first = false;
                                    else command += ',';
                                    command += devReader.GetValueAsString(i);
                                }
                                command += ")";
                            }

                            Debug.WriteLine(command);
                            Console.WriteLine(command);

                            if (execute)
                            {
                                prdCommand.CommandText = command;
                                prdCommand.ExecuteNonQuery();
                            }

                            prdUid.Remove(uid);
                        }
                    }
                }
            }

            using (var devCommand = devConnection.CreateCommand())
            {
                foreach (var uid in prdUid)
                {
                    var uidq = "'" + uid.Replace("'", "''") + "'";
                    devCommand.CommandText = "SELECT COUNT([" + uidColumn + "]) FROM " + table + " WHERE [" + uidColumn + "] = " + uidq;
                    var result = devCommand.ExecuteScalar();
                    if ((result is int count) && (count == 0))
                    {
                        using (var prdCommand = prdConnection.CreateCommand())
                        {
                            var command = "DELETE FROM " + table + " WHERE [" + uidColumn + "] = " + uidq;

                            Debug.WriteLine(command);
                            Console.WriteLine(command);

                            if (execute)
                            {
                                prdCommand.CommandText = command;
                                prdCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            using (var devCommand = devConnection.CreateCommand())
            {
                devCommand.CommandText = "SELECT COUNT([" + uidColumn + "]) FROM " + table;
                var devCount = devCommand.ExecuteScalar();

                using (var prdCommand = prdConnection.CreateCommand())
                {
                    prdCommand.CommandText = "SELECT COUNT([" + uidColumn + "]) FROM " + table;
                    var prdCount = prdCommand.ExecuteScalar();
                    Console.WriteLine($"-- Production has {prdCount} records in {table}; Dev has {devCount}");
                    Debug.WriteLine($"-- Production has {prdCount} records in {table}; Dev has {devCount}");
                }
            }
        }

        private class ColumnMap
        {
            private List<string> _names;
            private Dictionary<string, int> _indexes;

            public ColumnMap(SqlDataReader reader)
            {
                _names = new List<string>();
                _indexes = new Dictionary<string, int>();
                for (var i = 0; i < reader.FieldCount; ++i)
                {
                    _names.Add(reader.GetName(i));
                    _indexes.Add(reader.GetName(i), i);
                }
            }

            public int Count => _names.Count;

            public int this[string name]
            {
                get => _indexes[name];
            }

            public string this[int index]
            {
                get => _names[index];
            }
        }
    }
}
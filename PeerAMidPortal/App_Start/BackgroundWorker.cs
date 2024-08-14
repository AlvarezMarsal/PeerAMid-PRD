using PeerAMid.Support;
using PeerAMid.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace YardStickPortal;

#nullable enable

public class BackgroundWorker : IDisposable
{
    private const int WorkInterval = 15 * 60 * 1000; // 15 minutes
    private readonly ManualResetEvent _stopSignal;
    private static BackgroundWorker? _worker;
    private DateTime _previousPurgeTime = DateTime.MinValue;
    private readonly HashSet<int> _zombieProcessIds;
    private bool _disposed;
    private readonly bool _log;

    private BackgroundWorker(bool log)
    {
        _log = log;
        _zombieProcessIds = new ();
        _stopSignal = new ManualResetEvent(false);
        var thread = new Thread(Run);
        thread.Start();
    }

    public static void Start(bool log)
    {
        _worker = new BackgroundWorker(log);
    }

    public static void End()
    {
        if (_worker != null)
        {
            _worker._stopSignal?.Set();
            _worker = null;
        }
    }

    private void Run()
    {
        // Every 15 minutes, check for 'zombie' EXCEL and POWERPNT
        // instances, and kill them.

        do
        {
            KillZombieProcesses();

            var nextPurgeTime = _previousPurgeTime.AddHours(SessionData.HoursBetweenPurges);
            if (nextPurgeTime <= DateTime.Now)
            {
                PurgePrivateData();
                PurgeOldFiles();

                _previousPurgeTime = DateTime.Now;
            }

        } while (!_stopSignal!.WaitOne(WorkInterval));
    }

    private void KillZombieProcesses()
    {
        if (_log)
            Log.Info("Checking for zombie processes");

        try
        {
            var currentProcesses = new List<Process>();
            currentProcesses.AddRange(Process.GetProcessesByName("POWERPNT"));
            currentProcesses.AddRange(Process.GetProcessesByName("EXCEL"));

            foreach (var process in Process.GetProcessesByName("svchost"))
            {
                if (process.StartInfo.UserName.ToUpper() == "COMSERVEREXEC")
                    currentProcesses.Add(process);
            }

            if (_log)
            {
                if (currentProcesses.Count == 0)
                {
                    Log.Info("No potential zombies found");
                }
                else
                {
                    var b = new StringBuilder();
                    b.Append(currentProcesses[0].Id);
                    for (var i = 1; i < currentProcesses.Count; i++)
                        b.Append(',').Append(currentProcesses[i].Id);
                    Log.Info("Potential zombies: " + b.ToString());
                }
            }

            if (_log)
            {
                if (_zombieProcessIds.Count == 0)
                {
                    Log.Info("No suspected zombies");
                }
                else
                {
                    Log.Info("Suspected zombies: " + string.Join(",", _zombieProcessIds));
                }
            }

            // Zombies may have died since the last cycle
            var zpids = _zombieProcessIds.ToArray();
            foreach (var zpid in zpids)
            {
                Process? zp;
                try
                {
                    zp = Process.GetProcessById(zpid);
                }
                catch
                {
                    zp = null;
                }

                if (zp == null)
                {
                    if (_log)
                        Log.Info($"Suspected zombie {zpid} died on its own");
                    _zombieProcessIds.Remove(zpid);
                }
            }

            // Look at running processes
            foreach (var process in currentProcesses)
            {
                var id = 0;
                try
                {
                    id = process.Id;
                }
                catch (InvalidOperationException iox) when (iox.Message == "No process is associated with this object.")
                {
                    // it died quick
                    if (_log)
                        Log.Info($"Suspected zombie {id} died quickly");
                    _zombieProcessIds.Remove(id);
                    continue;
                }

                if (!_zombieProcessIds.Add(id))  // it's not new
                {
                    try
                    {
                        if (_log)
                            Log.Info("Killing suspected zombie " + id);
                        process.CloseMainWindow();
                        process.Close();
                        _zombieProcessIds.Remove(id);
                    }
                    catch (Exception ex1)
                    {
                        _zombieProcessIds.Remove(id);
                        if (!ex1.Message.Contains("process is associated"))
                        {
                            try
                            {
                                process.Kill();
                            }
                            catch (Exception ex)
                            {
                                Log.Warn(ex);
                            }
                        }
                    }
                }
                else
                {
                    if (_log)
                        Log.Info($"Moved potential zombie {id} to suspected list");
                }
            }

            if (_log)
            {
                if (_zombieProcessIds.Count == 0)
                {
                    Log.Info("No remaining zombies");
                }
                else
                {
                    Log.Info("Remaining suspected zombies: " + string.Join(",", _zombieProcessIds));
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private static void PurgePrivateData()
    {
        try
        {
            /*
            var db = DbFactory.CreateDatabase();
            using var dbCmd = db.GetStoredProcCommand("[YS].[Proc_PurgePrivateData]");
            db.AddInParameter(dbCmd, "@LifespanHours", DbType.Int32, SessionData.LifespanOfPrivateDataHours);
            db.AddInParameter(dbCmd, "@Backup", DbType.Int32, SessionData.BackupPrivateData);
            if (DatabaseAccess.LogDatabaseAccess)
                Log.Info(dbCmd);
            db.ExecuteNonQuery(dbCmd);
            */
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private static void PurgeOldFiles()
    {
        try
        {
            var folder = ConfigurationManager.AppSettings.GetForThisMachine("LogFolder");
            folder = Path.GetDirectoryName(folder);
            var files = Directory.EnumerateFiles(folder!, "*.*", SearchOption.AllDirectories);
            var cutoff = DateTime.Now.AddDays(-90);
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToUpper();
                if (ext is ".PPTX" or ".XLSM" or ".LOG")
                {
                    if (File.GetLastAccessTime(file) < cutoff)
                    {
                        try
                        {
                            File.Delete(file);
                            Log.Info("Deleted " + file);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }

    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stopSignal.Dispose(); // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BackgroundWorker()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

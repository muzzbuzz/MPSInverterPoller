using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MPSInverterPoller
{
    public class FileLog : ILogging
    {
        string Src = "Default";
        public int MaxLogFiles = 10;
        public int MaxLogSize = 1024768;
        /// <summary>
        /// Default is Logfilex.txt, x will be replaced with log number. 
        /// Log file number will be inserted just before extension, e.g LogFile1.txt
        /// </summary>
        public string LogFileName { get; set; } = "LogFile.txt";
        string _CurrLogFile = null;
        string GetLogFileName
        {
            get
            {
                if (_CurrLogFile == null) _CurrLogFile = GetNextLogFile();
                return _CurrLogFile;
            }
        }
        /// <summary>
        /// We will enumerate the directory and get the next log file number.
        /// </summary>
        /// <returns></returns>
        string GetNextLogFile()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fname = Path.GetFileNameWithoutExtension(LogFileName);
            var ext = Path.GetExtension(LogFileName);
            var files = Directory.GetFiles(path, fname + "*" + ext);
            Dictionary<int, string> AllLogs = new Dictionary<int, string>();
            foreach (var item in files)
            {
                var filename = Path.GetFileNameWithoutExtension(item);
                //get logfile num.
                var lastchars = filename.Substring(fname.Length);
                if (int.TryParse(lastchars, out var num))
                    if (!AllLogs.ContainsKey(num)) AllLogs.Add(num, item);
            }
            var orderedlogs = AllLogs.OrderBy(i => i.Key).ToList();
            //if we have too many, we'll remove them.
            RemoveOldLogs(orderedlogs, MaxLogFiles);
            if (orderedlogs.Count > 0)
            {
                var last = orderedlogs.Last();
                //check log size.
                if (IsLogSizeOk(last.Value, MaxLogSize)) return last.Value;
                else return fname + (last.Key + 1).ToString() + Path.GetExtension(LogFileName);
            }
            //no files yet, return first log file 0.
            return fname + "0" + Path.GetExtension(LogFileName);
        }

        /// <summary>
        /// Must be order by oldest to newest
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="MaxLogs"></param>
        void RemoveOldLogs(List<KeyValuePair<int, string>> logs, int MaxLogs)
        {
            if (logs.Count < MaxLogs) return;
            //remove the first x amount
            for (int i = 0; i < MaxLogs - logs.Count; i++)
            {
                var file = logs[i];
                try
                {
                    File.Delete(file.Value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FileLog:RemoveOldLogs File: {file} Err: " + ex.ToString());
                }
            }
        }

        bool IsLogSizeOk(string path, int size)
        {
            try
            {
                var fi = new FileInfo(path);
                if (fi.Length < size) return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// This will log to the debug output as well as including verbose messages in log.
        /// </summary>
        public bool EnDebug { get; set; }

        public FileLog()
        {

        }

        public bool EmptyLogger => false;

        public string E(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(msg, fp, src, ln, ELogTypes.Error);
        }
        public string E(Exception ex, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(ex.ToString(), fp, src, ln, ELogTypes.Error);
        }

        public string I(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(msg, fp, src, ln, ELogTypes.Info);
        }

        public string S(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(msg, fp, src, ln, ELogTypes.Security);
        }

        public string V(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            if (!EnDebug) return msg;
            return AddLog(msg, fp, src, ln, ELogTypes.Verbose);
        }

        public string W(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(msg, fp, src, ln, ELogTypes.Warning);
        }
        string AddLog(string msg, string filepath, string name, int line, ELogTypes type)
        {
            var fn = Path.GetFileName(name);
            var fullmsg = $"Date: {DateTime.UtcNow.ToString("dd/MM/yy HH:mm:ss")} {fn}:{name} Line:{line} LogType: {type.ToString()} Msg: {msg} \n";
            File.AppendAllText(GetLogFileName, fullmsg);
            if (EnDebug && Handy.IsInteractive()) Console.WriteLine(fullmsg);
            return fullmsg;
        }
    }
}

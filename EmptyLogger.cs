using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MPSInverterPoller
{
    public class EmptyLogger : ILogging
    {
        public bool EnDebug { get; set; } = true;
        bool ILogging.EmptyLogger => true;


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
            if (!EnDebug) return null;
            return AddLog(msg, fp, src, ln, ELogTypes.Verbose);
        }

        public string W(string msg, [CallerFilePath]string fp = null, [CallerMemberName] string src = "", [CallerLineNumber] int ln = 0)
        {
            return AddLog(msg, fp, src, ln, ELogTypes.Warning);
        }
        string AddLog(string msg, string filepath, string name, int line, ELogTypes type)
        {
            var fn = Path.GetFileName(name);
            var fullmsg = $"Date: {DateTime.UtcNow.ToString("dd/MM/yy HH:mm:ss")} {fn}:{name} Line:{line} LogType: {type.ToString()} Msg: {msg}";
            if (EnDebug) Console.WriteLine(fullmsg);
            return fullmsg;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MPSInverterPoller
{
    public class GetAlias
    {
        /// <summary>
        /// This will return the alias for a given file name.
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static string GetSymLink(string File)
        {
            ProcessStartInfo procinfo = new ProcessStartInfo();
            procinfo.FileName = "readlink";
            procinfo.Arguments = string.Format(" -f {0}", File);
            procinfo.UseShellExecute = false;
            procinfo.CreateNoWindow = true;
            procinfo.RedirectStandardOutput = true;
            procinfo.RedirectStandardError = true;
            var proc = Process.Start(procinfo);
            var OutputStr = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                OutputStr += proc.StandardOutput.ReadLine();
            }
            proc.Close();
            return OutputStr;
        }
    }
}

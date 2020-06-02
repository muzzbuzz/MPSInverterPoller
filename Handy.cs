using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MPSInverterPoller
{
    public class Handy
    {

        public static ILogging Logging = new EmptyLogger();
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }


        public static bool IsInteractive()
        {
            if (!IsLinux) return true;
            return Console.In != null && (Console.In is StreamReader || Console.In is TextReader);
        }

    }
}

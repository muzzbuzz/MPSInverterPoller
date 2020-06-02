using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller
{
    public interface ILogging
    {


        /// <summary>
        /// This indicates whether it's a placeholder logger.
        /// If this is true, then a fully implemented logger should be set.
        /// </summary>
        bool EmptyLogger { get; }
        /// <summary>
        /// Will write to debug output as well as file/db etc.
        /// </summary>
        bool EnDebug { get; set; }

        string E(System.Exception ex, string filePath = null, string methodName = null, int line = 0);
        string E(string msg, string filePath = null, string methodName = null, int line = 0);
        string W(string msg, string filePath = null, string methodName = null, int line = 0);
        string I(string msg, string filePath = null, string methodName = null, int line = 0);
        string V(string msg, string filePath = null, string methodName = null, int line = 0);
        string S(string msg, string filePath = null, string methodName = null, int line = 0);
    }
}

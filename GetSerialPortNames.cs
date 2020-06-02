using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MPSInverterPoller
{

    /// <summary>
    /// On the pi3 at least, the port names are static..
    ///             .-------- 1-1/1-1.2/1-1.2:1.0   0:1.2:1.0
    ///            |     .-- 1-1/1-1.4/1-1.4:1.0   0:1.4:1.0
    ///            |     |           ^     ^           ^
    ///  .----. .-----.-----.
    ///  |.--.| | =2= | =4= |
    ///  ||  || |-----|-----|
    ///  |`--'| | =3= | =5= |
    ///.----------------------.
    ///`----------------------'
    ///   LAN      |     |
    ///            |     `-- 1-1/1-1.5/1-1.5:1.0   0:1.5:1.0
    ///            `-------- 1-1/1-1.3/1-1.3:1.0   0:1.3:1.0
    ///                              ^     ^           ^
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public class GetSerialPortNames
    {
        public static ILogging Logging = new EmptyLogger();
        public const string UPPER_LEFT = "0:1.2:1.0-port0";
        public const string LOWER_LEFT = "0:1.3:1.0-port0";
        public const string UPPER_RIGHT = "0:1.4:1.0-port0";
        public const string LOWER_RIGHT = "0:1.5:1.0-port0";

        readonly static string serialpath = "/dev/serial/by-path/";


        public static string GetPortById(string contains)
        {
            try
            {
                if (!Handy.IsLinux) return contains;
                //this will get the serial ports listed in /dev/serial/by-id/
                if (Directory.Exists(serialpath))
                {
                    var files = Directory.GetFiles(serialpath).ToList();
                    var port = files.Where(i => i.Contains(contains)).FirstOrDefault();
                    if (port != null)
                    {
                        //get the link address
                        return GetAlias.GetSymLink(port);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.E("GetSerialPortNames:GetPortById " + ex.ToString());
            }
            return null;
        }
    }
}

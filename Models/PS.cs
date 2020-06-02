using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller.Models
{
    public class PS
    {
        public ILogging Logging { get; set; } = new EmptyLogger();

        public double PVInputPower1 { get; set; }
        public double PVInputPower2 { get; set; }


        public bool DataValid { get; set; }

        public DateTime TimeStamp { get; set; }

        public PS(byte[] data, int lengthread)
        {
            var str = "";
            try
            {
                if (lengthread < 10)
                {
                    Logging.V($"Data read too short: {UTF8Encoding.UTF8.GetString(data)}");
                    return;
                }
                //first calculate the CRC.
                //copy all the data except the last 3 bytes, which are CRC, CRC, 0x0d
                byte[] nocrc = new byte[lengthread - 3];
                for (int i = 0; i < nocrc.Length; i++) nocrc[i] = data[i];
                //check the CRC
                var crc = MpsCrc.caluCRC(nocrc);
                if (data[lengthread - 3] != crc[0] || data[lengthread - 2] != crc[1])
                {
                    Logging.V("PS Bad CRC\r\n");
                    Logging.V($"Calced CRC: {crc[0].ToString()} and {crc[1].ToString()}");
                    Logging.V($"CRC from device: {data[lengthread - 3].ToString()} and {data[lengthread - 2].ToString()}");
                    return;
                }
                str = UTF8Encoding.UTF8.GetString(data);
                var split = str.Trim().Substring(5).Split(',');
                PVInputPower1 = int.Parse(split[0]);
                PVInputPower2 = int.Parse(split[1]);
                TimeStamp = DateTime.UtcNow;
                DataValid = true;
            }
            catch (Exception ex)
            {
                Logging.V($"PS Data string {str} " + ex.ToString());
            }
        }

    }
}

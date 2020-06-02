using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller.Models
{
    public class GS
    {
        public ILogging Logging { get; set; } = new EmptyLogger();

        public int PVInputVoltage1 { get; set; }
        public int PVInputVoltage2 { get; set; }
        public double BatteryVoltage { get; set; }
        public int BatteryCapacity { get; set; }
        public double ChargingCurrent { get; set; }
        public double GridVoltage { get; set; }
        public double GridFreq { get; set; }
        public double GridCurrent { get; set; }
        public int InnerTemp { get; set; }
        public int MaxTemp { get; set; }

        public bool DataValid { get; set; }

        public DateTime TimeStamp { get; set; }

        public GS(byte[] data, int lengthread)
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
                    Logging.V("GS Bad CRC\r\n");
                    Logging.V($"Calced CRC: {crc[0].ToString()} and {crc[1].ToString()}");
                    Logging.V($"CRC from device: {data[lengthread - 3].ToString()} and {data[lengthread - 2].ToString()}");
                    return;
                }
                str = UTF8Encoding.UTF8.GetString(data);
                var split = str.Trim().Substring(5).Split(',');
                PVInputVoltage1 = int.Parse(split[0]) / 10;
                PVInputVoltage2 = int.Parse(split[1]) / 10;
                BatteryVoltage = int.Parse(split[4]);
                BatteryVoltage = BatteryVoltage / 10.0;
                BatteryCapacity = int.Parse(split[5]);
                ChargingCurrent = int.Parse(split[6]);
                ChargingCurrent = ChargingCurrent / 10.0;
                GridVoltage = int.Parse(split[7]) / 10.0;
                GridFreq = int.Parse(split[10]) / 10.0;
                GridCurrent = int.Parse(split[11]) / 10.0;
                InnerTemp = int.Parse(split[21]);
                MaxTemp = int.Parse(split[22]);
                TimeStamp = DateTime.UtcNow;
                DataValid = true;
                //Logging.I("GS Data valid!");
            }
            catch (Exception ex)
            {
                Logging.V($"GS Data string {str} " + ex.ToString());
            }
        }

    }
}

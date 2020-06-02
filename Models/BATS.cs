using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller.Models
{
    public class BATS
    {
        public ILogging Logging { get; set; } = new EmptyLogger();
        public double MaxChargingCurrent { get; set; }
        public double MaxChargingVoltage { get; set; }
        public double FloatVoltage { get; set; }
        public double CutOffVoltageGridLoss { get; set; }
        public double RecoverVoltageGridLoss { get; set; }
        public double CutOffVoltage { get; set; }
        public double RecoverVoltage { get; set; }
        public double MaxAcChargingCurrent { get; set; }
        public double MaxBatDischargeCurrentInHybrid { get; set; }

        public bool DataValid { get; set; }

        public DateTime TimeStamp { get; set; }

        public BATS(byte[] data, int lengthread)
        {
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
                    Logging.V("BATS Bad CRC\r\n");
                    Logging.V($"Calced CRC: {crc[0].ToString()} and {crc[1].ToString()}");
                    Logging.V($"CRC from device: {data[lengthread - 3].ToString()} and {data[lengthread - 2].ToString()}");
                    return;
                }
                var str = UTF8Encoding.UTF8.GetString(data);
                var split = str.Trim().Substring(5).Split(',');
                MaxChargingCurrent = int.Parse(split[0]) / 10.0;
                MaxChargingVoltage = int.Parse(split[1]) / 10.0;
                FloatVoltage = int.Parse(split[2]) / 10.0;
                CutOffVoltageGridLoss = int.Parse(split[6]) / 10.0;
                RecoverVoltageGridLoss = int.Parse(split[7]) / 10.0;
                CutOffVoltage = int.Parse(split[8]) / 10.0;
                RecoverVoltage = int.Parse(split[9]) / 10.0;
                MaxAcChargingCurrent = int.Parse(split[16]) / 10.0;
                MaxBatDischargeCurrentInHybrid = int.Parse(split[17]);
                TimeStamp = DateTime.UtcNow;
                DataValid = true;
            }
            catch (Exception ex)
            {
                Logging.V("BATS " + ex.ToString());
            }
        }


    }
}

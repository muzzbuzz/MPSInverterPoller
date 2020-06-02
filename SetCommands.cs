using System;
using System.Collections.Generic;
using System.Text;

namespace MPSInverterPoller
{
    public class SetCommands
    {

        //Set max charging current
        //result = excuteSetCommand("MCHGC", String.format("%04d", new Object[] { Integer.valueOf((int)(value * 10.0D)) }));

        //setMaxAcChargingCurrent
        //result = excuteSetCommand("MUCHGC", String.format("%04d", new Object[] { Integer.valueOf((int)(value * 10.0D)) }));

        //setMCHGV(double floatChargeVol, double maxChargeVol)
        //result = excuteSetCommand("MCHGV", String.format("%04d,%04d", new Object[] { Integer.valueOf((int)(maxChargeVol * 10.0D)), Integer.valueOf((int)(floatChargeVol * 10.0D)) }));

        //setGOLV(double value) - Set grid output voltage low loss point?
        //excuteSetCommand("GOLV", String.format("%04d", new Object[] { Integer.valueOf((int)(value* 10.0D)) }));

        //setGOHV - Set grid output voltage high loss point?
        //excuteSetCommand("GOHV", String.format("%04d", new Object[] { Integer.valueOf((int)(value * 10.0D)) }));

        //setGOLF(double value)
        //excuteSetCommand("GOLF", String.format("%04d", new Object[] { Integer.valueOf((int)(value * 100.0D)) }));

        //setGOHF(double value)
        //excuteSetCommand("GOHF", String.format("%04d", new Object[] { Integer.valueOf((int)(value * 100.0D)) }));


        //setGridWaittime(double value)
        //excuteSetCommand("FT", String.format("%03d", new Object[] { Integer.valueOf((int)value) }));

        //setGILTHV(double value) - Set the grid long time average voltage high loss point?
        //excuteSetCommand("GLTHV", String.format("%04d", new Object[] { Integer.valueOf((int)(value* 10.0D)) }));

        //setMaxPowerFeedingGrid(double value)
        //excuteSetCommand("GPMP", String.format("%06d", new Object[] { Integer.valueOf((int)value) }));

        //setLCD(String value)
        //excuteSetCommand("LST", getFomatStr(value, 2));

        //setBTNUM(double value)
        //excuteSetCommand("BTNUM", String.format("%02d", new Object[] { Integer.valueOf((int)value) }));

        //setBatDischargeVol(double batUnderVol, double underBackVol, double batWeakVol, double weakBackVol)
        //excuteSetCommand("BATDV", String.format("%04d,%04d,%04d,%04d", new Object[] { Integer.valueOf((int)(batUnderVol * 10.0D)), Integer.valueOf((int)(underBackVol * 10.0D)), Integer.valueOf((int)(batWeakVol * 10.0D)), Integer.valueOf((int)(weakBackVol * 10.0D)) }));

        //setCurrentTime(String time)
        //excuteSetCommand("DAT", time);

        //setLiBatterySetting(double mincurrent, double recovervoltage, double time)
        //excuteSetCommand("BCA", String.format("%04d,%03d,%04d", new Object[] { Integer.valueOf((int)(mincurrent * 10.0D)), Integer.valueOf((int)time), Integer.valueOf((int)(recovervoltage * 10.0D)) }));

        //loadsMachineSupplyPower(String isEnable)
        //excuteSetCommand("LON", isEnable);

        //setBatTptCompensate(double tptCompensate)
        //excuteSetCommand("BTS", String.format("%03d", new Object[] { Integer.valueOf((int)(tptCompensate * 10.0D)) }));

        //setLBF(int value) {
        //LBF<X><cr>: Setting Battery type
        //X is 0or1
        //0: lead acid battery
        //1: lithium battery
        //excuteSetCommand("BT", value);

        //activeLifeBattery()
        //excuteSetCommand("BST", null);

        //setGridPowerDeviation(double value)
        //result = excuteSetCommand("FPADJ", "0," + String.format("%04d", new Object[] { Integer.valueOf((int)value) }));
        //result = excuteSetCommand("FPADJ", "1," + String.format("%04d", new Object[] { Integer.valueOf((int) value)

        //setFeedingGridCalibrationPowerR(double value)
        //result = excuteSetCommand("FPRADJ", "0," + String.format("%04d", new Object[] { Integer.valueOf((int)value) }));
        //result = excuteSetCommand("FPRADJ", "1," + String.format("%04d", new Object[] { Integer.valueOf((int) value)

        //setBatDischargeMaxCurrentInHybridMode
        //excuteSetCommand("BDCM", String.format("%04d", new Object[] { Integer.valueOf((int)value) }));

        //setPALE(int value) { - Parallel for output? multiple inverters in parallel?
        //excuteSetCommand("PALE", value);

        //setAAPF
        //AutoAdjustPFWithPower
        //setAAPF(int isEnabled, double valueA, double valueB) {
        // String value1 = String.format("%03d", new Object[] { Integer.valueOf(vA) });
        //valueB = Math.abs(valueB) * 100.0D + 100.0D;
        //result = excuteSetCommand("AAPF", isEnabled + "," + value1 + "," + value2);

        //EmergencyPowerSupplyControl
        //setEPS(int isEnabled, double valueA, double valueB) {
        //int vA = (int)(valueA * 10.0D); - BatVoltoCutoffMainsOutput
        //String value1 = String.format("%04d", new Object[] { Integer.valueOf(vA) });
        //int vB = (int)(valueB * 10.0D); - BatVoltoTurnOnMainsOutput
        //String value2 = String.format("%04d", new Object[] { Integer.valueOf(vB) });
        //result = excuteSetCommand("EPS", isEnabled + "," + value1 + "," + value2);

        //setPLE(int value) {


        MpsInverterPoller mps;
        public SetCommands(MpsInverterPoller controller)
        {
            this.mps = controller;
        }

        public bool SetDCChargeAmps(string amps)
        {
            if (int.TryParse(amps, out var ampsint))
                return SetDCChargeAmps(ampsint);
            return false;
        }


        /// <summary>
        /// Will enable charging from grid with xx number of DC amps.
        /// </summary>
        /// <param name="amps">To disable, set value to 0</param>
        /// <returns></returns>
        public bool SetDCChargeAmps(int amps)
        {
            if (amps < 0) amps = 0;
            if (amps > 120) amps = 120;
            var ampdbl = Convert.ToInt32(amps).ToString("D3");
            //First turn on/off charge from grid
            if (amps == 0)
            {
                var resp = mps.SendSetCmd("EDB0");
                return resp.Contains("^1");
            }
            else
            {
                //enable charge from grid.
                var res = mps.SendSetCmd("EDB1");
                //set charge current
                res = mps.SendSetCmd($"MUCHGC{ampdbl}0");
                return res.Contains("^1");
            }
        }

        public bool SetBatDischargeVol(string req)
        {
            if (string.IsNullOrEmpty(req)) return false;
            var split = req.Split(" ");
            if (split.Length != 2) return false;
            //convert to doubles
            if (double.TryParse(split[0], out var v1) && double.TryParse(split[1], out var v2))
                return SetBatDischargeVol(v1, v2);
            return false;
        }

        public bool SetBatDischargeVol(double batUnderVol, double underBackVol)
        {
            double batWeakVol, weakBackVol;
            //set some limits
            batUnderVol = Limit(batUnderVol, 43, 52);
            underBackVol = Limit(underBackVol, 48, 52);

            //bat weak voltages might be for indicating when to turn on the backup generator.. :|
            //we'll just make them the same.
            batWeakVol = batUnderVol;
            weakBackVol = underBackVol;

            var under = Convert.ToInt32(batUnderVol).ToString("D3");
            var back = Convert.ToInt32(underBackVol).ToString("D3");

            var res = mps.SendSetCmd($"BATDV{under}0,{back}0,{under}0,{back}0");

            return res.Contains("^1");
        }
        //setBatDischargeMaxCurrentInHybridMode
        //excuteSetCommand("BDCM", String.format("%04d", new Object[] { Integer.valueOf((int)value) }));

        /// <summary>
        /// How many amps to discharge the battery at when in hybrid mode.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool SetBatDischargeMaxCurrentInHybridMode(string amps)
        {
            if (!double.TryParse(amps, out var v1)) return false;
            //set some limits
            var current = Limit(v1, 5, 150);
            var under = Convert.ToInt32(current).ToString("D4");
            var res = mps.SendSetCmd($"BDCM{under}");
            return res.Contains("^1");
        }

        double Limit(double req, double min, double max)
        {
            if (req < min) req = min;
            if (req > max) req = max;
            return req;
        }

    }
}

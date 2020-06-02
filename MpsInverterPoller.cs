using MPSInverterPoller.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace MPSInverterPoller
{
    public class MpsInverterPoller : IDisposable
    {
        public enum EMpsCmds
        {
            /// <summary>
            /// Power data
            /// </summary>
            GS,
            /// <summary>
            /// Battery data
            /// </summary>
            BATS,
            PS,

            PI,
            ID,
            VFW,
            VFW2,
            MD,
            PIRI,
            PIGS,
            MOD,
            PIWS,
            FLAG,
            T,
            ET,
            TPR,
            DI2,
            DI,
            CHGS,
            DM,
            BSDV,
            PRIO,
            /// <summary>
            /// int maxOutputPower = VolUtil.parseInt(qopmpStr);
            /// </summary>
            OPMP,
            /// <summary>
            ///  configdata.setCheck_charge(VolUtil.parseInt(retAtt[1]));
            //configdata.setCheck_accharge(VolUtil.parseInt(retAtt[2]));
            //configdata.setCheck_pvfeedgrid(VolUtil.parseInt(retAtt[3]));
            //configdata.setCheck_batdispvon(VolUtil.parseInt(retAtt[4]));
            //configdata.setCheck_batdispvloss(VolUtil.parseInt(retAtt[5]));
            //configdata.setCheck_batfeedpvon(VolUtil.parseInt(retAtt[6]));
            //configdata.setCheck_batfeedpvloss(VolUtil.parseInt(retAtt[7]));
            //configdata.setCheck_reactivePowerAutoControl(VolUtil.parseInt(retAtt[8]));
            /// </summary>
            HECS,
        }


        bool Disposed = false;
        public ILogging Log { get; set; }
        Timer TmrPoll = null;
        /// <summary>
        /// Regular update rate.
        /// </summary>
        public int UpdateRate { get; set; } = 3000;
        /// <summary>
        /// Ping every 10 seconds if we are getting faults.
        /// </summary>
        public int FailUpdateRate { get; set; } = 10000;
        public int ReqTimeout { get; set; } = 5000;

        /// <summary>
        /// A negative value means power is travelling from grid to inverter
        /// Positive value means inverter is outputting power to grid
        /// </summary>
        public event Action<float> OnPowerUpdate;

        public string Port { get; set; }
        SerialPort sp = null;

        string LastCmd = "BATS";
        //when a user requests a custom command.
        string ReqCmd = null;
        string SetCmd = null;
        string SetResp = null;

        public event Action<GS> OnGsReceived;
        public event Action<BATS> OnBatsReceived;
        public event Action<PS> OnPsReceived;


        public MpsInverterPoller(ILogging logger = null)
        {
            Log = logger ?? new EmptyLogger();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">Must be full port address! Use GetSerialPortNames for linux</param>
        public void Init(string port)
        {
            ClosePort();
            Port = port;
            // Task.Run(() => 
            //   {
            //      ReadData();
            //  });
            if (TmrPoll == null) TmrPoll = new Timer(TmrTick, this, UpdateRate, Timeout.Infinite);
            else TmrPoll.Change(UpdateRate, Timeout.Infinite);
        }

        static void TmrTick(object obj)
        {
            var wmp = obj as MpsInverterPoller;
            try
            {

                if (wmp.PollInverter())
                {
                    wmp.TmrPoll.Change(wmp.UpdateRate, Timeout.Infinite);
                    return;
                }
            }
            catch (Exception ex)
            {
                wmp.Log.E(ex);
            }
            //slow the poll down, less messages in error log, until I implement spam filtering.
            if (wmp.TmrPoll != null) wmp.TmrPoll.Change(wmp.FailUpdateRate, Timeout.Infinite);
        }

        public void Dispose()
        {
            if (Disposed) throw new ObjectDisposedException(nameof(MpsInverterPoller));
            OnPowerUpdate = null;
            Disposed = true;
            TmrPoll.Change(Timeout.Infinite, Timeout.Infinite);
            TmrPoll.Dispose();
            TmrPoll = null;
            ClosePort();
        }

        bool OpenPort()
        {
            try
            {
                //already open
                if (sp != null && sp.IsOpen) return true;
                //close and dispose baby
                if (sp != null) ClosePort();
                sp = new SerialPort(Port);
                sp.Parity = Parity.None;
                sp.BaudRate = 2400;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.ReadTimeout = 600;
                sp.Open();
                return true;
            }
            catch (Exception ex)
            {
                Log.E(ex.ToString());
            }
            return false;
        }
        void ClosePort()
        {
            try
            {
                if (sp != null)
                {
                    if (sp.IsOpen) sp.Close();
                    sp.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            sp = null;
        }

        bool PollInverter()
        {
            try
            {
                if (Disposed) return false;
                if (!OpenPort()) return false;
                var strcmd = "";
                var cmd = "";
                if (SetCmd == null)
                {
                    cmd = GetNextCmd();
                    strcmd = string.Format("^P{0:000}{1}", cmd.Length + 1, cmd + "\r");
                }
                else strcmd = SetCmd;

                var data = UTF8Encoding.UTF8.GetBytes(strcmd);
                ReadThenDiscard(sp, true);
                sp.Write(data, 0, data.Length);
                int Rcved = 0;
                var rxdata = new byte[32768];
                int count = 0;
                //set timeout.
                var to = DateTime.Now.AddMilliseconds(ReqTimeout);
                bool started = false;
                do
                {
                    //Give it time to catch up.
                    Thread.Sleep(100);
                    count = sp.Read(rxdata, Rcved, sp.BytesToRead);
                    Rcved += count;
                    if (Rcved > 0) started = true;
                }
                while (DateTime.Now < to && (!started || count != 0));
                //Interpret the data
                var cutdata = new byte[Rcved];
                Array.Copy(rxdata, 0, cutdata, 0, Rcved);
                if (SetCmd == null) ParseData(cutdata, cmd);
                else
                {
                    if (Rcved > 0)
                        SetResp = UTF8Encoding.UTF8.GetString(cutdata);
                    SetCmd = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.E(ex);
            }
            return false;
        }
        bool Exit = false;

        public string SendCustCmd(string cmd)
        {
            cmd = cmd.Trim().ToUpper();
            if (string.IsNullOrEmpty(cmd)) return "Bad command request";
            ReqCmd = cmd;
            return null;
        }

        /// <summary>
        /// Use the SetCommands class to generate request.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public string SendSetCmd(string cmd)
        {
            try
            {
                //got one in progress already...
                if (SetCmd != null) return "^0";
                SetResp = null;
                SetCmd = string.Format("^S{0:000}{1}\r", cmd.Length + 1, cmd);
                var to = DateTime.Now.AddMilliseconds(12000);
                //wait a bit
                while (DateTime.Now < to && SetCmd != null)
                    Thread.Sleep(50);
                if (SetCmd != null)
                {
                    //error, it wasn't sent..
                    SetCmd = null;
                    SetResp = null;
                    return "^0";
                }
                return SetResp;
            }
            catch (Exception ex)
            {
                Log.E(ex);
            }
            return "^0";
        }

        void ReadData()
        {
            //just keep reading..
            while (!Exit)
            {
                try
                {
                    if (OpenPort())
                    {
                        ReadThenDiscard(sp, false);
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Error reading data");
                }
                Thread.Sleep(100);
            }
        }

        string GetNextCmd()
        {
            if (ReqCmd != null)
            {
                LastCmd = ReqCmd;
                ReqCmd = null;
                Console.WriteLine($"Sent {LastCmd.ToString()}");
                return LastCmd;
            }
            if (LastCmd == EMpsCmds.BATS.ToString())
                return LastCmd = EMpsCmds.GS.ToString();
            else if (LastCmd == EMpsCmds.GS.ToString())
                return LastCmd = EMpsCmds.PS.ToString();
            else return LastCmd = EMpsCmds.BATS.ToString();
        }

        void ReadThenDiscard(SerialPort sp, bool discard = false)
        {
            var str = "";
            if (sp.BytesToRead != 0)
            {
                var data = new byte[sp.BytesToRead + 1];
                sp.Read(data, 0, sp.BytesToRead);
                str = UTF8Encoding.UTF8.GetString(data);
                Log.V($"Read: {str}");
            }
            if (discard)
            {
                Log.V($"Discarded: {str}");
                sp.DiscardInBuffer();
            }
        }

        void ParseData(byte[] data, string cmd)
        {
            switch (cmd)
            {
                case "GS":
                    var gs = new GS(data, data.Length);
                    if (gs.DataValid)
                    {
                        OnGsReceived?.Invoke(gs);
                        Log.V("New GS data received from MPS Inverter");
                    }
                    break;
                case "BATS":
                    var bats = new BATS(data, data.Length);

                    if (bats.DataValid)
                    {
                        OnBatsReceived?.Invoke(bats);
                        Log.V("New BATS data received from MPS Inverter");
                    }
                    break;
                case "PS":
                    var ps = new PS(data, data.Length);
                    if (ps.DataValid)
                    {
                        OnPsReceived?.Invoke(ps);
                        Log.V("New PS data received from MPS Inverter");
                    }
                    break;
                default:
                    //CUSTOM command, write output.
                    try
                    {
                        var res = UTF8Encoding.UTF8.GetString(data);
                        Log.V(res);
                        Debug.WriteLine(res);
                    }
                    catch (Exception ex)
                    {

                    }
                    break;
            }
        }
    }

}

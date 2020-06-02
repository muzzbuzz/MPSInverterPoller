using MPSInverterPoller.Models;
using System;
using System.Threading;

namespace MPSInverterPoller
{
    
    class Program
    {
        static MpsInverterPoller MpsInverterPoll = null;
        static SetCommands sc = null;
        static ILogging Log = new FileLog();

        static GS LastGS = null;
        static BATS LastBats = null;
        static PS LastPS = null;
        static Timer TmrPeriodic = null;

        static int PauseShowData = 16;
        static int PauseShowSp = 15;

        static bool InMenu = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start polling....");
            Console.ReadLine();

            MpsInverterPoll = new MpsInverterPoller(Log);
            MpsInverterPoll.OnBatsReceived += MpsInverterPoll_OnBatsReceived;
            MpsInverterPoll.OnGsReceived += MpsInverterPoll_OnGsReceived;
            MpsInverterPoll.OnPsReceived += MpsInverterPoll_OnPsReceived;

            MpsInverterPoll.Init(GetSerialPortNames.GetPortById(GetSerialPortNames.LOWER_LEFT));
            sc = new SetCommands(MpsInverterPoll);
            Menu();
        }


        static void Menu()
        {
            bool exit = false;
            TmrPeriodic = new Timer(TmrTick, null, 3000, 1500);
            string resp = "";
            while (!exit)
            {
                Console.WriteLine("Press enter for menu");
                resp = Console.ReadLine().ToUpper();
                if (!string.IsNullOrEmpty(resp)) InMenu = true;
                if (string.IsNullOrEmpty(resp))
                {
                    PauseShowData = 0;
                    Console.Clear();
                    Console.WriteLine("1-Enable debug");
                    Console.WriteLine("2-Disable debug");
                    Console.WriteLine("3-Send custom CMD");
                    Console.WriteLine("4-Set batt discharge voltages");
                    Console.WriteLine("5-Set DC amps charge from grid");
                    Console.WriteLine("7-Set DC discharge amps");
                }
                else if (resp == "1") Log.EnDebug = true;
                else if (resp == "2") Log.EnDebug = false;
                else if (resp == "3")
                {
                    Console.WriteLine("Enter command: E.g BATS");
                    resp = Console.ReadLine();
                    var res = MpsInverterPoll.SendCustCmd(resp);
                    if (res != null) Console.WriteLine(res);
                }
                else if (resp == "4")
                {
                    Console.WriteLine("Enter battery low limit then recovery limit, e.g 45.3 49.0");
                    resp = Console.ReadLine();
                    if (sc.SetBatDischargeVol(resp)) Console.WriteLine("Success");
                    else Console.WriteLine("Error, check syntax?");
                }
                else if (resp == "5")
                {
                    Console.WriteLine("Enter DC charge amps from grid, Eg 50, enter 0 to disable charge from grid");
                    resp = Console.ReadLine();
                    if (sc.SetDCChargeAmps(resp)) Console.WriteLine("Success");
                    else Console.WriteLine("Error, check syntax?");
                }               
                else if (resp == "7")
                {
                    Console.WriteLine("Enter DC discharge amps Eg 50");
                    resp = Console.ReadLine();
                    if (sc.SetBatDischargeMaxCurrentInHybridMode(resp)) Console.WriteLine("Success");
                    else Console.WriteLine("Error, check syntax?");
                }
                InMenu = false;
            }
            resp = Console.ReadLine();
        }
        static void TmrTick(object obj)
        {
            ShowData();
        }

        static void ShowData()
        {
            if (InMenu) return;
            PauseShowData++;
            if (PauseShowData < PauseShowSp) return;

            var pwr = 0.0;
            if (LastGS != null)
                pwr = LastGS.BatteryVoltage * LastGS.ChargingCurrent;

            Console.Clear();
            Console.WriteLine("\r\n");
            Console.WriteLine("\r\n");
            Console.WriteLine($"MPS Battery Volts: {LastGS?.BatteryVoltage} Amps: {LastGS?.ChargingCurrent} Power: {pwr}");
            Console.WriteLine($"PV1 output: {LastPS?.PVInputPower1}");
            Console.WriteLine($"Max grid charge current: {LastBats?.MaxAcChargingCurrent}");
        }
      
        private static void MpsInverterPoll_OnGsReceived(Models.GS obj)
        {
            LastGS = obj;
        }

        private static void MpsInverterPoll_OnBatsReceived(Models.BATS obj)
        {
            LastBats = obj;
        }

        private static void MpsInverterPoll_OnPsReceived(PS obj)
        {
            LastPS = obj;
            
        }
    }
}

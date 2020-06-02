# MPSInverterPoller
 Cross platform MPS / Voltronic inverter polling code, raspberry pi, windows, linux.
 
 ABSOLUTELY NO WARRANTY PROVIDED. I OWN A 5K MPS HYBRID INVERTER, I believe that Voltronic re-brand these inverters and should be compatible with similar models, reading commands is low risk, DO NOT issue commands if you don't know what you're doing / not sure if you're inverter is compatible ETC.
 
 This is a dirty quick project I've knocked up to read my MPS 5K hybrid inverter over the serial port (NOT THE USB PORT).
 
 To get started on Windows, I recommend downloading and installing Visual studio community edition, it's free and works nicely out of the box. On installation you'll need to install .NET desktop development, shouldn't require much more than that, plenty of info out there on how to compile a .net core3.1 Console application.
 
 You will need VS2019! 2017 doesn't support .net core 3.1 (or anything above 2.2 I think?)
 
You can use VS code if on other platforms etc, but I you'll have to research that yourself.

This code is ready to use on a Raspberry PI! To use on windows, you will need to replace this line in Program.cs

MpsInverterPoll.Init(GetSerialPortNames.GetPortById(GetSerialPortNames.LOWER_LEFT));

With something like: 

MpsInverterPoll.Init("COM1");

For the PI, use a compatible USB to serial converter, the MPS is RS232 voltage levels (+- 10ish volts, don't use a TTL 5v or 3.3v FTDI device for Arduino)

The USB/Serial converter is probably compatible if you issue the comand 'ls -la /dev | grep ttyUSB' note the devices, then plug in the converter, you should see another one pop up. At the moment, the code presumes you've plugged it into the LOWER_LEFT port, so looking at the ports with Ethernet on the left, it's the bottom left one.

If you compile the code in visual studio it will compile a version for Windows. To compile for the Raspberry PI, you will need to open the Package Manager console, (press ctrl + q and type in package manager and it will be shown), type in 'dotnet publish -r linux-arm' without quotes. It will produce an output directory in MPSInverterPoller\bin\Debug\netcoreapp3.1\linux-arm\publish.

Copy the contents of the publish directory to your pi using FTP or WinSCP or whatever, then on the PI navigate to the directory and issue the command 

sudo chmod +x MPSInverterPoller
This will make it executable, you can then run the program by typing sudo ./MPSInverterPoller

Press enter to start polling. It will simply provide some basic battery charge data and solar power output to begin with.

Additional data is accessible, have a look inside the BATS, PS, GS files and you can see what is available.



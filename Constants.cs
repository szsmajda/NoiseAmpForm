using System;

namespace NoiseAmpControlApp
{
    public static class Constants
    {
        public const string AudioFile = "test.wav";

        public const int NoiseBaudRate = 38400;
        public const int CtrlBaudRate = 9600;
        public const string NoisePortName = "COM1";
        public const string CtrlPortName = "COM4";
        public const int UdpClientPort = 9;
        public const int UdpEndPointPort = 9;
        public const int UdpCh1Port = 23410;
        public const string UdpEndPointAddress = "192.168.1.178";

        public const UInt16 Noise1Cycle = 2;
        public const UInt16 Noise2Cycle = 2;
        public const UInt16 Noise3Cycle = 2;
        public const UInt16 Noise4Cycle = 2;

        public const Int16 Noise1Offset = 0;
        public const Int16 Noise2Offset = 0;
        public const Int16 Noise3Offset = 0;
        public const Int16 Noise4Offset = 0;

        public const UInt16 MinimumNoiseValue = 5;
        public const UInt16 MaximumNoiseValue = 1000;
        public const int MinimumVolumeValue = 40;
        public const int MaximumVolumeValue = 1;

        public const int NoiseMaxChr = 4;
        public const int NoiseMaxBytes = 23;
        public const int CtrlMaxBytes = 100;

        
        public const string Ack = "ACK:";
        //TTT commands
        public const string Volume1Fix = "TC*31v";
        public const string Volume2Fix = "TC*30v";
        public const string AllZoneON = "TX8";
        public const string AllZoneOFF = "TX0";
        public const string CommandEOF = ";";


        public const string KeepAlive = "KEEPALIVE";

        public const string Stream1OnFix = "X1";

        public const string Stream1Off = "C00";

        public const string MyIP = "192.168.1.174:";
        public const string Stream1Port = "23410";
        public const string Stream2Port = "23420";

        public const UInt16 DefaultStartPackets = 6;
        public const long DefaultTimerTime = 16;
        public const int DefaultTimerPackets = 3;

        //Serial commands

        public const string Vol1Fix = "*31v";
        public const string Vol2Fix = "*30v";
    }
}

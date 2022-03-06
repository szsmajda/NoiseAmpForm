using System;

namespace NoiseAmpControlApp
{
    public static class Constants
    {
        public const string AudioFile = "test.wav";

        public const int BaudRate = 38400;
        public const string PortName = "COM10";
        public const int UdpClientPort = 9;
        public const int UdpEndPointPort = 9;
        public const int UdpCh1Port = 23410;
        public const string UdpEndPointAddress = "192.168.1.122";

        public const UInt16 Noise1Cycle = 10;
        public const UInt16 Noise2Cycle = 10;
        public const UInt16 Noise3Cycle = 10;
        public const UInt16 Noise4Cycle = 10;

        public const bool N1IsEnable = true;
        public const bool N2IsEnable = true;
        public const bool N3IsEnable = false;
        public const bool N4IsEnable = false;

        public const int MaxChr = 4;
        public const int MaxBytes = 23;

        public const string VolumeFix = "TC*31v";
        public const string Ack = "ACK:";
        public const string AllZoneON = "TX8";

        public const string AllZoneOFF = "TX0";

        public const string KeepAlive = "KEEPALIVE";

        public const string Stream1OnFix = "X1";

        public const string Stream1Off = "C00";

        public const string MyIP = "192.168.1.87:";
        public const string Stream1Port = "23410";
        public const string Stream2Port = "23420";

        public const UInt16 DefaultStartPackets = 6;
        public const long DefaultTimerTime = 16;
        public const int DefaultTimerPackets = 3;
    }
}

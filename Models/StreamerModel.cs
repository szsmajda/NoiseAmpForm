using System;
using System.Collections;
using System.Collections.Generic;

namespace NoiseAmpControlApp.Models
{
    public class StreamerModel
    {
        public StreamerModel()
        {
            Ch1SampleData = new List<short>();
            Ch1SendDatagram = new List<byte>();
        }

        public UInt16 Ch1Startpackets { get; set; }
        public UInt16 DefaultStartpackets { get; set; }
        public int CommandCounter { get; set; }
        public int Ch1LastSampleSentOut { get; set; }
        public int Ch1Timerpackets { get; set; }
        public int DefaultTimerpackets { get; set; }
        public long Ch1Timertime { get; set; }
        public long DefaultTimertime { get; set; }

        public byte Ch1StreamStatus;
        public byte Ch1BuffStatus;

        public BitArray Ch1ChStats;

        public bool Ch1Needtoplay { get; set; }
        public bool Ch1Filended { get; set; }
        public bool Ch1NodataNeeded { get; set; }
        public bool Ch1StopIt { get; set; }

        public List<short> Ch1SampleData { get; set; }
        public List<byte> Ch1SendDatagram { get; set; }
    }
}

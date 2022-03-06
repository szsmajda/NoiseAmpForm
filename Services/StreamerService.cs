using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Threading;
using NoiseAmpControlApp.Models;

namespace NoiseAmpControlApp.Services
{
    public class StreamerService
    {
        public StreamerModel Model { get; set; }
        private static Timer _ch1Timer;
        public event EventHandler NeedToPlayReached;
        public event EventHandler StopItReached;
        public StreamerService()
        {
            Model = new StreamerModel();
            StreamerInit();
        }
        private void StreamerInit()
        {
            Model.DefaultStartpackets = Constants.DefaultStartPackets;
            Model.DefaultTimertime = Constants.DefaultTimerTime;
            Model.DefaultTimerpackets = Constants.DefaultTimerPackets;
            Model.CommandCounter = 10;
        }
        public void StartTimer()
        {
            _ch1Timer = new Timer(Ch1TimerCallback, null, 16, Timeout.Infinite);
        }

        public void Ch1Play()
        {
            Model.Ch1Needtoplay = true;
            Model.Ch1Filended = false;
            Model.Ch1StopIt = false;
            Model.Ch1LastSampleSentOut = 0;
            Model.Ch1SampleData.Clear();
            Model.Ch1Startpackets = Model.DefaultStartpackets;
            Model.Ch1Timerpackets = Model.DefaultTimerpackets;
            Model.Ch1Timertime = Model.DefaultTimertime;

            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(new WaveFileReader(Constants.AudioFile)))
            {
                byte[] samples = new byte[pcm.Length];

                pcm.Read(samples, 0, samples.Length);
                for (int i = 0; i < samples.Length; i = i + 2)
                {
                    long yshort = samples[i + 1];
                    yshort = yshort << 8;
                    yshort += samples[i];

                    Model.Ch1SampleData.Add((short)yshort);
                }
            }
        }

        private void Ch1TimerCallback(Object state)
        {
            Model.Ch1NodataNeeded = false;
            if (!Model.Ch1NodataNeeded)
            {
                Stopwatch watch = new Stopwatch();

                for (int i = 0; i < Model.Ch1Timerpackets; i++)
                {
                    if (Model.Ch1Needtoplay)
                    {
                        NeedToPlayReached?.Invoke(this, EventArgs.Empty);
                    }
                }

                if (Model.Ch1Needtoplay)
                {
                    if (!Model.Ch1StopIt) _ch1Timer.Change(Math.Max(0, Model.Ch1Timertime - watch.ElapsedMilliseconds), Timeout.Infinite);
                }
            }
            else if (Model.Ch1Needtoplay)
            {
                if (!Model.Ch1StopIt) _ch1Timer.Change(Math.Max(0, Model.Ch1Timertime), Timeout.Infinite);
            }

            if (Model.Ch1StopIt)
            {
                StopItReached?.Invoke(this, EventArgs.Empty);
                Model.Ch1Needtoplay = false;
            }
        }
    }
}


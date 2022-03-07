using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NoiseAmpControlApp.Services
{
    public class UdpService
    {
        private IPEndPoint _iPEndPoint;
        public string ReceivedString = string.Empty;
        private readonly StreamerService _streamerService;
        private readonly UdpClient _udpCtrlClient;
        private readonly UdpClient _udpCh1Client;
        private byte[] _counterBytes;
        private static Stopwatch stopwatch = new Stopwatch();
        
        public UdpService()
        {
            _streamerService = new StreamerService();
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(Constants.UdpEndPointAddress), Constants.UdpEndPointPort);
            _udpCtrlClient = new UdpClient(Constants.UdpClientPort);
            _udpCh1Client = new UdpClient(Constants.UdpCh1Port);
            _streamerService.NeedToPlayReached += Ch1SendPackage;
            _streamerService.StopItReached += Ch1SendOff;
            _udpCtrlClient.BeginReceive(UdpReceive, null);
        }

        private void UdpReceive(IAsyncResult res)
        {
            if (_udpCtrlClient != null)
            {
                Byte[] receivedUdpbytes = _udpCtrlClient.EndReceive(res, ref _iPEndPoint);
                ReceivedString = Encoding.ASCII.GetString(receivedUdpbytes);
            }

            _udpCtrlClient.BeginReceive(new AsyncCallback(UdpReceive), null);
        }

        public new void Send(SendTypes sendTypes)
        {
            Form1.Form.UpdateOutputConsole($"UdpService is sending {sendTypes}...");

            switch (sendTypes)
            {
                case SendTypes.SpeakOut:
                    SpeakOut();
                    break;
                case SendTypes.NoiseMeasure:
                    NoiseMeasure();
                    break;
                case SendTypes.KeepAlive:
                    KeepAlive();
                    break;
                default:
                    break;
            }
        }

        private void KeepAlive()
        {
            SetCounterBytesAndCommandCounter();

            byte[] commandBytes = Encoding.ASCII.GetBytes(Constants.KeepAlive);
            byte[] sendBytes = new byte[_counterBytes.Length + commandBytes.Length];

            Array.Copy(_counterBytes, 0, sendBytes, 0, _counterBytes.Length);
            Array.Copy(commandBytes, 0, sendBytes, _counterBytes.Length, commandBytes.Length);

            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        private void SpeakOut()
        {
            SetCounterBytesAndCommandCounter();

            string sendstring = string.Format("{0}{1:D2};", Constants.VolumeFix, NoiseFilter.Vol1);
            var sendBytes = CreateSendBytes(sendstring);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            while (!ReceivedString.Contains(Constants.Ack)) ;

            Form1.Form.UpdateOutputConsole($"Received {Constants.Ack}");

            sendBytes = CreateSendBytes(Constants.AllZoneON);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            Form1.Form.UpdateOutputConsole($"Sent {Constants.AllZoneON}");

            while (!ReceivedString.Contains(Constants.Ack)) ;

            _streamerService.Ch1Play();
            Ch1SendOn();
            for (int i = 0; i < _streamerService.Model.Ch1Startpackets; i++)
            {
                Ch1SendPackage();
            }

            _streamerService.Model.Ch1NodataNeeded = false;
            _streamerService.StartTimer();
        }

        //
        private void NoiseMeasure()
        {
            SetCounterBytesAndCommandCounter();
            var sendBytes = CreateSendBytes(Constants.AllZoneOFF);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            Form1.Form.UpdateOutputConsole($"Sent { Constants.AllZoneOFF}");

            SetCounterBytesAndCommandCounter();
            sendBytes = CreateSendBytes(Constants.Stream1Off);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        private void Ch1SendOn()
        {
            SetCounterBytesAndCommandCounter();

            string sendstring = string.Format("{0}{1}{2}", Constants.Stream1OnFix, Constants.MyIP, Constants.Stream1Port);
            var sendBytes = CreateSendBytes(sendstring);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        private void Ch1SendOff(object sender = null, EventArgs e = null)
        {
            SetCounterBytesAndCommandCounter();
            var sendBytes = CreateSendBytes(Constants.Stream1Off);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        public void Ch1SendPackage(object sender = null, EventArgs e = null)
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
            Debug.WriteLine($"Timer: {stopwatch.ElapsedMilliseconds.ToString()}");

            byte[] sendBytes;
            _streamerService.Model.Ch1SendDatagram.Clear();
            for (int z = 0; z < 4; z++) _streamerService.Model.Ch1SendDatagram.AddRange(BitConverter.GetBytes((ushort)0));

            for (int i = 0; i < 128; i++)
            {
                if (_streamerService.Model.Ch1LastSampleSentOut >= _streamerService.Model.Ch1SampleData.Count)
                {
                    _streamerService.Model.Ch1Filended = true;
                }

                if (!_streamerService.Model.Ch1Filended)
                {
                    _streamerService.Model.Ch1SendDatagram.AddRange(BitConverter.GetBytes(_streamerService.Model.Ch1SampleData[_streamerService.Model.Ch1LastSampleSentOut]));
                    _streamerService.Model.Ch1LastSampleSentOut++;
                }
                else
                {
                    _streamerService.Model.Ch1SendDatagram.AddRange(BitConverter.GetBytes(1024));
                }
            }

            sendBytes = _streamerService.Model.Ch1SendDatagram.ToArray();

            _udpCh1Client.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpCh1Port);

            if (_streamerService.Model.Ch1Filended)
            {
                Ch1SendOff();
                _streamerService.Model.Ch1Needtoplay = false;
            }

            stopwatch.Restart();
        }
        private void SetCounterBytesAndCommandCounter()
        {
            _counterBytes = Encoding.ASCII.GetBytes(_streamerService.Model.CommandCounter.ToString());
            if (_streamerService.Model.CommandCounter < 99)
            {
                _streamerService.Model.CommandCounter++;
            }
            else
            {
                _streamerService.Model.CommandCounter = 10;
            }
        }

        private byte[] CreateSendBytes(string encodedCharts)
        {
            byte[] commandBytes = Encoding.ASCII.GetBytes(encodedCharts);
            byte[] sendBytes = new byte[_counterBytes.Length + commandBytes.Length];

            Array.Copy(_counterBytes, 0, sendBytes, 0, _counterBytes.Length);
            Array.Copy(commandBytes, 0, sendBytes, _counterBytes.Length, commandBytes.Length);

            return sendBytes;
        }
    }
}

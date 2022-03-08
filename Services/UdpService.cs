using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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

        public delegate void EnableUI(bool buttonState);
        public event EnableUI SpeakOutEnabled;

        
        public UdpService()
        {
            _streamerService = new StreamerService();
            _iPEndPoint = new IPEndPoint(IPAddress.Parse(Constants.UdpEndPointAddress), Constants.UdpEndPointPort);
            _udpCtrlClient = new UdpClient(Constants.UdpClientPort);
            _udpCh1Client = new UdpClient(Constants.UdpCh1Port);
            _streamerService.NeedToPlayReached += Ch1SendPackage;
            _streamerService.StopItReached += Ch1SendOff;
            _udpCtrlClient.BeginReceive(UdpCtrlReceive, null);
            _udpCh1Client.BeginReceive(OnCH1_UDPReceive, null);
        }

        private void UdpCtrlReceive(IAsyncResult res)
        {
            if (_udpCtrlClient != null)
            {
                Byte[] receivedUdpbytes = _udpCtrlClient.EndReceive(res, ref _iPEndPoint);
                ReceivedString = Encoding.ASCII.GetString(receivedUdpbytes);
            }

            _udpCtrlClient.BeginReceive(new AsyncCallback(UdpCtrlReceive), null);
        }

        private void OnCH1_UDPReceive(IAsyncResult res)
        {
            try
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                if (_udpCh1Client != null)
                {
                    byte[] data = _udpCh1Client.EndReceive(res, ref remote);
                    byte[] ch1_header = new byte[2];
                    Array.Copy(data, 0, ch1_header, 0, ch1_header.Length);
                    if (ch1_header[0] != _streamerService.Model.Ch1StreamStatus)
                    {
                        _streamerService.Model.Ch1StreamStatus = ch1_header[0];

                        if ((byte)(_streamerService.Model.Ch1StreamStatus & 0x07) != _streamerService.Model.Ch1BuffStatus)
                        {
                            _streamerService.Model.Ch1BuffStatus = (byte)(_streamerService.Model.Ch1StreamStatus & 0x07);

                            switch (_streamerService.Model.Ch1BuffStatus)
                            {
                                case (byte)buff_stats.OK:
                                    _streamerService.Model.Ch1Timerpackets = _streamerService.Model.DefaultTimerpackets;
                                    break;
                                case (byte)buff_stats.OL1:
                                    _streamerService.Model.Ch1Timerpackets = 2;
                                    break;
                                case (byte)buff_stats.OL2:
                                    _streamerService.Model.Ch1Timerpackets = 1;
                                    break;
                                case (byte)buff_stats.OL3:
                                    _streamerService.Model.Ch1Timerpackets = 1;
                                    break;
                                case (byte)buff_stats.UL1:
                                    _streamerService.Model.Ch1Timerpackets = 4;
                                    break;
                                case (byte)buff_stats.UL2:
                                    _streamerService.Model.Ch1Timerpackets = 6;
                                    break;
                                case (byte)buff_stats.UL3:
                                    _streamerService.Model.Ch1Timerpackets = 8;
                                    break;
                            }
                        }
                    }
                    _udpCh1Client.BeginReceive(OnCH1_UDPReceive, null);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Itt valami hiba lett: " + ex);
            }
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

            while (!ReceivedString.Contains(Constants.Ack));
            SpeakOutEnabled(true);
        }
        
        private void SpeakOut()
        {
            SetCounterBytesAndCommandCounter();

            var sendBytes = CreateSendBytes(Constants.AllZoneON);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            Form1.Form.UpdateOutputConsole($"Sent {Constants.AllZoneON}");

            while (!ReceivedString.Contains(Constants.Ack)) ;

            SetCounterBytesAndCommandCounter();

            string sendstring = string.Format("{0}{1:D2}{2}", Constants.Volume1Fix, NoiseFilter.Ch1Volume, Constants.CommandEOF);
            sendBytes = CreateSendBytes(sendstring);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            Form1.Form.UpdateOutputConsole($"Sent {Constants.Volume1Fix}");

            while (!ReceivedString.Contains(Constants.Ack)) ;

            SetCounterBytesAndCommandCounter();

            sendstring = string.Format("{0}{1:D2}{2}", Constants.Volume2Fix, NoiseFilter.Ch2Volume, Constants.CommandEOF);
            sendBytes = CreateSendBytes(sendstring);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            Form1.Form.UpdateOutputConsole($"Sent {Constants.Volume2Fix}");

            while (!ReceivedString.Contains(Constants.Ack)) ;

            Form1.Form.UpdateOutputConsole($"Received {Constants.Ack}");

            Thread.Sleep(1000);

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
            if (!_streamerService.Model.Ch1StopIt)
            {
                Ch1Stop();
            }
            else
            {
                SetCounterBytesAndCommandCounter();
                var sendBytes = CreateSendBytes(Constants.AllZoneOFF);
                _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

                while (!ReceivedString.Contains(Constants.Ack)) ;

                SetCounterBytesAndCommandCounter();
                sendBytes = CreateSendBytes(Constants.Stream1Off);
                _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
            }
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
            var sendBytes = CreateSendBytes(Constants.AllZoneOFF);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);

            while (!ReceivedString.Contains(Constants.Ack)) ;

            SetCounterBytesAndCommandCounter();
            sendBytes = CreateSendBytes(Constants.Stream1Off);
            _udpCtrlClient.Send(sendBytes, sendBytes.Length, Constants.UdpEndPointAddress, Constants.UdpEndPointPort);
        }

        private void Ch1Stop()
        {
            _streamerService.Model.Ch1StopIt = true;
            _streamerService.Model.Ch1Needtoplay = true;
        }
        public void Ch1SendPackage(object sender = null, EventArgs e = null)
        {
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

using System;
using System.IO.Ports;
using System.Text;

namespace NoiseAmpControlApp.Services
{
    public class SerialPortService
    {
        private byte[] _noiseReceivedBytes { get; set; }
        private byte[] _ctrlReceivedBytes { get; set; }
        private readonly SerialPort _noiseSerialPort;
        private readonly SerialPort _ctrlSerialPort;

        private bool _gotAmpStart = false;
        public SerialPortService()
        {
            _noiseSerialPort.BaudRate = Constants.NoiseBaudRate;
            _noiseSerialPort.PortName = Constants.NoisePortName;
            _noiseSerialPort.Parity = Parity.None;
            _noiseSerialPort.StopBits = StopBits.One;

            _ctrlSerialPort.BaudRate = Constants.CtrlBaudRate;
            _ctrlSerialPort.PortName = Constants.CtrlPortName;
            _ctrlSerialPort.Parity = Parity.None;
            _ctrlSerialPort.StopBits = StopBits.Two;

            _noiseReceivedBytes = new byte[Constants.NoiseMaxBytes];
            _ctrlReceivedBytes = new byte[Constants.CtrlMaxBytes];
        }

        public void Start()
        {
            _noiseSerialPort.Open();
            _ctrlSerialPort.Open();
            _noiseSerialPort.DiscardInBuffer();
            _ctrlSerialPort.DiscardInBuffer();
            _noiseSerialPort.DataReceived += NoisePortDataReceived;
            _ctrlSerialPort.DataReceived += CtrlPortDataReceived;
            Form1.Form.UpdateOutputConsole("SerialPorts are listening...");
        }

        public void Stop()
        {
            _noiseSerialPort.DataReceived -= NoisePortDataReceived;
            _ctrlSerialPort.DataReceived -= CtrlPortDataReceived;
            _noiseSerialPort.Close();
            _ctrlSerialPort.Close();
            _noiseSerialPort.Dispose();
            _ctrlSerialPort.Dispose();
            Form1.Form.UpdateOutputConsole("SerialPorts are closed...");
        }

        public void SendVolume1Data ()
        {
            string volumeString = string.Format("{0}{1:D2}{2}", Constants.Vol1Fix, NoiseFilter.Ch1Volume, Constants.CommandEOF);
            byte[] serialBytes = Encoding.ASCII.GetBytes(volumeString);

            _ctrlSerialPort.Write(serialBytes, 0, serialBytes.Length);
        }

        public void SendVolume2Data()
        {
            string volumeString = string.Format("{0}{1:D2}{2}", Constants.Vol2Fix, NoiseFilter.Ch2Volume, Constants.CommandEOF);
        }

        private void NoisePortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (byte i = 0; i < Constants.NoiseMaxBytes; i++)
            {
                _noiseReceivedBytes[i] = Convert.ToByte(_noiseSerialPort.ReadByte());
            }
            NoiseFilter.StartCalculation(_noiseReceivedBytes);
        }

        private void CtrlPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for(byte i = 0; i < Constants.CtrlMaxBytes; i++)
            {
                _ctrlReceivedBytes[i] = Convert.ToByte(_ctrlSerialPort.ReadByte());
            }

            if(_ctrlReceivedBytes[6] == 's') //status
            {
                if ((_ctrlReceivedBytes[1] == '3') && (_ctrlReceivedBytes[2] == '1')) //address
                {
                    if ((_ctrlReceivedBytes[9] == '4') && (_ctrlReceivedBytes[10] == '0')) //amp_start
                    {
                        SendVolume1Data();
                    }
                }
                /*if ((_ctrlReceivedBytes[1] == '3') && (_ctrlReceivedBytes[2] == '0'))
                {
                    if ((_ctrlReceivedBytes[9] == '4') && (_ctrlReceivedBytes[10] == '0'))
                    {
                        SendVolume2Data();
                    }
                }*/
            }
        }
    }
}

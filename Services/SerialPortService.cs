using System;
using System.IO.Ports;

namespace NoiseAmpControlApp.Services
{
    public class SerialPortService : SerialPort
    {
        private byte[] _receivedBytes { get; set; }

        public SerialPortService() : base()
        {
            BaudRate = Constants.BaudRate;
            PortName = Constants.PortName;
            Parity = Parity.None;
            StopBits = StopBits.One;

            _receivedBytes = new byte[Constants.MaxBytes];
        }

        public void Start()
        {
            this.Open();

            DiscardInBuffer();

            this.DataReceived += PortDataReceived;
            Form1.Form.UpdateOutputConsole("SerialPort is listening...");
        }

        public void Stop()
        {
            this.DataReceived -= PortDataReceived;
            this.Close();
            this.Dispose();
            Form1.Form.UpdateOutputConsole("SerialPort is closed...");
        }

        private void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (byte i = 0; i < Constants.MaxBytes; i++)
            {
                _receivedBytes[i] = Convert.ToByte(ReadByte());
            }
            NoiseFilter.StartCalculation(_receivedBytes);
        }
    }
}

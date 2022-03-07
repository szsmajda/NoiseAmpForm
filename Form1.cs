using NoiseAmpControlApp.Services;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace NoiseAmpControlApp
{
    public partial class Form1 : Form
    {
        private UdpService _udpService;
        private SerialPortService _serialPortService;
        private int _lineSerialOutputConsole = 0;
        private int _lineCounterOutputConsole = 0;
        public static Form1 Form;

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            _udpService = new UdpService();
            _serialPortService = new SerialPortService();
            Form = this;
            _serialPortService.Start();
        }

        public void UpdateSerialConsole(string consoleText)
        {
            if (InvokeRequired)
            {
                SerialConsoleTextBox.Invoke((MethodInvoker)delegate { SerialConsoleTextBox.Text = $"{_lineSerialOutputConsole}: {consoleText} {Environment.NewLine} {SerialConsoleTextBox.Text}"; });
            }
            _lineSerialOutputConsole++;
        }

        public void UpdateOutputConsole(string consoleText)
        {
            if (InvokeRequired)
            {
                OutputConsoleTextBox.Invoke((MethodInvoker)delegate { OutputConsoleTextBox.Text = $"{_lineCounterOutputConsole}: {consoleText} {Environment.NewLine} {OutputConsoleTextBox.Text}"; });
            }

            OutputConsoleTextBox.Text = $"{_lineCounterOutputConsole}: {consoleText} {Environment.NewLine} {OutputConsoleTextBox.Text}";
            _lineCounterOutputConsole++;
        }

        private void KeepAlive_button_Click(object sender, EventArgs e)
        {
            _udpService.Send(SendTypes.KeepAlive);
        }

        private void SpeakOut_button_Click(object sender, EventArgs e)
        {
            NoiseMeasure_button.Enabled = true;
            SpeakOut_button.Enabled = false;
            _serialPortService.Stop();
            _udpService.Send(SendTypes.SpeakOut);
        }

        private void NoiseMeasure_button_click(object sender, EventArgs e)
        {
            NoiseMeasure_button.Enabled = false;
            SpeakOut_button.Enabled = true;
            _udpService.Send(SendTypes.NoiseMeasure);
            _serialPortService.Start();
        }
    }
}

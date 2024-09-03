using System;
using System.IO.Ports;
using System.Text;
using System.Globalization;

namespace SerialPortReader
{
    public partial class Form1 : Form, IDisposable
    {
        private readonly SerialPort _comPort;

        public Form1()
        {
            InitializeComponent();

            _comPort = new SerialPort
            {
                PortName = "COM4",
                BaudRate = 9600,
                DataBits = 7,
                StopBits = StopBits.One,
                Parity = Parity.Even,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            try
            {
                _comPort.Open();
                label3.Text = "Port Açýldý";
                this.Text = $"PORT AKTÝF: {_comPort.PortName}";
                timer1.Start();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static string CalculateLRC(List<byte> bytes)
        {
            int lrc = 0;
            foreach (var b in bytes)
            {
                lrc = (byte)((lrc + b) & 0xFF);
            }
            return lrc.ToString("X");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _comPort.Close();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                SendAndReceiveData("a01" + "4601" + "R00001", label1);
                SendAndReceiveData("a01" + "4601" + "R00000", label2);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void SendAndReceiveData(string data, Label label)
        {
            var ascii = new List<byte>(System.Text.Encoding.ASCII.GetBytes(data));
            ascii[0] = 0x02;
            var lrc = CalculateLRC(ascii);
            ascii.AddRange(System.Text.Encoding.ASCII.GetBytes(lrc));
            ascii.Add(0x03);
            var sendData = ascii.ToArray();

            _comPort.Write(sendData, 0, sendData.Length);
            var inputData = _comPort.ReadTo(Convert.ToString((Char)3));
            var first = inputData.IndexOf("460") + 3;
            var i = Int32.Parse(inputData.Substring(first, 4), System.Globalization.NumberStyles.HexNumber);
            label.Text = $"R{data.Substring(10, 5)}: {i}";
        }

        private void LogError(Exception ex)
        {
            label3.Text = ex.Message;
        }

        public void Dispose()
        {
            _comPort.Dispose();
        }
    }
}
using System;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace SerialPortReader
{
    public partial class Form1 : Form
    {
        private SerialPort ComPort;

        public Form1()
        {
            InitializeComponent();

            ComPort = new SerialPort();
            ComPort.PortName = "COM4";
            ComPort.BaudRate = 9600;
            ComPort.DataBits = 7;
            ComPort.StopBits = StopBits.One;
            ComPort.Parity = Parity.Even;
            ComPort.ReadTimeout = 500;
            ComPort.WriteTimeout = 500;
            try
            {
                ComPort.Open();
                label3.Text = "Port A��ld�"; // port a��l�nca g�nderece�i mesaj bu k�s�mda yer almaktad�r.
                timer1.Start(); // timer'� ba�latarak interval k�sm�ndan verilerin ne kadar h�zl� ve kadar yava� gidebilece�ini ayarlayabilirsiniz.
            }
            catch (Exception ex)
            {
                label3.Text = ex.Message; // hata mesaj� d�nd��� tektirde label3.Text'e yazd�racakt�r.
            }
        }

        // verileri okuyup hesaplayan byte olarak ��kt� veren k�s�m.
        public static string calculateLRC(List<byte> bytes)
        {
            int LRC = 0;
            for (int i = 0; i < bytes.Count; i++)
            {
                LRC = (byte)((LRC + bytes[i]) & 0xFF);
            }
            return LRC.ToString("X");
        }

        // Uygulama kapat�ld���nda d�necek olan metodlar bu k�s�mda yer almaktad�r.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ComPort.Close();
            }
            catch (Exception ex)
            {
                label3.Text = ex.Message;
            }
        }
        // Timer1 tick zaman� aktifle�meye ba�lad��� gibi �al��acakt�r.
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                string gidenveri = "a01" + "4601" + "R00001";
                List<byte> ascii = new List<byte>();
                ascii.AddRange(System.Text.Encoding.ASCII.GetBytes(gidenveri));
                ascii[0] = 0x02;
                string asciilrc = calculateLRC(ascii);
                ascii.AddRange(System.Text.Encoding.ASCII.GetBytes(asciilrc));
                ascii.Add(0x03);
                byte[] gidenveria = ascii.ToArray();
                ComPort.Write(gidenveria, 0, gidenveria.Length);
                string inputData = ComPort.ReadTo(Convert.ToString((Char)3));
                int first = inputData.IndexOf("460") + 3;
                int i = Int32.Parse(inputData.Substring(first, 4), System.Globalization.NumberStyles.HexNumber);
                label1.Text = "R00001: " + Convert.ToString(i);
                gidenveri = "a01" + "4601" + "R00000";
                ascii = new List<byte>();
                ascii.AddRange(System.Text.Encoding.ASCII.GetBytes(gidenveri));
                ascii[0] = 0x02;
                asciilrc = calculateLRC(ascii);
                ascii.AddRange(System.Text.Encoding.ASCII.GetBytes(asciilrc));
                ascii.Add(0x03);
                gidenveria = ascii.ToArray();
                ComPort.Write(gidenveria, 0, gidenveria.Length);
                inputData = ComPort.ReadTo(Convert.ToString((Char)3));
                first = inputData.IndexOf("460") + 3;
                i = Int32.Parse(inputData.Substring(first, 4), System.Globalization.NumberStyles.HexNumber);
                label2.Text = "R00000: " + Convert.ToString(i);
            }
            catch (Exception ex)
            {
                label3.Text = ex.Message;
            }
        }
    }
}
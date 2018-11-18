using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DMXDemoWPF
{
    public partial class MainWindow : Window
    {
        SerialPort sp;
        byte[] data = new byte[513];
        const int startAddress = 80;
        DispatcherTimer dt;

        public MainWindow()
        {
            InitializeComponent();

            sp = new SerialPort();
            sp.StopBits = StopBits.Two;
            sp.BaudRate = 250000;

            cbxPorts.Items.Add("None");
            foreach(string s in SerialPort.GetPortNames())
                cbxPorts.Items.Add(s);

            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(100);
            dt.Tick += Dt_Tick;      

        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            SendDmxData(data);
        }
        private void sldr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            data[startAddress + 0] = Convert.ToByte(sldrRed.Value);
            data[startAddress + 1] = Convert.ToByte(sldrGreen.Value);
            data[startAddress + 2] = Convert.ToByte(sldrBlue.Value);
            data[startAddress + 3] = 0;
            data[startAddress + 4] = 0;
            data[startAddress + 5] = 0;
        }

        private void cbxPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sp != null)
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                if(cbxPorts.SelectedItem.ToString() != "None")
                {
                    //Poortnaam aanpassen volgens keuze
                    sp.PortName = cbxPorts.SelectedItem.ToString();
                    sp.Open();
                    sldrBlue.IsEnabled = true;
                    sldrGreen.IsEnabled = true;
                    sldrRed.IsEnabled = true;
                }
                else
                {
                    sldrRed.IsEnabled = false;
                    sldrGreen.IsEnabled = false;
                    sldrBlue.IsEnabled = false;
                }
            }
        }
        private void SendDmxData(byte[] pData)
        {
            if(sp != null && sp.IsOpen)
            {
                sp.BreakState = true;
                Thread.Sleep(1);
                sp.BreakState = false;
                Thread.Sleep(1);

                sp.Write(pData, 0, 513);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sp != null) {
                if (sp.IsOpen)
                    SendDmxData(new byte[513]);

                sp.Dispose();
            }
        }
    }
}

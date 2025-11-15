using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace TempGauge
{
    class SerialConnection
    {
        SerialPort port;
        public string? outputData1 { get; set; }
        public string? filteredData1 { get; set; }
        public Decimal outputDouble1 { get; set; }

        string CommPort { get; set; }
        int BaudRate { get; set; }

        static public bool AppState = false;

        public SerialConnection(string commport, int baudrate)
        {
            CommPort = commport;
            BaudRate = baudrate;
            port = new SerialPort(CommPort, BaudRate);
        }

        public void Sense(TextBlock TempTxtblock)
        {
            new Thread(() =>
            {
                port.Open();
                TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
                while (AppState == false)
                {
                    try
                    {
                        if (port.ReadExisting() != null)
                        {
                            outputData1 = port.ReadExisting();
                            try
                            {
                                filteredData1 = Regex.Replace(outputData1, @"[^\d.-]", "");
                                outputDouble1 = Decimal.Parse(filteredData1);
                                if (outputDouble1 < 80.00m && outputDouble1 >= 60.00m)
                                {
                                    UpdateMessage(outputDouble1, port, TempTxtblock);
                                }
                            }
                            catch (FormatException) {/*exception ignored*/}
                        }
                    }
                    catch (System.InvalidOperationException e)
                    {
                        /*ignored exception*/
                    }

                    if (AppState == true)
                    {
                        taskComplete.SetResult(true);
                        Action action3 = () => TempTxtblock.Text = "OFF";
                        port.Close();
                        Dispatcher.UIThread.Post(action3);
                        break;
                    }
                }
            }).Start();
        }

        void UpdateMessage(Decimal inputDouble, SerialPort serialPort, TextBlock textBlock)
        {
            Action action1 = () => textBlock.Text = inputDouble.ToString("F1");
            Dispatcher.UIThread.Post(action1);
        }

        public void SetAppState(bool state)
        {
            AppState = state;
        }

    }
}

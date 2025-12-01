using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TempGauge
{
    class TcpConnection
    {
        //public static float data = 0;

        static Socket socket;
        static Socket accepted;
        static string? strData = null;

        public static byte[]? Buffer { get; set; }

        public static bool AppState = false;
        public void StartServer()
        {       
            try
            {
                //temporary integer value to manage data flow
                int count = 0;
                while(count <= 100)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Bind(new IPEndPoint(IPAddress.Any, 8888));
                    socket.Listen(100);

                    accepted = socket.Accept();
                    Buffer = new byte[accepted.SendBufferSize];
                    int bytesRead = accepted.Receive(Buffer);
                    byte[] formatted = new byte[bytesRead];
                    
                    for(int i = 0; i < bytesRead; i++)
                    {
                        formatted[i] = Buffer[i];
                        count++;
                    }

                    strData = Encoding.ASCII.GetString(formatted);
                    socket.Close();
                    accepted.Close();
                }
            }
            catch (Exception e)
            {
                    /*Exception ignored*/
            }        
        }

        //The Sense function begins a thread which will feed the UI the sensor data
        public void Sense(TextBlock TempTxtblock)
        {
            new Thread(() =>
            {
                TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
                while (AppState == false)
                {
                    try
                    {                        
                        if (strData != null)
                        {
                            UpdateMessage(strData, TempTxtblock);

                        } else if(strData == null)
                        {
                            UpdateMessage("000", TempTxtblock);
                        }
                            
                        //Console.WriteLine("Things are happening");
                    }
                    catch (System.InvalidOperationException e)
                    {
                        /*ignored exception*/
                    }

                    if (AppState == true)
                    {
                        taskComplete.SetResult(true);
                        Action action2 = () => TempTxtblock.Text = "OFF";
                        socket.Close();
                        accepted.Close();
                        Dispatcher.UIThread.Post(action2);
                        break;
                    }
                }
            }).Start();
        }

        void UpdateMessage(string data, TextBlock textBlock)
        {
            Action action1 = () => textBlock.Text = data;      
            Dispatcher.UIThread.Post(action1);        
            //Dispatcher.UIThread.Post(() =>
            //{
            //    textBlock.Text = data;  
            //});
        }

        public void StopSensing()
        {
            AppState = true;
        }
    }
}

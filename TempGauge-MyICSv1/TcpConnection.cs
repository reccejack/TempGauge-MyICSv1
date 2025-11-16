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
    public static float data = 0;
    public static byte[]? bytes = null;

    public static bool AppState = false;
    public void StartServer()
    {
        IPAddress ipAddress = IPAddress.Parse("169.254.102.3");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8888);

        try
        {
            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Socket handler = listener.Accept();
            
            while (true)
            {
                bytes = new byte[10];
                int bytesRec = handler.Receive(bytes);
                data = float.Parse(bytesRec.ToString());
            }
        }
        catch (Exception e)
        {
                //Insert TEXTBOX that provides information on this exception
        }        
    }

    public void Sense(TextBlock TempTxtblock)
    {
        new Thread(() =>
        {
            //StartServer();
            TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
            while (AppState == false)
            {
                try
                {
                    UpdateMessage(data, TempTxtblock);
                    Console.WriteLine("Things are happening");
                }
                catch (System.InvalidOperationException e)
                {
                    /*ignored exception*/
                }

                if (AppState == true)
                {
                    taskComplete.SetResult(true);
                    Action action3 = () => TempTxtblock.Text = "OFF";
                    //port.Close();
                    Dispatcher.UIThread.Post(action3);
                    break;
                }
            }
        }).Start();
    }

    void UpdateMessage(float inputFloat, TextBlock textBlock)
    {
        Action action1 = () => textBlock.Text = inputFloat.ToString("F1");      
        Dispatcher.UIThread.Post(action1);        
    }
}
}

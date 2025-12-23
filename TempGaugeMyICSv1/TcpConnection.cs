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
using TempGaugeMyICS;

namespace TempGaugeMyICS;

class TcpConnection
{
    static Socket socket;
    static Socket accepted;
    static string? strData = null;
   
    public static byte[]? Buffer { get; set; }

    public static bool? AppState = null;
    public void StartServer(TextBlock tempReading)
    {
        AppState = false;

        //Acquiring the IP Address of the host machine
        string hostName = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(hostName);
        IPAddress ipAddress = hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        var endPoint = new IPEndPoint(ipAddress, 8888);


        //IPAddress and IPEndPoint information for Server (DCS or SCADA System)        
        //IPAddress ipAddress = IPAddress.Parse("169.254.102.3");
        //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8888);

        new Thread(() =>
        {
            TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
            while (AppState == false)
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //socket.Bind(localEndPoint);
                    socket.Bind(endPoint);
                    socket.Listen(100);
                    accepted = socket.Accept();
                    //Buffer = new byte[accepted.SendBufferSize];
                    Buffer = new byte[2];
                    int bytesRead = accepted.Receive(Buffer);
                    byte[] formatted = new byte[bytesRead];

                    for (int i = 0; i < bytesRead; i++)
                    {
                        formatted[i] = Buffer[i];
                    }

                    strData = Encoding.ASCII.GetString(formatted);

                    if (strData != null)
                    {
                        UpdateMessage(strData, tempReading);
                                                }
                    else if (strData == null)
                    {
                        UpdateMessage("000", tempReading);
                    }
                    socket.Close();
                    accepted.Close();
                }
                catch (Exception ex)
                {
                    /*Ignore exception*/
                }
            }

            if (AppState == true)
            {
                //taskComplete.SetResult(true);
                Action action2 = () => tempReading.Text = "OFF";
                socket.Close();
                accepted.Close();
                Dispatcher.UIThread.Post(action2);
                taskComplete.SetResult(true);
            }

        }).Start();
    }

    void UpdateMessage(string data, TextBlock textBlock)
    {
        Action action1 = () => textBlock.Text = data;      
        Dispatcher.UIThread.Post(action1);        
    }

    public void StopSensing()
    {
        AppState = true;
    }
}

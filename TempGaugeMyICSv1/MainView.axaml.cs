using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using TempGaugeMyICS;

namespace TempGaugeMyICS;

public partial class MainView : UserControl
{
    TcpConnection tcp1 = new TcpConnection();
    //SerialConnection sensor1 = new SerialConnection("COM3", 9600);

    public MainView()
    {
        InitializeComponent();
    }

    private void Start_Click1(object? sender, RoutedEventArgs e)
    {
        tcp1.StartServer(TempReading);
    }

    private void Start_Click2(object? sender, RoutedEventArgs e)
    {
        tcp1.StopSensing();
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}
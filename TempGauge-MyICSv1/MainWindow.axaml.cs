using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO.Ports;
using TempGauge;

namespace TempGauge_MyICSv1
{
    public partial class MainWindow : Window
    {

        TcpConnection tcp1 = new TcpConnection();
        //SerialConnection sensor1 = new SerialConnection("COM3", 9600);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click1(object? sender, RoutedEventArgs e)
        {
            tcp1.StartServer();
            tcp1.Sense(temperature);
        }
    }
}
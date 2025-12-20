using Avalonia;
using System;
using System.Linq;
using System.Threading;
using TempGaugeMyICS;

namespace TempGaugeMyICS
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static int Main(string[] args)
        {
            var builder = BuildAvaloniaApp();
            if (args.Contains("--drm"))
            {
                SilenceConsole();
                // By default, Avalonia will try to detect output card automatically.
                // But you can specify one, for example "/dev/dri/card1".
                return builder.StartLinuxDrm(args: args, card: null, scaling: 1.0);
            }

            return builder.StartWithClassicDesktopLifetime(args);
        }

        private static void SilenceConsole()
        {
            new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}

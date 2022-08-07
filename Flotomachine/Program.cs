using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using System.Threading;
using Flotomachine.Services;
using Flotomachine.Utility;

namespace Flotomachine;
internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Thread.CurrentThread.Name = "Main";

        if (!Directory.Exists(App.MyDocumentPath))
        {
            Directory.CreateDirectory(App.MyDocumentPath);
        }

        if (!Directory.Exists(App.LogFolderPath))
        {
            Directory.CreateDirectory(App.LogFolderPath);
        }

        AppBuilder app = BuildAvaloniaApp();

        try
        {
            app.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogManager.ErrorLog(e);
        }
        finally
        {
            ModBusService.Exit();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UseReactiveUI().UsePlatformDetect().LogToTrace();
    }
}
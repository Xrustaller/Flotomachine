using Flotomachine.ViewModels;
using System.Collections.Generic;
using System;
using Avalonia.Media.Imaging;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Flotomachine.Services;

public class CameraService
{
    private static Thread _thread;
    private static bool _exit;

    public static event Action<IBitmap> DataCollected;
    public static Timer ThisTimer;
    
    static CameraService()
    {
        ThisTimer = new Timer(5000);
        ThisTimer.AutoReset = true;
        ThisTimer.Elapsed += ThisTimerOnElapsed;
        ThisTimer.Start();
    }

    public static Exception Initialize()
    {
        return null;
        try
        {
            _thread = new Thread(ThisThread) { Name = "CameraService" };
            _thread.Start();
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private static void ThisThread()
    {
        while (!_exit)
        {

        }
    }

    private static void ThisTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        DataCollected?.Invoke(null);
    }

    public static void Exit()
    {
        _exit = true;
    }
}
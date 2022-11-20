using Flotomachine.ViewModels;
using System.Collections.Generic;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Timers;
using System.Threading;
using Flotomachine.Utility;
using Timer = System.Timers.Timer;

namespace Flotomachine.Services;

public class CameraService
{
    private static Thread _thread;
    private static bool _exit;

    public static event Action<IBitmap> DataCollected;
    
    public static Exception Initialize()
    {
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
            if (DataCollected?.GetInvocationList().Length > 0)
            {
                byte[] imageBytes = HttpHelper.DownloadFile(App.Settings.Configuration.Main.CamIp + "/photo");
                //byte[] imageBytes = HttpHelper.DownloadFile("https://hsto.org/files/fde/a29/431/fdea29431e444674a089b5804fbf2634.jpg");
                if (imageBytes == null)
                {
                    DataCollected?.Invoke(null);
                }
                else
                {
                    MemoryStream ms = new(imageBytes);
                    Bitmap bm = new(ms);
                    DataCollected?.Invoke(bm);
                }

                Thread.Sleep(200);
            }
            Thread.Sleep(800);
        }
    }

    //private static void ThisTimerOnElapsed(object sender, ElapsedEventArgs e)
    //{
    //    DataCollected?.Invoke(null);
    //}

    public static void Exit()
    {
        _exit = true;
    }
}
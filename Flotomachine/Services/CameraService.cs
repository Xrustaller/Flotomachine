using Avalonia.Media.Imaging;
using Flotomachine.Utility;
using System;
using System.IO;
using System.Threading;

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
                byte[] imageBytes = HttpHelper.DownloadFile(App.Settings.Configuration.Camera.Url + "/photo");
                //byte[] imageBytes = HttpHelper.DownloadFile("https://hsto.org/files/fde/a29/431/fdea29431e444674a089b5804fbf2634.jpg");
                if (imageBytes == null)
                {
                    DataCollected?.Invoke(null);
                }
                else
                {
                    using MemoryStream ms = new(imageBytes);
                    DataCollected?.Invoke(new Bitmap(ms));
                }

                Thread.Sleep(200);
                continue;
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
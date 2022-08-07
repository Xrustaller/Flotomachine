using System;
using System.IO;

namespace Flotomachine.Utility;

public static class LogManager
{
    public static void ErrorLog(string log, string name = "ErrorLog")
    {
        StreamWriter sw = new(Path.Combine(App.LogFolderPath, $"{name}-{DateTime.Now:hh.mm.ss_dd-MM-yyyy}.txt"));
        sw.WriteLine(log);
        sw.Close();
    }
    public static void ErrorLog(Exception exception, string name = "ErrorLog")
    {
        StreamWriter sw = new(Path.Combine(App.LogFolderPath, $"{name}-{DateTime.Now:hh.mm.ss_dd-MM-yyyy}.txt"));
        sw.WriteLine(exception);
        sw.Close();
    }
}
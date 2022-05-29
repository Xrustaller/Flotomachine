using System;
using System.IO;
using Flotomachine.Utility;

namespace Flotomachine.Services;

public static class GlobalSettingsService
{
    public static Settings Settings { get; private set; }

    public static Exception Load()
    {
        string path = Path.Join(Directory.GetCurrentDirectory(), "Settings");
        if (JsonProvider.TryReadJson(path, out Settings result, out Exception exception))
        {
            Settings = result;
        }
        return exception;
    }

    public static void Save()
    {
        string path = Path.Join(Directory.GetCurrentDirectory(), "Settings");
        JsonProvider.WriteJson(path, Settings);
    }
}
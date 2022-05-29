using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Flotomachine.Utility;

public static class JsonProvider
{
    // Чтение
    public static T ReadJson<T>(string path)
    {
        byte[] bt = File.ReadAllBytes(path);
        string str = Encoding.Default.GetString(bt);
        return JsonConvert.DeserializeObject<T>(str);
    }

    public static Exception ReadJson<T>(string path, out T result)
    {
        try
        {
            byte[] bt = File.ReadAllBytes(path);
            string str = Encoding.Default.GetString(bt);
            result = JsonConvert.DeserializeObject<T>(str);
        }
        catch (Exception e)
        {
            result = default;
            return e;
        }
        return null;
    }

    public static bool TryReadJson<T>(string path, out T result)
    {
        try
        {
            byte[] bt = File.ReadAllBytes(path);
            string str = Encoding.Default.GetString(bt);
            result = JsonConvert.DeserializeObject<T>(str)!;
        }
        catch
        {
            result = default!;
            return false;
        }
        return true;
    }

    public static bool TryReadJson<T>(string path, out T result, out Exception exception)
    {
        try
        {
            byte[] bt = File.ReadAllBytes(path);
            string str = Encoding.Default.GetString(bt);
            result = JsonConvert.DeserializeObject<T>(str);
        }
        catch (Exception e)
        {
            result = default;
            exception = e;
            return false;
        }
        exception = null!;
        return true;
    }
    // Запись
    public static Exception WriteJson<T>(string path, T source, Formatting format = Formatting.Indented)
    {
        try
        {
            string str = JsonConvert.SerializeObject(source, format);
            byte[] bt = Encoding.Default.GetBytes(str);
            File.WriteAllBytes(path, bt);
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
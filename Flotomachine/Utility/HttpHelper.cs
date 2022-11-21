using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flotomachine.Utility;

public static class HttpHelper
{
    public static readonly HttpClient HttpClient = new();

    public static T GetJson<T>(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        var res = HttpClient.GetStringAsync(uriResult);
        res.Wait();
        return JsonConvert.DeserializeObject<T>(res.Result);
    }

    public static async Task<string> GetStringAsync(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        return await HttpClient.GetStringAsync(uriResult);
    }

    public static void FileDownloadAndSave(string uri, string outputPath)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        if (File.Exists(outputPath))
        {
            return;
            throw new FileNotFoundException("File already exists.", nameof(outputPath));
        }

        Task<byte[]> fileBytes = HttpClient.GetByteArrayAsync(uriResult);
        fileBytes.Wait();
        File.WriteAllBytes(outputPath, fileBytes.Result);
    }

    public static Task<byte[]> DownloadFileAsync(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        return HttpClient.GetByteArrayAsync(uriResult);
    }

    public static byte[] DownloadFile(string uri)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        try
        {
            Task<byte[]> fileBytes = HttpClient.GetByteArrayAsync(uriResult);
            fileBytes.Wait();
            return fileBytes.Result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async void DownloadFileAsync(string uri, string outputPath)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        if (File.Exists(outputPath))
        {
            return;
            throw new FileNotFoundException("File not found.", nameof(outputPath));
        }

        byte[] fileBytes = await HttpClient.GetByteArrayAsync(uriResult);
        await File.WriteAllBytesAsync(outputPath, fileBytes);
    }

}
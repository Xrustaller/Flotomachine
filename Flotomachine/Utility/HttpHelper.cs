﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flotomachine.Utility;

public static class HttpHelper
{
    private static readonly HttpClient HttpClient = new();

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

    public static void DownloadFile(string uri, string outputPath)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        if (File.Exists(outputPath))
        {
            return;
            //throw new FileNotFoundException("File already exists.", nameof(outputPath));
        }

        var fileBytes = HttpClient.GetByteArrayAsync(uriResult);
        fileBytes.Wait();
        File.WriteAllBytes(outputPath, fileBytes.Result);
    }

    public static async void DownloadFileAsync(string uri, string outputPath)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri uriResult))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        if (!File.Exists(outputPath))
        {
            throw new FileNotFoundException("File not found.", nameof(outputPath));
        }

        byte[] fileBytes = await HttpClient.GetByteArrayAsync(uriResult);
        await File.WriteAllBytesAsync(outputPath, fileBytes);
    }
}
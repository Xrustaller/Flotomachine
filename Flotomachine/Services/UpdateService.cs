using Flotomachine.Utility;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Flotomachine.Services;

public static class UpdateService
{
    public static bool NeedUpdate { get; private set; } = false;

    [CanBeNull]
    public static Version NewVersion { get; private set; }

    public static Exception Initialize(Settings settingsConfiguration)
    {
        try
        {
            using HttpClient client = new();
            Task<string> e = client.GetStringAsync(settingsConfiguration.Main.UpdateJsonUrl);
            e.Wait();

            Version gitVersion = JsonConvert.DeserializeObject<UpdateFile>(e.Result)?.Version;
            if (gitVersion == null)
            {
                return new Exception("Git file empty");
            }

            NewVersion = gitVersion;

            if (Assembly.GetEntryAssembly()?.GetName().Version >= gitVersion)
            {
                return null;
            }

            NeedUpdate = true;
            return null;

        }
        catch (Exception e)
        {
            return e;
        }
    }

}
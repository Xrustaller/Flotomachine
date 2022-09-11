using Flotomachine.Utility;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Flotomachine.Services;

public static class UpdateService
{
    public static bool NeedUpdate { get; private set; } = false;

    [CanBeNull]
    public static Version NewVersion { get; private set; }

    private static string _fileUrl = null;

    public static Exception Initialize(Settings settingsConfiguration)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            try
            {
                UpdateFile json = HttpHelper.GetJson<UpdateFile>(settingsConfiguration.Main.UpdateJsonUrl);
                Version gitVersion = json.Version;
                if (gitVersion == null)
                {
                    return new Exception("Git file empty");
                }

                NewVersion = gitVersion;

                if (Assembly.GetEntryAssembly()?.GetName().Version >= gitVersion)
                {
#if !RP_DEBUG
                    return null;
#endif
                }

                NeedUpdate = true;

                _fileUrl = GetLatestDebFileUrl(settingsConfiguration.Main.UpdateFileUrl, gitVersion);
                return null;

            }
            catch (Exception e)
            {
                return e;
            }
        }
        
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                UpdateFile json = HttpHelper.GetJson<UpdateFile>(settingsConfiguration.Main.UpdateJsonUrl);
                Version gitVersion = json.Version;
                if (gitVersion == null)
                {
                    return new Exception("Git file empty");
                }

                NewVersion = gitVersion;

                if (Assembly.GetEntryAssembly()?.GetName().Version >= gitVersion)
                {
#if !DEBUG
                    return null;
#endif
                }

                NeedUpdate = true;

                _fileUrl = GetLatestDebFileUrl(settingsConfiguration.Main.UpdateFileUrl, gitVersion);
                return null;

            }
            catch (Exception e)
            {
                return e;
            }
        }

        return null;
    }

    public static string DownloadLatestReleaseFile()
    {
        string name = Path.Join(App.DownloadPath, GetLatestDebFileName(NewVersion));
        HttpHelper.DownloadFile(_fileUrl, name);
        return name;
    }

    private static string GetLatestDebFileUrl(string gitUrl, Version ver) => gitUrl + GetLatestDebFileName(ver);

    private static string GetLatestDebFileName(Version ver) => $"Flotomachine.{ver.ToShortString()}.linux-arm.deb";
}


[Serializable]
public class UpdateFile
{
    public Version Version { get; set; }

    public UpdateFile()
    {

    }
}
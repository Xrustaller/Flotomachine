using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Flotomachine.Utility;
using JetBrains.Annotations;

namespace Flotomachine.Services;

public static class UpdateService
{
	public static bool NeedUpdate { get; private set; } = false;

	[CanBeNull]
	public static Version NewVersion { get; private set; } = new(0, 0);

	private static string _fileUrl = null;

	public static Exception CheckUpdates(bool force = false) => CheckUpdates(App.Settings.Configuration, force);
	public static Exception CheckUpdates(Settings settingsConfiguration, bool force = false)
	{
		try
		{
			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			//{

			//}

			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			//{

			//}

			UpdateFile json = HttpHelper.GetJson<UpdateFile>(settingsConfiguration.Main.UpdateJsonUrl);
			Version gitVersion = json.Version;
			if (gitVersion == null)
			{
				return new Exception("Git file empty");
			}

			NewVersion = gitVersion;

			if (Assembly.GetEntryAssembly()?.GetName().Version >= gitVersion)
			{
#if !DEBUG && !RP_DEBUG
                return null;
#endif
			}

			NeedUpdate = true;

			_fileUrl = GetLatestDebFileUrl(settingsConfiguration.Main.UpdateFileUrl, gitVersion);
		}
		catch (Exception e)
		{
			return e;
		}

		return null;
	}

	public static string DownloadLatestReleaseFile()
	{
		string name = Path.Join(App.DownloadPath, GetLatestDebFileName(NewVersion));
		if (_fileUrl == null)
		{
			return null;
		}
		HttpHelper.DownloadFileAsync(_fileUrl, name);
		return name;
	}

	public static void InstallDebFile(string path)
	{
#if DEBUG && !RP_DEBUG
		Process proc = new();
		proc.StartInfo.FileName = "explorer";
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.Start();
#elif !DEBUG
        Process proc = new();
        proc.StartInfo.FileName = "bash";
        proc.StartInfo.Arguments = $"-c \"sudo dpkg -i {path}; Flotomachine\"";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
#endif

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
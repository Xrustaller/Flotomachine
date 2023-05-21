using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flotomachine.Utility;
using Newtonsoft.Json;
using Semver;

namespace Flotomachine.Services;

public class GitHubService
{
	public static GitHubService Instance = new GitHubService();

	private const string ReleasesEndpoint = "https://api.github.com/repos/Xrustaller/Flotomachine/releases";
	private readonly HttpClient _httpClient = new();
	private readonly WebClient _webClient = new();

	public Dictionary<string, SemVersion> Releases { get; private set; }

	public bool NeedUpdate { get; private set; } = false;
	public SemVersion NewVersion { get; private set; } = new(0, 0);

	public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
	public event EventHandler<DownloadDataCompletedEventArgs> DownloadComplete;

	private GitHubService()
	{
		_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Flotomachine", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
		_webClient.Headers.Add("User-Agent", $"Flotomachine-{Assembly.GetExecutingAssembly().GetName().Version}");
		_webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
		_webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
	}

	private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) => DownloadComplete(sender, e);
	private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) => DownloadProgressChanged(sender, e);

	public async Task<bool> DownloadVersionAsync(string versionKey, string path = null!)
	{
		List<string> assetUrls = await GetAssetUrlsAsync(versionKey);
		Console.WriteLine(assetUrls);
		foreach (var assetUrl in assetUrls)
		{
			await GetAssetsAsync(assetUrl, path);
		}

		return true;
	}

	public async Task<Dictionary<string, SemVersion>> GetReleasesAsync()
	{
		string? pageNumber = "1";
		Dictionary<string, SemVersion>? releases = new();
		while (pageNumber != null)
		{
			HttpResponseMessage response = await _httpClient.GetAsync(new Uri(ReleasesEndpoint + "?page=" + pageNumber));
			string contentJson = await response.Content.ReadAsStringAsync();
			VerifyGitHubApiResponse(response.StatusCode, contentJson);
			dynamic releasesJson = JsonConvert.DeserializeObject<dynamic>(contentJson);
			foreach (dynamic releaseJson in releasesJson)
			{
				bool preRelease = releaseJson["prerelease"];
				//if (!_settings.IncludePreRelease && preRelease) continue;
				dynamic releaseId = releaseJson["id"].ToString();
				try
				{
					string tagName = releaseJson["tag_name"].ToString();
					string version = CleanVersion(tagName);
					SemVersion semVersion = SemVersion.Parse(version, SemVersionStyles.Any);
					releases.Add(releaseId, semVersion);
				}
				catch (Exception)
				{
					// ignored
				}
			}

			pageNumber = GetNextPageNumber(response.Headers);
		}

		return releases;
	}

	private void VerifyGitHubApiResponse(HttpStatusCode statusCode, string content)
	{
		switch (statusCode)
		{
			case HttpStatusCode.Forbidden when content.Contains("API rate limit exceeded"):
				throw new Exception("GitHub API rate limit exceeded.");
			case HttpStatusCode.NotFound when content.Contains("Not Found"):
				throw new Exception("GitHub Repo not found.");
			default:
			{
				if (statusCode != HttpStatusCode.OK)
					throw new Exception("GitHub API call failed.");
				break;
			}
		}
	}

	private string? GetNextPageNumber(HttpHeaders headers)
	{
		string linkHeader;
		try
		{
			linkHeader = headers.GetValues("Link").FirstOrDefault();
		}
		catch (Exception)
		{
			return null;
		}

		if (string.IsNullOrWhiteSpace(linkHeader))
		{
			return null;
		}

		string[] links = linkHeader.Split(',');
		return !links.Any()
			? null
			: (
				from link in links
				where link.Contains(@"rel=""next""")
				select Regex.Match(link, "(?<=page=)(.*)(?=>;)").Value).FirstOrDefault();
	}

	private string CleanVersion(string version)
	{
		var cleanedVersion = version;
		cleanedVersion = cleanedVersion.StartsWith("v") ? cleanedVersion.Substring(1) : cleanedVersion;
		var buildDelimiterIndex = cleanedVersion.LastIndexOf("+", StringComparison.Ordinal);
		cleanedVersion = buildDelimiterIndex > 0
			? cleanedVersion.Substring(0, buildDelimiterIndex)
			: cleanedVersion;
		return cleanedVersion;
	}

	private async Task GetAssetsAsync(string assetUrl, string path)
	{
		using MD5 md5 = MD5.Create();
		await using FileStream fs = new(path, FileMode.OpenOrCreate);
		byte[] data = await _webClient.DownloadDataTaskAsync(assetUrl);

		await fs.WriteAsync(data, 0, data.Length);
		//var response = await _httpClient.GetAsync(assetUrl);
		//Stream release = await response.Content.ReadAsStreamAsync();
		//await release.CopyToAsync(fs);
	}

	private async Task<List<string>> GetAssetUrlsAsync(string releaseId)
	{
		List<string> assets = new();
		string assetsEndpoint = ReleasesEndpoint + "/" + releaseId + "/assets";
		HttpResponseMessage response = await _httpClient.GetAsync(new Uri(assetsEndpoint));
		string contentJson = await response.Content.ReadAsStringAsync();
		VerifyGitHubApiResponse(response.StatusCode, contentJson);
		dynamic assetsJson = JsonConvert.DeserializeObject<dynamic>(contentJson);
		foreach (dynamic assetJson in assetsJson)
		{
			assets.Add(assetJson["browser_download_url"].ToString());
		}


		return assets;
	}

	private async Task<bool> VerifyGithubMd5Async(string provided, Stream content)
	{
		using MD5 md5 = MD5.Create();
		return BitConverter.ToString(await md5.ComputeHashAsync(content)).Replace("-", "").ToLowerInvariant() == provided.ToLowerInvariant();
	}

	public async Task<bool> CheckUpdates(bool force = false)
	{
#if DEBUG || RP_DEBUG
		force = true;
#endif
		try
		{
			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			//{

			//}

			//if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			//{

			//}

			Releases = await Instance.GetReleasesAsync();
			KeyValuePair<string, SemVersion> lastRelease = Releases.First();

			NewVersion = lastRelease.Value;

			if (force)
			{
				NeedUpdate = true;
			}

			if (SemVersion.FromVersion(Assembly.GetEntryAssembly()?.GetName().Version) < lastRelease.Value)
			{
				NeedUpdate = true;
			}
		}
		catch (Exception)
		{
			// ignored
		}

		return NeedUpdate;
	}

	private static string GetLatestDebFileName(Version ver) => $"Flotomachine.{ver.ToShortString()}.linux-arm.deb";

}
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using Flotomachine.Services;
using Flotomachine.Utility;
using ReactiveUI;
using Semver;

namespace Flotomachine.ViewModels;

public class UpdateWindowViewModel : ViewModelBase
{
	private readonly Window _mainWindow;
	private readonly Window _updateWindow;

	private InfoViewModel _text = new("Текущая: v" + Assembly.GetEntryAssembly()?.GetName().Version.ToShortString() + "\n" + "Новая версия: v" + GitHubService.Instance.NewVersion, "#FFFFFF");
	private bool _buttonsEnable = true;

	private int _downloadProgressBarValue;
	private bool _downloadProgressBarVisible = false;

	public InfoViewModel Text
	{
		get => _text;
		set => this.RaiseAndSetIfChanged(ref _text, value);
	}

	public bool ButtonsEnable
	{
		get => _buttonsEnable;
		set => this.RaiseAndSetIfChanged(ref _buttonsEnable, value);
	}
	public int DownloadProgressBarValue
	{
		get => _downloadProgressBarValue;
		set => this.RaiseAndSetIfChanged(ref _downloadProgressBarValue, value);
	}

	public bool DownloadProgressBarVisible
	{
		get => _downloadProgressBarVisible;
		set => this.RaiseAndSetIfChanged(ref _downloadProgressBarVisible, value);
	}

	public ICommand HoldOverClick { get; }
	public ICommand DownloadClick { get; }

	public UpdateWindowViewModel()
	{

	}

	public UpdateWindowViewModel(Window window, Window thisWindow)
	{
		_mainWindow = window;
		_updateWindow = thisWindow;
		HoldOverClick = new DelegateCommand(HoldOver);
		DownloadClick = new DelegateCommand(Download);
	}

	private async void Download(object obj)
	{
		ButtonsEnable = false;
		DownloadProgressBarVisible = true;
		DownloadProgressBarValue = 0;

		GitHubService.Instance.DownloadProgressChanged += InstanceOnDownloadProgressChanged;
		GitHubService.Instance.DownloadComplete += InstanceOnDownloadComplete;

		KeyValuePair<string, SemVersion> lastRelease = GitHubService.Instance.Releases.First();

		string temp = System.IO.Path.GetTempFileName();
		await GitHubService.Instance.DownloadVersionAsync(lastRelease.Key, temp);
		if (string.IsNullOrEmpty(temp))
		{
			Text.Text = "Ошибка получения файла";
			return;
		}
		Text.Text = "Запуск обновления\nОбновление длится 1-5 мин.\nПрограмма будет закрыта и перезапущена\nНе выключайте компьютер и не запускайте программу заново";
		await Task.Run(() =>
		{
			Thread.Sleep(10000);
#if DEBUG && !RP_DEBUG
			Process proc = new();
			proc.StartInfo.FileName = "explorer";
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.Start();
#elif !DEBUG
	        Process proc = new();
	        proc.StartInfo.FileName = "bash";
	        proc.StartInfo.Arguments = $"-c \"sudo dpkg -i {temp}; Flotomachine\"";
	        proc.StartInfo.UseShellExecute = false;
	        proc.StartInfo.RedirectStandardOutput = true;
	        proc.Start();
#endif
			Dispatcher.UIThread.InvokeAsync(() => _mainWindow.Close());
		});
	}

	private void InstanceOnDownloadComplete(object? sender, DownloadDataCompletedEventArgs e)
	{
		GitHubService.Instance.DownloadProgressChanged -= InstanceOnDownloadProgressChanged;
		GitHubService.Instance.DownloadComplete -= InstanceOnDownloadComplete;
	}

	private void InstanceOnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
	{
		Dispatcher.UIThread.InvokeAsync(() => DownloadProgressBarValue = e.ProgressPercentage);
	}

	private void HoldOver(object obj)
	{
		_updateWindow.Close();
	}

	public override void OnDestroy()
	{

	}
}
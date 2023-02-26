using System.Windows.Input;
using Avalonia.Media;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class RfidAdminSettingsViewModel : ViewModelBase
{
	private readonly MainWindowViewModel _mainWindowViewModel;

	private int _busId;
	private int _lineId;
	private int _clockFrequencySpi;

	private string _info;
	private IBrush _colorInfo;

	public int BusId
	{
		get => _busId;
		set => this.RaiseAndSetIfChanged(ref _busId, value);
	}

	public int LineId
	{
		get => _lineId;
		set => this.RaiseAndSetIfChanged(ref _lineId, value);
	}

	public int ClockFrequencySpi
	{
		get => _clockFrequencySpi;
		set => this.RaiseAndSetIfChanged(ref _clockFrequencySpi, value);
	}

	public string Info
	{
		get => _info;
		set => this.RaiseAndSetIfChanged(ref _info, value);
	}

	public IBrush ColorInfo
	{
		get => _colorInfo;
		set => this.RaiseAndSetIfChanged(ref _colorInfo, value);
	}

	public ICommand SaveClick { get; }

	public RfidAdminSettingsViewModel()
	{

	}

	public RfidAdminSettingsViewModel(MainWindowViewModel mainWindowViewModel)
	{
		_mainWindowViewModel = mainWindowViewModel;

		SaveClick = new DelegateCommand(SaveSettings);

		BusId = App.Settings.Configuration.RfId.BusId;
		LineId = App.Settings.Configuration.RfId.LineId;
		ClockFrequencySpi = App.Settings.Configuration.RfId.ClockFrequencySpi;
	}

	private void SaveSettings(object obj)
	{
		App.Settings.Configuration.RfId.BusId = BusId;
		App.Settings.Configuration.RfId.LineId = LineId;
		App.Settings.Configuration.RfId.ClockFrequencySpi = ClockFrequencySpi;

		App.Settings.SaveConfig();

		Info = "Успешно";
		ColorInfo = Brush.Parse("#10FF10");
	}
}
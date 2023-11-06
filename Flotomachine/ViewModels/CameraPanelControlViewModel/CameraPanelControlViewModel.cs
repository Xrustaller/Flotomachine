using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Flotomachine.Services;
using ReactiveUI;

namespace Flotomachine.ViewModels;
public class CameraPanelControlViewModel : ViewModelBase
{
	private readonly MainWindowViewModel _mainWindowViewModel;
	private Bitmap _mainImage;

	public Bitmap MainImage
	{
		get => _mainImage;
		set => this.RaiseAndSetIfChanged(ref _mainImage, value);
	}

	public CameraPanelControlViewModel()
	{

	}

	public CameraPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
	{
		_mainWindowViewModel = mainWindowViewModel;
		CameraService.DataCollected += CameraServiceOnDataCollected;
	}

	private async void CameraServiceOnDataCollected(Bitmap bitmap)
	{
		await Dispatcher.UIThread.InvokeAsync(() =>
		{
			MainImage = bitmap;
		});
	}

	public override void OnDestroy()
	{
		CameraService.DataCollected -= CameraServiceOnDataCollected;
	}
}

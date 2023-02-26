using Avalonia.Media;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class InfoViewModel : ViewModelBase
{
	private string _text;
	private IBrush _color;

	public string Text
	{
		get => _text;
		set => this.RaiseAndSetIfChanged(ref _text, value);
	}

	public IBrush Color
	{
		get => _color;
		set => this.RaiseAndSetIfChanged(ref _color, value);
	}

	public InfoViewModel()
	{

	}

	public InfoViewModel(string text, IBrush color)
	{
		Text = text;
		Color = color;
	}
	public InfoViewModel(string text, string color)
	{
		Text = text;
		Color = Brush.Parse(color);
	}
}


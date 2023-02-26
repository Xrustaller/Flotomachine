using ReactiveUI;

namespace Flotomachine.ViewModels;

public class HomeModuleDataViewModel : ViewModelBase
{
	private string _name = "NAME";
	private string _value = "NULL";
	private string _valueName = "NULL";

	public HomeModuleDataViewModel()
	{

	}

	public HomeModuleDataViewModel(string name, string value, string valueName)
	{
		Name = name;
		Value = value;
		ValueName = valueName;
	}

	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}

	public string Value
	{
		get => _value;
		set => this.RaiseAndSetIfChanged(ref _value, value);
	}

	public string ValueName
	{
		get => _valueName;
		set => this.RaiseAndSetIfChanged(ref _valueName, value);
	}
}
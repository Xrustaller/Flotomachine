using ReactiveUI;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class LoginPassViewModel : ViewModelBase
{
    private string _login;
    private string _passOne;
    private string _passTwo;
    private InfoViewModel _info;

    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public string PassOne
    {
        get => _passOne;
        set => this.RaiseAndSetIfChanged(ref _passOne, value);
    }

    public string PassTwo
    {
        get => _passTwo;
        set => this.RaiseAndSetIfChanged(ref _passTwo, value);
    }

    public InfoViewModel Info
    {
        get => _info;
        set => this.RaiseAndSetIfChanged(ref _info, value);
    }

    public ICommand ButtonClick { get; set; }
}
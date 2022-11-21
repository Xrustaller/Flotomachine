using Flotomachine.Services;
using Flotomachine.Utility;
using Flotomachine.View;
using ReactiveUI;
using System.Reflection;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class SettingsPanelControlViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _programInfo = $"Флотомашина v{Assembly.GetEntryAssembly()?.GetName().Version.ToShortString()}";
    private InfoViewModel _updateInfo;
    private string _updateButtonText;
    private InfoViewModel _userInfo;
    private string _userName;
    private string _userLogin;
    private string _userPass;

    private LoginPassViewModel _changePasswordViewModel;
    private AddDelUserCardViewModel _addDelUserCardViewModel;

    public string ProgramInfo
    {
        get => _programInfo;
        set => this.RaiseAndSetIfChanged(ref _programInfo, value);
    }

    public InfoViewModel UpdateInfo
    {
        get => _updateInfo;
        set => this.RaiseAndSetIfChanged(ref _updateInfo, value);
    }

    public string UpdateButtonText
    {
        get => _updateButtonText;
        set => this.RaiseAndSetIfChanged(ref _updateButtonText, value);
    }

    public InfoViewModel UserInfo
    {
        get => _userInfo;
        set => this.RaiseAndSetIfChanged(ref _userInfo, value);
    }

    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    public string UserLogin
    {
        get => _userLogin;
        set => this.RaiseAndSetIfChanged(ref _userLogin, value);
    }

    public string UserPass
    {
        get => _userPass;
        set => this.RaiseAndSetIfChanged(ref _userPass, value);
    }

    public LoginPassViewModel ChangePasswordViewModel
    {
        get => _changePasswordViewModel;
        set => this.RaiseAndSetIfChanged(ref _changePasswordViewModel, value);
    }

    public AddDelUserCardViewModel AddDelUserCardViewModel
    {
        get => _addDelUserCardViewModel;
        set => this.RaiseAndSetIfChanged(ref _addDelUserCardViewModel, value);
    }

    public ICommand SaveUserClick { get; }
    public ICommand UpdateClick { get; }

    public SettingsPanelControlViewModel()
    {
        if (UpdateService.NeedUpdate)
        {
            UpdateButtonText = "Загрузить и обновить";
            UpdateInfo = new InfoViewModel($"Вышла новая версия: v{UpdateService.NewVersion}", "#FFFF10");
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
        }
    }

    public SettingsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        ChangePasswordViewModel = new LoginPassViewModel()
        {
            ButtonClick = new DelegateCommand(ChangePass)
        };
        AddDelUserCardViewModel = new AddDelUserCardViewModel()
        {
            AddUserCard = new DelegateCommand(AddCard),
            DelUserCard = new DelegateCommand(DeleteCard)
        };

        SaveUserClick = new DelegateCommand(SaveUser);
        UpdateClick = new DelegateCommand(Update);

        foreach (CardId item in DataBaseService.GetCardIds(_mainWindowViewModel.CurrentUser))
        {
            AddDelUserCardViewModel.CardsList.Add(new CardIdListBox(item));
        }

        if (UpdateService.NeedUpdate)
        {
            UpdateButtonText = "Загрузить и обновить";
            UpdateInfo = new InfoViewModel($"Вышла новая версия: v{UpdateService.NewVersion}", "#FFFF10");
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
        }

        UserName = _mainWindowViewModel.CurrentUser.Name ?? "";
        UserLogin = _mainWindowViewModel.CurrentUser.Username ?? "";
    }

    private void SaveUser(object obj)
    {
        if (_mainWindowViewModel.CurrentUser.Username == UserLogin && _mainWindowViewModel.CurrentUser.Name == UserName)
        {
            UserInfo = new InfoViewModel("Нечего изменять", "#10FF10");
            return;
        }

        if (_mainWindowViewModel.CurrentUser.Username != UserLogin)
        {
            if (!_mainWindowViewModel.CurrentUser.CheckPass(UserPass))
            {
                ChangePasswordViewModel.Info = new InfoViewModel("Неверный пароль", "#FF1010");
                return;
            }

            _mainWindowViewModel.CurrentUser.Username = UserLogin;
            _mainWindowViewModel.CurrentUser.PassHash = User.GenerateHash(UserLogin, UserPass);

            UserPass = "";
        }

        _mainWindowViewModel.CurrentUser.Name = UserName;

        DataBaseService.GetAndSet(p =>
        {
            p.Users.Update(_mainWindowViewModel.CurrentUser);
        });

        UserInfo = new InfoViewModel("Успешно", "#10FF10");
    }

    private void Update(object obj)
    {
        if (UpdateService.NeedUpdate)
        {
            _mainWindowViewModel.CheckUpdate();
        }
        else
        {
            UpdateButtonText = "Проверить обновления";
            UpdateInfo = new InfoViewModel("Обновление не требуется", "#10FF10");
            UpdateService.CheckUpdates(true);
        }
    }

    private void ChangePass(object parameter)
    {
        if (string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassOne) || string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassTwo))
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Заполните все поля", "#FF1010");
            return;
        }

        if (ChangePasswordViewModel.PassOne.Length < 4)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Пароль < 4 символов", "#FF1010");
            return;
        }

        if (ChangePasswordViewModel.PassOne != ChangePasswordViewModel.PassTwo)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Разные пароли", "#FF1010");
            return;
        }

        _mainWindowViewModel.CurrentUser.PassHash = User.GenerateHash(_mainWindowViewModel.CurrentUser.Username, ChangePasswordViewModel.PassOne);
        DataBaseService.GetAndSet(p =>
        {
            p.Users.Update(_mainWindowViewModel.CurrentUser);
        });

        ChangePasswordViewModel.Login = "";
        ChangePasswordViewModel.PassOne = "";
        ChangePasswordViewModel.PassTwo = "";
        ChangePasswordViewModel.Info = new InfoViewModel("Успешно", "#10FF10");
    }

    private async void AddCard(object parameter)
    {
        ReadCardWindow readCard = new(App.Settings.Configuration.RfId.BusId, App.Settings.Configuration.RfId.LineId, App.Settings.Configuration.RfId.ClockFrequencySpi);
        byte[] result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Считывание отменено", "#FF1010");
            return;
        }

        if (_mainWindowViewModel.CurrentUser == null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Пользователь не найден", "#FF1010");
            return;
        }

        CardId card = DataBaseService.GetCard(result);

        if (card != null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Уже зарегистрирована", "#FF1010");
            return;
        }

        DataBaseService.CreateCard(_mainWindowViewModel.CurrentUser, result);

        AddDelUserCardViewModel.CardsList.Clear();
        foreach (CardId item in DataBaseService.GetCardIds(_mainWindowViewModel.CurrentUser))
        {
            AddDelUserCardViewModel.CardsList.Add(new CardIdListBox(item));
        }

        AddDelUserCardViewModel.Info = new InfoViewModel("Успешно добавлена", "#10FF10");
    }

    private async void DeleteCard(object parameter)
    {
        ReadCardWindow readCard = new(App.Settings.Configuration.RfId.BusId, App.Settings.Configuration.RfId.LineId, App.Settings.Configuration.RfId.ClockFrequencySpi);
        var result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Считывание отменено", "#FF1010");
            return;
        }

        if (_mainWindowViewModel.CurrentUser == null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Пользователь не найден", "#FF1010");
            return;
        }

        CardId card = DataBaseService.GetCard(result);
        if (card == null)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Не найдено", "#FF1010");
            return;
        }

        if (card.UserId != _mainWindowViewModel.CurrentUser.Id)
        {
            AddDelUserCardViewModel.Info = new InfoViewModel("Не принадлежит", "#FF1010");
            return;
        }

        DataBaseService.DeleteCard(card);

        AddDelUserCardViewModel.CardsList.Clear();
        foreach (CardId item in DataBaseService.GetCardIds(_mainWindowViewModel.CurrentUser))
        {
            AddDelUserCardViewModel.CardsList.Add(new CardIdListBox(item));
        }

        AddDelUserCardViewModel.Info = new InfoViewModel("Успешно удалена", "#10FF10");
    }
}
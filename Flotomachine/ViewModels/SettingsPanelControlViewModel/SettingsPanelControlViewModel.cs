using Flotomachine.Services;
using Flotomachine.View;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class SettingsPanelControlViewModel : ViewModelBase
{
    private LoginPassViewModel _changePasswordViewModel;
    private AddDelUserCardViewModel _addDelUserCardViewModel;

    private readonly MainWindowViewModel _mainWindowViewModel;

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

    public SettingsPanelControlViewModel()
    {

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

        foreach (CardId item in DataBaseService.GetCardIds(_mainWindowViewModel.CurrentUser))
        {
            AddDelUserCardViewModel.CardsList.Add(new CardIdListBox(item));
        }
    }

    private void ChangePass(object parameter)
    {
        if (string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassOne) || string.IsNullOrWhiteSpace(ChangePasswordViewModel.PassTwo))
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Заполните все поля", "#FF1010");
            return;
        }

        if (ChangePasswordViewModel.PassOne != ChangePasswordViewModel.PassTwo)
        {
            ChangePasswordViewModel.Info = new InfoViewModel("Разные пароли", "#FF1010");
            return;
        }

        _mainWindowViewModel.CurrentUser.PassHash = User.GenerateHash(_mainWindowViewModel.CurrentUser.Username, ChangePasswordViewModel.PassOne);
        DataBaseService.ChangePassword(_mainWindowViewModel.CurrentUser);

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
using Avalonia.Media;
using Flotomachine.Services;
using Flotomachine.View;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Flotomachine.ViewModels;

public class AddDelUserCardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainWindowViewModel;

    private string _info;
    private IBrush _colorInfo;

    public ObservableCollection<CardIdListBox> CardsList { get; set; } = new ObservableCollection<CardIdListBox>();

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

    public ICommand AddUserCard { get; }
    public ICommand DelUserCard { get; }

    public AddDelUserCardViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
        AddUserCard = new DelegateCommand(AddCard);
        DelUserCard = new DelegateCommand(DeleteCard);
        _mainWindowViewModel.UserChangedEvent += RefreshCardList;
    }

    private void RefreshCardList(User user)
    {
        if (user == null)
        {
            CardsList.Clear();
            Info = "";
            return;
        }

        CardsList.Clear();
        foreach (CardId item in DataBaseService.GetCardIds(user))
        {
            CardsList.Add(new CardIdListBox(item));
        }
    }

    private async void AddCard(object parameter)
    {
        ReadCardWindow readCard = new ReadCardWindow();
        var result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Считывание отменено";
            return;
        }

        if (_mainWindowViewModel.CurrentUser == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Пользователь не найден";
            return;
        }

        var card = DataBaseService.GetCard(result);

        if (card != null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Уже зарегистрирована";
            return;
        }

        DataBaseService.CreateCard(_mainWindowViewModel.CurrentUser, result);
        RefreshCardList(_mainWindowViewModel.CurrentUser);
        ColorInfo = Brush.Parse("#10FF10");
        Info = "Успешно добавлена";
    }

    private async void DeleteCard(object parameter)
    {
        ReadCardWindow readCard = new ReadCardWindow();
        var result = await readCard.ShowDialog<byte[]>(App.MainWindow);

        if (result == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Считывание отменено";
            return;
        }

        if (_mainWindowViewModel.CurrentUser == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Пользователь не найден";
            return;
        }

        CardId card = DataBaseService.GetCard(result);
        if (card == null)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Не найдено";
            return;
        }

        if (card.UserId != _mainWindowViewModel.CurrentUser.Id)
        {
            ColorInfo = Brush.Parse("#FF1010");
            Info = "Не принадлежит";
            return;
        }

        DataBaseService.DeleteCard(card);
        RefreshCardList(_mainWindowViewModel.CurrentUser);
        ColorInfo = Brush.Parse("#10FF10");
        Info = "Успешно удалена";
    }
}

public class CardIdListBox
{
    public int Id { get; set; }

    public string CardId { get; set; }

    public CardIdListBox(CardId card)
    {
        Id = card.Id;
        CardId = BitConverter.ToString(card.CardBytes);
    }
}
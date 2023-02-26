using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Flotomachine.Services;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class AddDelUserCardViewModel : ViewModelBase
{

	private InfoViewModel _info;

	public ObservableCollection<CardIdListBox> CardsList { get; set; } = new ObservableCollection<CardIdListBox>();

	public InfoViewModel Info
	{
		get => _info;
		set => this.RaiseAndSetIfChanged(ref _info, value);
	}

	public ICommand AddUserCard { get; set; }
	public ICommand DelUserCard { get; set; }

	public AddDelUserCardViewModel()
	{

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
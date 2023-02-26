namespace Flotomachine.ViewModels
{
	public class GraphPanelControlViewModel : ViewModelBase
	{
		private readonly MainWindowViewModel _mainWindowViewModel;
		public GraphPanelControlViewModel()
		{
		}

		public GraphPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
		}

	}
}
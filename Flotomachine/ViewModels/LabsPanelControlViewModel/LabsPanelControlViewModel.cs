using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using AvaloniaEdit.Utils;
using Flotomachine.Services;
using ReactiveUI;

namespace Flotomachine.ViewModels;

public class LabsPanelControlViewModel : ViewModelBase
{
	private readonly MainWindowViewModel _mainWindowViewModel;
#if DEBUG
	private bool _debug = true;
#else
    private bool _debug = false;
#endif

	private Experiment _experimentSelected;
	private bool _visibleExperiment;

	private ObservableCollection<Experiment> _experimentSource = new();
	public ObservableCollection<Experiment> ExperimentCollection { get; set; } = new();
	//public HierarchicalTreeDataGridSource<Experiment> Source { get; private set; }

	public Experiment ExperimentSelected
	{
		get => _experimentSelected;
		set
		{
			_experimentSelected = value;
			ExperimentSelectedChanged(value);
		}
	}

	public bool VisibleExperiment
	{
		get => _visibleExperiment;
		set => this.RaiseAndSetIfChanged(ref _visibleExperiment, value);
	}

	public HierarchicalTreeDataGridSource<ExperimentData> ExperimentDataSource { get; set; }

	public LabsPanelControlViewModel()
	{
		Reload();
	}

	List<Experiment> DebugTestObj = new();

	public LabsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
	{
		_mainWindowViewModel = mainWindowViewModel;

		ExportExcelExperimentButtonClick = new DelegateCommand(ExportExcelExperiment);
		PrintExperimentButtonClick = new DelegateCommand(PrintExperiment);
		DeleteExperimentButtonClick = new DelegateCommand(DeleteExperiment);

		Reload();
	}

	private void Reload()
	{
		if (_debug)
		{
			ExperimentCollection.Clear();
			for (int x = 1; x <= 50; x++)
			{
				ExperimentCollection.Add(new Experiment() { Id = x });
			}
			return;
		}

		ExperimentCollection.Clear();
		ExperimentCollection.AddRange(DataBaseService.GetAllExperiments(_mainWindowViewModel.CurrentUser));
		_experimentSource.AddRange(DataBaseService.GetAllExperiments(_mainWindowViewModel.CurrentUser));
		foreach (Experiment item in _experimentSource)
		{
			List<ExperimentData>? data = item.GetExperimentData();
			foreach (ExperimentData? itemData in data)
			{
				itemData.ExperimentDataValues.AddRange(itemData.GetExperimentDataValues());
			}
			item.ExperimentData.AddRange(data);
		}
	}

	public ICommand ExportExcelExperimentButtonClick { get; }
	public ICommand PrintExperimentButtonClick { get; }
	public ICommand DeleteExperimentButtonClick { get; }

	public void ExperimentSelectedChanged(Experiment experiment)
	{
		VisibleExperiment = true;

		if (_debug)
		{

			return;
		}

		foreach (ExperimentData? item in DataBaseService.GetExperimentData(experiment.Id))
		{

		}

		ExperimentDataSource = new HierarchicalTreeDataGridSource<ExperimentData>(_people)
		{
			Columns =
			{
				new TextColumn<Person, string>("First Name", x => x.FirstName),
				new TextColumn<Person, string>("Last Name", x => x.LastName),
				new TextColumn<Person, int>("Age", x => x.Age),
			},
		};
	}

	public void ExportExcelExperiment(object obj)
	{

	}

	public void PrintExperiment(object obj)
	{

	}

	public void DeleteExperiment(object obj)
	{
		VisibleExperiment = false;



		Reload();
	}

	public class Person
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public int Age { get; set; }
		public ObservableCollection<Person> Children { get; } = new();
	}
}
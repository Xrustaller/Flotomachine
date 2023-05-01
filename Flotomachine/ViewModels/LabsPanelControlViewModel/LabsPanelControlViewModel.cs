using System;
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


	private ObservableCollection<Dictionary<string, int>> _experimentSource = new();
	public HierarchicalTreeDataGridSource<Dictionary<string, int>> ExperimentDataSource { get; set; }

	public LabsPanelControlViewModel()
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
	}

	public LabsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
	{
		_mainWindowViewModel = mainWindowViewModel;

		ExportExcelExperimentButtonClick = new DelegateCommand(ExportExcelExperiment);
		PrintExperimentButtonClick = new DelegateCommand(PrintExperiment);
		DeleteExperimentButtonClick = new DelegateCommand(DeleteExperiment);
		AddExperimentButtonClick = new DelegateCommand(AddExperiment);

		Reload();
	}

	private void Reload()
	{
		//if (_debug)
		//{
		//	ExperimentCollection.Clear();
		//	for (int x = 1; x <= 50; x++)
		//	{
		//		ExperimentCollection.Add(new Experiment() { Id = x });
		//	}
		//	return;
		//}

		ExperimentCollection.Clear();
		ExperimentCollection.AddRange(DataBaseService.GetAllExperiments(_mainWindowViewModel.CurrentUser));
	}

	public ICommand ExportExcelExperimentButtonClick { get; }
	public ICommand PrintExperimentButtonClick { get; }
	public ICommand DeleteExperimentButtonClick { get; }
	public ICommand AddExperimentButtonClick { get; }

	public void ExperimentSelectedChanged(Experiment experiment)
	{
		VisibleExperiment = true;

		List<string> modules = new List<string>();
		foreach (ExperimentData? item in DataBaseService.GetExperimentData(experiment.Id))
		{
			Dictionary<string, int> result = new Dictionary<string, int>();
			var values = DataBaseService.GetExperimentDataNameAndValues(item.Id);
			foreach (var item2 in values)
			{
				var name = item2.Item1;
				result.Add(name, item2.Item2.ModuleData);
				if (!modules.Contains(name))
				{
					modules.Add(name);
				}
			}
			_experimentSource.Add(result);
		}

		ExperimentDataSource = new HierarchicalTreeDataGridSource<Dictionary<string, int>>(_experimentSource);
		foreach (var item in modules)
		{
			ExperimentDataSource.Columns.Add(new TextColumn<Dictionary<string, int>, int>(item, x => x[item]));
		}
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

	public void AddExperiment(object obj)
	{
		var _fields = DataBaseService.GetModulesFields();

		var _experiment = DataBaseService.CreateExperiment(_mainWindowViewModel.CurrentUser, 3);

		Random random = new Random();

		for (int i = 0; i < random.Next(5, 25); i++)
		{
			List<ExperimentDataValue> Data = new();
			foreach (ModuleField field in _fields)
			{
				if (!field.Active)
				{
					continue;
				}
				Data.Add(new ExperimentDataValue(field, random.Next(1, 100000000)));
			}

			ExperimentData datai = DataBaseService.AddExperimentData(_experiment);
			DataBaseService.AddExperimentDataValues(datai, Data);
		}
		_experiment.End();
		DataBaseService.UpdateExperiment(_experiment);
	}
}
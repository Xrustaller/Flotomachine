using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Flotomachine.Services;
using Flotomachine.Utility;
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

	private int _experimentSelected;
	private bool _visibleExperiment;
	private ObservableCollection<ExpObj> _experiment = new ObservableCollection<ExpObj>();

	public ObservableCollection<int> ExperimentCollection { get; set; } = new();

	public int ExperimentSelected
	{
		get => _experimentSelected;
		set
		{
			_experimentSelected = value;
			ExperimentSelectedChanged(value);
		}
	}

	public ObservableCollection<ExpObj> Experiment
	{
		get => _experiment;
		set => this.RaiseAndSetIfChanged(ref _experiment, value);
	}

	public bool VisibleExperiment
	{
		get => _visibleExperiment;
		set => this.RaiseAndSetIfChanged(ref _visibleExperiment, value);
	}

	public LabsPanelControlViewModel()
	{

	}

	Dictionary<int, List<ExpObj>> DebugTestObj = new();

	public LabsPanelControlViewModel(MainWindowViewModel mainWindowViewModel)
	{
		_mainWindowViewModel = mainWindowViewModel;

		ExportExcelExperimentButtonClick = new DelegateCommand(ExportExcelExperiment);
		PrintExperimentButtonClick = new DelegateCommand(PrintExperiment);
		DeleteExperimentButtonClick = new DelegateCommand(DeleteExperiment);

		if (_debug)
		{

			for (int i = 1; i <= 50; i++)
			{
				List<ExpObj> items = new();
				for (int x = 1; x <= 50; x++)
				{
					items.Add(new ExpObj(DateTime.Now.ToLongTimeString(), DateTime.Now.Minute.ToString() + ":" + x, DateTime.Now.Minute.ToString() + ":" + i, "25", "750", (x + i).ToString(), "0"));
				}
				DebugTestObj.Add(i, items);
			}

			ExperimentCollection.Clear();
			foreach (var item in DebugTestObj)
			{
				ExperimentCollection.Add(item.Key);
			}
			return;
		}

		ExperimentCollection.Clear();
		foreach (int item in DataBaseService.GetExperiments(_mainWindowViewModel.CurrentUser))
		{
			ExperimentCollection.Add(item);
		}
	}

	public ICommand ExportExcelExperimentButtonClick { get; }
	public ICommand PrintExperimentButtonClick { get; }
	public ICommand DeleteExperimentButtonClick { get; }

	public void ExperimentSelectedChanged(int experimentId)
	{
		VisibleExperiment = true;
		Experiment.Clear();
		if (_debug)
		{
			foreach (var item in DebugTestObj[experimentId])
			{
				Experiment.Add(item);
			}
			return;
		}

		foreach (var item in DataBaseService.GetExperimentData(experimentId))
		{
			Experiment.Add(new ExpObj(item));
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

	}
}
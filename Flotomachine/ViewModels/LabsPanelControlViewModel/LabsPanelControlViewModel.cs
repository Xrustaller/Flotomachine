using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using AvaloniaEdit.Utils;
using DynamicData;
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

	private HierarchicalTreeDataGridSource<ExpDataObj> _experimentDataSource;
	private ObservableCollection<ExpDataObj> _experimentSource = new();

	public ObservableCollection<Experiment> ExperimentCollection { get; set; } = new();

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

	public HierarchicalTreeDataGridSource<ExpDataObj> ExperimentDataSource
	{
		get => _experimentDataSource;
		set => this.RaiseAndSetIfChanged(ref _experimentDataSource, value);
	}

	public ObservableCollection<ExpDataObj> ExperimentSource
	{
		get => _experimentSource;
		set => this.RaiseAndSetIfChanged(ref _experimentSource, value);
	}

	public ICommand ExportExcelExperimentButtonClick { get; }
	public ICommand PrintExperimentButtonClick { get; }
	public ICommand DeleteExperimentButtonClick { get; }
	public ICommand AddExperimentButtonClick { get; }

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
		ExperimentDataSource = new HierarchicalTreeDataGridSource<ExpDataObj>(ExperimentSource)
		{
			Columns =
			{
				new HierarchicalExpanderColumn<ExpDataObj>(new TextColumn<ExpDataObj, TimeSpan?>("Время сбора", x => x.DateTime), x => x.Values),
				new TextColumn<ExpDataObj, string>("Модуль", x => x.Name),
				new TextColumn<ExpDataObj, int?>("Значение", x => x.Value)
			}
		};
		ExperimentCollection.Clear();
		ExperimentCollection.AddRange(DataBaseService.GetAllExperiments(_mainWindowViewModel.CurrentUser));
	}

	public void ExperimentSelectedChanged(Experiment? experiment)
	{
		VisibleExperiment = true;

		ExperimentSource.Clear();

		if (experiment == null)
		{
			return;
		}

		foreach (ExperimentData? experimentData in DataBaseService.GetExperimentData(experiment.Id))
		{
			var ex = new ExpDataObj(experimentData);
			List<ExperimentDataValue>? values = DataBaseService.GetExperimentDataValues(experimentData.Id);
			foreach (var item in values)
			{
				ex.Values.Add(new ExpDataObj(item));
			}
			ExperimentSource.Add(ex);
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
			DataBaseService.AddExperimentDataValues(DataBaseService.AddExperimentData(_experiment),
				(from field in _fields
				 where field.Active
				 select new ExperimentDataValue(field, random.Next(1, 100000000))).ToList());
		_experiment.End();
		DataBaseService.UpdateExperiment(_experiment);
	}
}
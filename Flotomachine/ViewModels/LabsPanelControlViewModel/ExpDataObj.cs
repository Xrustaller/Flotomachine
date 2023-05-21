using System;
using System.Collections.ObjectModel;
using Flotomachine.Services;

namespace Flotomachine.ViewModels;

public class ExpDataObj
{
	public int? ExpDataId { get; set; }
	public int? ExperimentDataValueId { get; set; }
	public TimeSpan? DateTime { get; set; }
	public string? Name { get; set; }
	public int? Value { get; set; }
	public ObservableCollection<ExpDataObj> Values { get; set; }

	public ExpDataObj(ExperimentData item)
	{
		ExpDataId = item.Id;
		DateTime = item.Date.TimeOfDay;
		Values = new ObservableCollection<ExpDataObj>();
	}

	public ExpDataObj(ExperimentDataValue item)
	{
		ExperimentDataValueId = item.Id;
		Name = $"{item.ModuleField.Module.Name}-{item.ModuleField.FieldName}({item.ModuleField.ValueName})";
		Value = item.ModuleData;
		Values = new ObservableCollection<ExpDataObj>();
	}
}
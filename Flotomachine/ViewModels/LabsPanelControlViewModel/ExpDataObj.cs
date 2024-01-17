using System;
using Flotomachine.Elements;
using Flotomachine.Services;

namespace Flotomachine.ViewModels;

public class ExpDataObj
{
	public int? ExpDataId { get; set; }
	public TimeSpan? DateTime { get; set; }
	public ObservableDictionary<int, int> ValuesDictionary { get; set; }
	public ExpDataObj(ExperimentData item)
	{
		ExpDataId = item.Id;
		DateTime = item.Date.TimeOfDay;
		ValuesDictionary = new ObservableDictionary<int, int>();
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment")]
public class Experiment
{
	private string? _name;

	[Column("_id")]
	public int Id { get; set; }

	[Column("name")]
	public string? Name
	{
		get => _name ?? Id.ToString();
		set => _name = value;
	}

	[Column("user_id")]
	public int UserId { get; set; }

	[Column("timer_tick")]
	public int TimerTick { get; set; }

	[Column("date_start")]
	public DateTime DateStart { get; set; }

	[Column("date_end")]
	public DateTime? DateEnd { get; set; }
	public virtual ObservableCollection<ExperimentData> ExperimentData { get; set; }

	public Experiment()
	{

	}

	public Experiment(int user, int timerTick)
	{
		UserId = user;
		TimerTick = timerTick;
		DateStart = DateTime.Now;
	}

	public Experiment(User user, int timerTick)
	{
		UserId = user.Id;
		TimerTick = timerTick;
		DateStart = DateTime.Now;
	}

	public List<ExperimentData> GetExperimentData() => DataBaseService.GetExperimentData(Id);

	public void End()
	{
		DateEnd = DateTime.Now;
	}
}
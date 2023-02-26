using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment_data")]
public class ExperimentData
{
	[Key, Column("_id")]
	public int Id { get; set; }

	[Column("experiment")]
	public int ExperimentId { get; set; }
	public virtual Experiment Experiment { get; set; }

	[Column("date")]
	public DateTime Date { get; set; }

	public virtual List<ExperimentDataValue> ExperimentDataValues { get; set; }

	public ExperimentData()
	{

	}
	public ExperimentData(Experiment experiment)
	{
		ExperimentId = experiment.Id;
		Date = DateTime.Now;
	}
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment_data_value")]
public class ExperimentDataValue
{
	[Key, Column("_id")]
	public int Id { get; set; }

	[Column("experiment_data")]
	public int ExperimentDataId { get; set; }
	public virtual ExperimentData ExperimentData { get; set; }

	[Column("module_field")]
	public int ModuleFieldId { get; set; }
	public virtual ModuleField ModuleField { get; set; }

	[Column("module_data")]
	public int ModuleData { get; set; }

	public ExperimentDataValue()
	{

	}
	public ExperimentDataValue(int field, int data)
	{
		ExperimentDataId = -1;
		ModuleFieldId = field;
		ModuleData = data;
	}
	public ExperimentDataValue(ModuleField field, int data)
	{
		ExperimentDataId = -1;
		ModuleFieldId = field.Id;
		ModuleData = data;
	}
	public ExperimentDataValue(int experimentId, int field, int data)
	{
		ExperimentDataId = experimentId;
		ModuleFieldId = field;
		ModuleData = data;
	}
	public ExperimentDataValue(Experiment experiment, int field, int data)
	{
		ExperimentDataId = experiment.Id;
		ModuleFieldId = field;
		ModuleData = data;
	}
	public ExperimentDataValue(int experimentId, ModuleField field, int data)
	{
		ExperimentDataId = experimentId;
		ModuleFieldId = field.Id;
		ModuleData = data;
	}
	public ExperimentDataValue(Experiment experiment, ModuleField field, int data)
	{
		ExperimentDataId = experiment.Id;
		ModuleFieldId = field.Id;
		ModuleData = data;
	}
}
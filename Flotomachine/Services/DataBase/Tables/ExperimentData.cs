using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment_data")]
public class ExperimentData
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("experiment")]
    public int Experiment { get; set; }

    [Column("module_field")]
    public int ModuleField { get; set; }

    [Column("module_data")]
    public int ModuleData { get; set; }

    public ExperimentData()
    {

    }

    public ExperimentData(int experiment, int field, int data)
    {
        Experiment = experiment;
        ModuleField = field;
        ModuleData = data;
    }
    public ExperimentData(Experiment experiment, int field, int data)
    {
        Experiment = experiment.Id;
        ModuleField = field;
        ModuleData = data;
    }
}
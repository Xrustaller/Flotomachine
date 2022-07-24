using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment_data")]
public class ExperimentData
{
    [Column("_id")]
    public int Id { get; set; }

    [Column("user")]
    public int UserId { get; set; }

    [Column("module_field")]
    public int ModuleField { get; set; }

    [Column("module_data")]
    public int ModuleData { get; set; }

    public ExperimentData()
    {

    }

    public ExperimentData(int user, int field, int data)
    {
        UserId = user;
        ModuleField = field;
        ModuleData = data;
    }
}
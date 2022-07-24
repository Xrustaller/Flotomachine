using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

/// <summary>
/// Поля считывания датчиков
/// </summary>
[Table("module_field")]
public class ModuleField
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("module_type")]
    public int ModuleType { get; set; }

    [Column("data_name")]
    public string DataName { get; set; }

    [Column("start_address")]
    public int StartAddress { get; set; }
}
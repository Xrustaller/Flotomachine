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

    [Column("module_id")]
    public int ModuleId { get; set; }

    [Column("field_name")]
    public string FieldName { get; set; }

    [Column("start_address")]
    public int StartAddress { get; set; }

    [Column("value_name")]
    public string ValueName { get; set; }

    [Column("active")]
    public bool Active { get; set; }
}
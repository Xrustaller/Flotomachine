using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("module_field")]
public class ModuleField
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("module_type")]
    public int ModuleType { get; set; }
}
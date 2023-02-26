using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

/// <summary>
/// Модуль
/// </summary>
[Table("modules")]
public class Module
{
	[Key, Column("_id")]
	public byte Id { get; set; }

	[Column("name")]
	public string Name { get; set; }
	[Column("active")]
	public bool Active { get; set; }

	public virtual List<ModuleField> Fields { get; set; }
}


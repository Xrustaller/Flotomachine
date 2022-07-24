﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

/// <summary>
/// Модуль
/// </summary>
[Table("module_type")]
public class ModuleType
{
    [Key, Column("_id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }
}


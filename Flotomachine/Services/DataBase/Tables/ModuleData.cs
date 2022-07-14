using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("module_data")]
public class ModuleData
{
    [Column("_id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("data")]
    public int Data { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    public ModuleData()
    {

    }

    public ModuleData(int id, int user, int data)
    {
        Id = id;
        UserId = user;
        Data = data;
        Date = DateTime.Now;
    }

    public ModuleData(ModuleType module, User user, int data)
    {
        Id = module.Id;
        UserId = user.Id;
        Data = data;
        Date = DateTime.Now;
    }

    public ModuleData(int id, User user, int data)
    {
        Id = id;
        UserId = user.Id;
        Data = data;
        Date = DateTime.Now;
    }

    public ModuleData(ModuleType module, int user, int data)
    {
        Id = module.Id;
        UserId = user;
        Data = data;
        Date = DateTime.Now;
    }
}
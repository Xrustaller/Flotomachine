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

    [Column("date")]
    public DateTime Date { get; set; }

    public ExperimentData()
    {

    }

    public ExperimentData(int id, int user, int data)
    {
        Id = id;
        UserId = user;
        Date = DateTime.Now;
    }

    public ExperimentData(int id, User user, int data)
    {
        Id = id;
        UserId = user.Id;
        Date = DateTime.Now;
    }
    
}
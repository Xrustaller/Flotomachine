using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flotomachine.Services;

[Table("experiment")]
public class Experiment
{
    [Column("_id")]
    public int Id { get; set; }

    [Column("user")]
    public int UserId { get; set; }

    [Column("timer_tick")]
    public int TimerTick { get; set; }

    [Column("date_start")]
    public DateTime DateStart { get; set; }

    [Column("date_end")]
    public DateTime? DateEnd { get; set; }

    public Experiment()
    {

    }

    public Experiment(int id, int user, int timerTick)
    {
        Id = id;
        UserId = user;
        TimerTick = timerTick;
        DateStart = DateTime.Now;
    }

    public Experiment(int id, User user, int timerTick)
    {
        Id = id;
        UserId = user.Id;
        TimerTick = timerTick;
        DateStart = DateTime.Now;
    }
}
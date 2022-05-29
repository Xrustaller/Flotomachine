using System.ComponentModel;

namespace Flotomachine.Services;

public class TimerObject : INotifyPropertyChanged
{
    private string _name;
    private TimeStruct _time;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Name
    {
        get => _name;
        set
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            _name = value;
        }
    }

    public TimeStruct Time
    {
        get => _time;
        set
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Time"));
            _time = value;
        }
    }

    public TimerObject(string name, TimeStruct time)
    {
        Name = name;
        Time = time;
    }

    public TimerObject(string name, int min, int sec)
    {
        Name = name;
        Time = new TimeStruct(min, sec);
    }

    public TimerObject(string name, int sec)
    {
        Name = name;
        Time = new TimeStruct(sec);
    }

}
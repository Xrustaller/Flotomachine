using System.ComponentModel;

namespace Flotomachine.Services;

public struct TimeStruct : INotifyPropertyChanged
{
    private int _min;
    private int _sec;
    public event PropertyChangedEventHandler PropertyChanged;

    public int Min
    {
        get => _min;
        set
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Min"));
            _min = value;
        }
    }

    public int Sec
    {
        get => _sec;
        set
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sec"));
            _sec = value;
        }
    }

    public TimeStruct(int min, int sec) : this()
    {
        Min = min;
        Sec = sec;
    }

    public TimeStruct(int sec) : this()
    {
        Min = sec / 60;
        Sec = sec % 60;
    }

    public override string ToString()
    {
        return $"{Min}:{Sec}";
    }

}
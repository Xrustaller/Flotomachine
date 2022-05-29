using System;

namespace Flotomachine.Utility;

[Serializable]
public class Settings
{
    public MainSettings Main { get; set; }
    public RfIdSettings RfId { get; set; }
    public SerialSettings Serial { get; set; }


    [Serializable]
    public class MainSettings
    {
        
    }

    [Serializable]
    public class RfIdSettings
    {

    }

    [Serializable]
    public class SerialSettings
    {
        
    }
}
﻿using System;

namespace Flotomachine.Utility;

[Serializable]
public class Settings : IJsonConfiguration<Settings>
{
    public MainSettings Main { get; set; }
    public RfIdSettings RfId { get; set; }
    public SerialSettings Serial { get; set; }
    public DatabaseSettings DataBase { get; set; }

    public Settings()
    {
        Main = new MainSettings();
        RfId = new RfIdSettings();
        Serial = new SerialSettings();
        DataBase = new DatabaseSettings();
    }

    public Settings DefaultConfig()
    {
        return new Settings
        {
            Main = new MainSettings
            {
                MainTimerModuleId = 1,
            },
            RfId = new RfIdSettings
            {
                BusId = 0,
                LineId = 0,
                ClockFrequencySpi = 1_000_000,
            },
            Serial = new SerialSettings
            {
                Port = "COM0",
                BaudRate = 19200,
            },
            DataBase = new DatabaseSettings
            {
                FileName = "Flotomachine.db"
            }
        };
    }
}

[Serializable]
public class MainSettings
{
    public byte MainTimerModuleId { get; set; }

    public MainSettings()
    {

    }
}

[Serializable]
public class RfIdSettings
{
    public int BusId { get; set; }
    public int LineId { get; set; }
    public int ClockFrequencySpi { get; set; }

    public RfIdSettings()
    {

    }
}

[Serializable]
public class SerialSettings
{
    public string Port { get; set; }
    public int BaudRate { get; set; }

    public SerialSettings()
    {

    }
}

[Serializable]
public class DatabaseSettings
{
    public string FileName { get; set; }

    public DatabaseSettings()
    {

    }
}
﻿using System;

namespace Flotomachine.Utility;

[Serializable]
public class Settings : IJsonConfiguration<Settings>
{
	public MainSettings Main { get; set; } = new();
	public RfIdSettings RfId { get; set; } = new();
	public SerialSettings Serial { get; set; } = new();
	public CameraSettings Camera { get; set; } = new();
	public DatabaseSettings DataBase { get; set; } = new();

	public Settings()
	{

	}

	public Settings DefaultConfig() => new();
}

[Serializable]
public class MainSettings
{
	public string UpdateJsonUrl { get; set; } = "https://raw.githubusercontent.com/Xrustaller/Flotomachine/master/update.json";
	public string UpdateFileUrl { get; set; } = "https://github.com/Xrustaller/Flotomachine/releases/latest/download/";
	public byte MainTimerModuleId { get; set; } = 1;
	public bool CheckUpdatesAtStartUp { get; set; } = true;

	public MainSettings()
	{

	}
}

[Serializable]
public class RfIdSettings
{
	public int BusId { get; set; } = 0;
	public int LineId { get; set; } = 0;
	public int ClockFrequencySpi { get; set; } = 1_000_000;

	public RfIdSettings()
	{

	}
}

[Serializable]
public class SerialSettings
{
	public string Port { get; set; } = "COM0";
	public int BaudRate { get; set; } = 9600;

	public SerialSettings()
	{

	}
}

[Serializable]
public class CameraSettings
{
	public string Url { get; set; } = "http://192.1.1.10:8000";

	public CameraSettings()
	{

	}
}

[Serializable]
public class DatabaseSettings
{
	public string DefaultDataBaseUrl { get; set; } = "https://raw.githubusercontent.com/Xrustaller/Flotomachine/master/FlotomachineDefault.db";
	public string FileName { get; set; } = "Flotomachine.db";

	public DatabaseSettings()
	{

	}
}
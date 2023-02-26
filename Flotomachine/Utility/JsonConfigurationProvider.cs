using System;
using System.IO;

namespace Flotomachine.Utility;

public interface IJsonConfiguration<out T>
{
    T DefaultConfig();
}

public class JsonConfigurationProvider<T> where T : IJsonConfiguration<T>, new()
{
    private readonly string _name;
    public T Configuration { get; set; } = new T().DefaultConfig();

    public JsonConfigurationProvider(string name = "Config")
    {
        _name = name;
        if (File.Exists(_name))
        {
            InitConfigurationProvider();
            return;
        }
        SaveConfig();
    }

    public JsonConfigurationProvider(T configuration, string name = "Config")
    {
        _name = name;
        Configuration = configuration;
        if (File.Exists(_name))
        {
            InitConfigurationProvider();
            return;
        }
        SaveConfig();
    }

    public void InitConfigurationProvider()
    {
        try
        {
            Configuration = OpenConfig();
        }
        catch (Exception)
        {
            File.Delete(_name);
            T newConf = new();
            SaveConfig(newConf.DefaultConfig());
            Configuration = OpenConfig();
        }
    }

    private T OpenConfig() => JsonProvider.ReadJson<T>(_name) ?? throw new Exception($"Error configuration. Config file opening error.");
    public void SaveConfig() => SaveConfig(Configuration);
    public void SaveConfig(T config) => JsonProvider.WriteJson(_name, config);
}
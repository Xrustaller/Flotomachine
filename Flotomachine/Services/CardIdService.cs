using Iot.Device.Mfrc522;
using Iot.Device.Rfid;
using System;
using System.Device.Spi;
using System.Threading;

namespace Flotomachine.Services;

public class CardIdService : IDisposable
{
    private readonly int _busId;
    private readonly int _lineId;
    private readonly int _pinReset;

    private SpiConnectionSettings _spiConnection;
    private SpiDevice _spiDevice;
    private MfRc522 _mfrc522;

    public CardIdService()
    {
        _busId = 0;
        _lineId = 0;
        _pinReset = 22;
    }

    public CardIdService(int busId, int lineId, int pinReset = -1)
    {
        _busId = busId;
        _lineId = lineId;
        _pinReset = pinReset;
    }

    public string CreateConnection(int clockFrequency = 1_000_000) //было 5000000, надо уменьшить скорость
    {
        _spiConnection = new(_busId, _lineId)
        {
            // Here you can use as well MfRc522.MaximumSpiClockFrequency which is 10_000_000
            // Anything lower will work as well
            ClockFrequency = clockFrequency
        };
        _spiDevice = SpiDevice.Create(_spiConnection);
        _mfrc522 = new MfRc522(_spiDevice, _pinReset);
#if DEBUG
        Console.WriteLine("SPI Connection create");
        Console.WriteLine("Version MFC522: " + _mfrc522.Version + " Only versions 1.0 and  2.0 are valid for authentic MFRC522. Some copies may not have a proper version but would just work.");
#endif
        return _mfrc522.Version.ToString();
    }

    public Data106kbpsTypeA ReadCard()
    {
        bool res;
        Data106kbpsTypeA card;
        try
        {
            do
            {
                res = _mfrc522.ListenToCardIso14443TypeA(out card, TimeSpan.FromSeconds(2));
                Thread.Sleep(res ? 0 : 200);
            }
            while (!res);
            return card;
        }
        catch
        {
            Dispose();
            return null;
        }
    }

    public void Dispose()
    {
        _spiDevice?.Dispose();
        _mfrc522?.Dispose();
    }
}
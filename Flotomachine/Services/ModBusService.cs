using Modbus.Device;
using System;
using System.IO.Ports;
using System.Threading;

namespace Flotomachine.Services;

public static class ModBusService
{
    private static Thread _thread;
    private static bool _exit;

    private static SerialPort _serialPort;
    private static ModbusSerialMaster _modbusSerialMaster;

    public static readonly int[] BaudRateList = { 4800, 9600, 19200, 38400, 57600, 115200, 230400 };

    public static Exception Initialize(string portName, int baudRate = 115200)
    {
        _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        try
        {
            _thread = new Thread(ThisThread);
            _thread.Start();
            _serialPort.Open();
            _modbusSerialMaster = ModbusSerialMaster.CreateRtu(_serialPort);
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }

    private static void ThisThread()
    {
        // Должен пробегаться по всем устройствам
        // Если на главном таймере нажалась кнопка активации эксперимента, то запускает запись данных в базу 
        // Стандартный режим подразумевает считывание датчиков и вывод их на главный экран. 
        while (!_exit)
        {

        }
    }

    public static void Exit()
    {
        _exit = true;
    }

    private static void пример()
    {
        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(_serialPort);

        byte slaveID = 1;
        ushort startAddress = 0;
        ushort numOfPoints = 1;
        ushort[] holding_register = master.ReadHoldingRegisters(slaveID, startAddress, numOfPoints);
        Console.WriteLine(holding_register);
        Console.ReadKey();
    }
}
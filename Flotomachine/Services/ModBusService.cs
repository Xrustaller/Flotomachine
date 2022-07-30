using Modbus.Device;
using System;
using System.IO.Ports;
using System.Threading;
using Modbus.Utility;

namespace Flotomachine.Services;

public enum ModBusState
{
    Close,
    Error,
    Wait,
    Experiment
}

public static class ModBusService
{
    private static string _serialPort;
    private static int _baudRate;

    private static SerialPort _port;
    private static ModbusSerialMaster _bus;

    private static ModBusState _state = ModBusState.Close;

    public static ModBusState State
    {
        get => _state;
        private set => _state = value;
    }

    private static Thread _thread;
    private static bool _exit;

    public static readonly int[] BaudRateList = { 4800, 9600, 19200, 38400, 57600, 115200, 230400 };
    public static readonly int[] PinRaspberryList = { 4, 5, 6, 12, 13, 17, 18, 22, 23, 24, 25, 26, 27 };


    public static Exception Initialize()
    {
        try
        {
            _serialPort = App.Settings.Configuration.Serial.Port;
            _baudRate = App.Settings.Configuration.Serial.BaudRate;
            _thread = new Thread(ThisThread) { Name = "ModBusServise" };
            _thread.Start();
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }

    private static void CreatePort()
    {
        while (State is ModBusState.Error or ModBusState.Close && !_exit)
        {
            if (Create())
            {
                State = ModBusState.Wait;
                _bus = ModbusSerialMaster.CreateRtu(_port);
                _bus.Transport.ReadTimeout = 1000;
                _bus.Transport.WriteTimeout = 1000;
                break;
            }

            State = ModBusState.Error;
            Thread.Sleep(500);
        }

        bool Create()
        {
            if (_port is { IsOpen: true })
            {
                _port.Close();
                State = ModBusState.Close;
            }

            _serialPort = App.Settings.Configuration.Serial.Port;
            _baudRate = App.Settings.Configuration.Serial.BaudRate;

            _port = new SerialPort(_serialPort);
            _port.BaudRate = _baudRate;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.Parity = Parity.None;
            try
            {
                _port.Open();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    private static void ThisThread()
    {
        int timerTick = 3;

        CreatePort();

        // Должен пробегаться по всем устройствам
        // Если на главном таймере нажалась кнопка активации эксперимента, то запускает запись данных в базу 
        // Стандартный режим подразумевает считывание датчиков и вывод их на главный экран. 
        while (!_exit)
        {
            if (_serialPort != App.Settings.Configuration.Serial.Port || _baudRate != App.Settings.Configuration.Serial.BaudRate)
            {
                CreatePort();
            }
            
            switch (State)
            {
                case ModBusState.Wait:
                    timerTick = ReadInputRegisters(App.Settings.Configuration.Main.MainTimerModuleId, 1, 1)[0];
                    if (ReadInputs(App.Settings.Configuration.Main.MainTimerModuleId, 7, 1)[0])
                    {
                        State = ModBusState.Experiment;
                    }
                    
                    break;
                case ModBusState.Experiment:
                    if (ReadInputs(App.Settings.Configuration.Main.MainTimerModuleId, 7, 1)[0])
                    {
                        State = ModBusState.Wait;
                    }
                    
                    break;
            }

            ReadInputRegisters(1, 0, 8);
            Thread.Sleep(250);
        }

        ushort[] ReadInputRegisters(byte slaveId, ushort startAdd, ushort numOfPoints)
        {
            try
            {
                return _bus.ReadInputRegisters(slaveId, startAdd, numOfPoints);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        bool[] ReadInputs(byte slaveId, ushort startAdd, ushort numOfPoints)
        {
            try
            {
                return _bus.ReadInputs(slaveId, startAdd, numOfPoints);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public static void Exit()
    {
        _exit = true;
    }

    public static void ModbusSerialRtuMasterWriteRegisters()
    {
        using SerialPort port = new SerialPort("COM1");
        // configure serial port
        port.BaudRate = 9600;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.Open();

        // create modbus master
        IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

        byte slaveId = 1;
        ushort startAddress = 100;
        ushort[] registers = new ushort[] { 1, 2, 65535 };

        // write three registers
        master.WriteMultipleRegisters(slaveId, startAddress, registers);
    }

    public static void ModbusSerialRtuMasterReadRegisters()
    {
        using SerialPort port = new SerialPort("COM1");
        // configure serial port
        port.BaudRate = 9600;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.Open();

        // create modbus master
        IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

        byte slaveId = 1;
        ushort startAddress = 1;

        // read three registers
        ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 8);
        //uint value = ModbusUtility.GetUInt32(registers[0], registers[1]);
    }

    /// <summary>
    ///     Write a 32 bit value.
    /// </summary>
    public static void ReadWrite32BitValue()
    {
        using SerialPort port = new SerialPort("COM1");
        // configure serial port
        port.BaudRate = 9600;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        port.Open();

        // create modbus master
        ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

        byte slaveId = 1;
        ushort startAddress = 1008;
        uint largeValue = ushort.MaxValue + 5;

        ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 0);
        ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 2);

        // write large value in two 16 bit chunks
        master.WriteMultipleRegisters(slaveId, startAddress, new ushort[] { lowOrderValue, highOrderValue });

        // read large value in two 16 bit chunks and perform conversion
        ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 2);
        uint value = ModbusUtility.GetUInt32(registers[1], registers[0]);
    }
}
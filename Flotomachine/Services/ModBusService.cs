using Modbus.Device;
using Modbus.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace Flotomachine.Services;

public enum ModBusState
{
    Close,
    Error,
    Wait,
    Experiment
}

public class ModBusService
{
    private string _serialPort;
    private int _baudRate;
    private ModBusState _state = ModBusState.Close;

    public SerialPort SerialPort { get; private set; }
    private ModbusSerialMaster _bus;
    private bool _exit;

    public ModBusState State
    {
        get => _state;
        private set
        {
            _state = value;
            StatusChanged?.Invoke(value);
        }
    }

    public static readonly int[] BaudRateList = { 1200, 4800, 9600, 19200, 38400, 57600, 115200 };
    //public readonly int[] PinRaspberryList = { 4, 5, 6, 12, 13, 17, 18, 22, 23, 24, 25, 26, 27 };

    public event Action<ModBusState> StatusChanged;
    public event Action DataCollected;

    public readonly Thread Thread;

    public ModBusService()
    {
        Thread = new Thread(ThisThread) { Name = "ModBusServise" };
        Thread.Start();
    }

    private void CreatePort()
    {
        while (State is ModBusState.Error or ModBusState.Close && !_exit)
        {
            if (SerialPort is { IsOpen: true })
            {
                SerialPort.Close();
                State = ModBusState.Close;
            }

            _serialPort = App.Settings.Configuration.Serial.Port;
            _baudRate = App.Settings.Configuration.Serial.BaudRate;

            SerialPort = new SerialPort(_serialPort);
            SerialPort.BaudRate = _baudRate;
            SerialPort.DataBits = 8;
            SerialPort.StopBits = StopBits.One;
            SerialPort.Parity = Parity.None;
            try
            {
                SerialPort.Open();
            }
            catch (Exception)
            {
                State = ModBusState.Error;
                Thread.Sleep(500);
                continue;
            }
            _bus = ModbusSerialMaster.CreateRtu(SerialPort);
            _bus.Transport.ReadTimeout = 1000;
            _bus.Transport.WriteTimeout = 1000;
            State = ModBusState.Wait;
        }
    }

    private void ThisThread()
    {
        int timerTick = 3;

        CreatePort();

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

            List<Module> modules = DataBaseService.GetModules();
            List<ModuleField> fields = new List<ModuleField>();
            foreach (Module item in modules)
            {
                fields.AddRange(DataBaseService.GetModulesFields(item));
            }




            DataCollected?.Invoke();
            Thread.Sleep(250);
        }
    }

    public void Exit()
    {
        _exit = true;
    }

    private ushort[] ReadInputRegisters(byte slaveId, ushort startAdd, ushort numOfPoints)
    {
        try
        {
            return _bus.ReadInputRegisters(slaveId, startAdd, numOfPoints);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private bool[] ReadInputs(byte slaveId, ushort startAdd, ushort numOfPoints)
    {
        try
        {
            return _bus.ReadInputs(slaveId, startAdd, numOfPoints);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void ModbusSerialRtuMasterWriteRegisters()
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

    public void ModbusSerialRtuMasterReadRegisters()
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
    public void ReadWrite32BitValue()
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
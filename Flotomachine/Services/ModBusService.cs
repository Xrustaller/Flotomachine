using Modbus.Device;
using Modbus.Utility;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

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
    private static ModBusState _state = ModBusState.Close;

    public static SerialPort SerialPort { get; private set; }
    private static ModbusSerialMaster _bus;
    private static bool _exit;

    public static ModBusState State
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

    public static event Action<ModBusState> StatusChanged;
    public static event Action<List<ExperimentDataValue>> DataCollected;

    public static Thread Thread;

    private static List<ModuleField> _fields;
    private static readonly List<ExperimentDataValue> _datas = new();

    private static Experiment _experiment;
    private static Timer _experimentTimer;

    public static Exception Initialize()
    {
        try
        {
            Thread = new Thread(ThisThread) { Name = "ModBusServise" };
            Thread.Start();
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private static void CreatePort()
    {
        _fields = DataBaseService.GetAllModulesFields();

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
            _bus.Transport.ReadTimeout = 200;
            _bus.Transport.WriteTimeout = 200;
            State = ModBusState.Wait;
        }
    }

    private static void ReadAllModules()
    {
        lock (_datas)
        {
            _datas.Clear();
            foreach (ModuleField field in _fields)
            {
                ushort? data = ReadInputRegisters((byte)field.ModuleId, (byte)field.StartAddress);
                if (data != null)
                {
                    _datas.Add(new ExperimentDataValue(field, data.Value));
                }
            }
            DataCollected?.Invoke(_datas);
        }
    }

    private static void ThisThread()
    {
        CreatePort();

        while (!_exit)
        {
            if (_serialPort != App.Settings.Configuration.Serial.Port || _baudRate != App.Settings.Configuration.Serial.BaudRate)
            {
                CreatePort();
            }

            

            bool? expInput = ReadInputs(App.Settings.Configuration.Main.MainTimerModuleId, 7);

            if (expInput == null)
            {
                State = ModBusState.Error;
                continue;
            }

            ReadAllModules();

            switch (State)
            {
                case ModBusState.Wait:
                    {
                        if (expInput == true && App.MainWindowViewModel.CurrentUser != null && App.MainWindowViewModel.CurrentUser.Root != true)
                        {
                            State = ModBusState.Experiment;
                            
                            ushort timer = ReadInputRegisters(App.Settings.Configuration.Main.MainTimerModuleId, 1) ?? 3;
                            _experiment = DataBaseService.CreateExperiment(App.MainWindowViewModel.CurrentUser, timer);

                            _experimentTimer = new Timer(timer * 1000);
                            _experimentTimer.AutoReset = true;
                            _experimentTimer.Elapsed += TimerElapsed;
                            _experimentTimer.Start();
                        }
                        break;
                    }

                case ModBusState.Experiment:
                    {
                        if (expInput == false)
                        {
                            State = ModBusState.Wait;
                            _experimentTimer.Stop();
                            _experiment?.End();
                            DataBaseService.UpdateExperiment(_experiment);
                            _experiment = null;
                        }
                        break;
                    }

            }

            Thread.Sleep(200);
        }
    }

    private static void TimerElapsed(object source, ElapsedEventArgs e)
    {
        lock (_datas)
        {
            lock (_experiment)
            {
                ExperimentData data = DataBaseService.AddExperimentData(_experiment);
                DataBaseService.AddExperimentDataValues(data, _datas);
            }
        }
    }

    public static void Exit()
    {
        _exit = true;
    }

    private static ushort? ReadInputRegisters(byte slaveId, ushort startAdd)
    {
        try
        {
            return _bus.ReadInputRegisters(slaveId, startAdd, 1)[0];
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool? ReadInputs(byte slaveId, ushort startAdd)
    {
        try
        {
            return _bus.ReadInputs(slaveId, startAdd, 1)[0];
        }
        catch (Exception)
        {
            return null;
        }
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
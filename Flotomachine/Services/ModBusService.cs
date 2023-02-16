using Flotomachine.ViewModels;
using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
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
    private static Thread _thread;
    private static bool _exit;

    private static string _serialPortName;
    private static int _serialPortBaudRate;
    private static ModBusState _state = ModBusState.Close;

    private static SerialPort _serialPort;
    private static ModbusSerialMaster _bus;

    private static List<ModuleField> _fields;
    private static readonly List<ExperimentDataValue> Data = new();

    private static Experiment _experiment;
    private static Timer _experimentTimer;

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
    public static event Action<List<HomeModuleDataViewModel>> DataCollected;

    public static Exception Initialize()
    {
        try
        {
            _thread = new Thread(ThisThread) { Name = "ModBusServise" };
            _thread.Start();
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private static void ThisThread()
    {
        RefreshPort();

        while (!_exit)
        {
            if (_serialPortName != App.Settings.Configuration.Serial.Port || _serialPortBaudRate != App.Settings.Configuration.Serial.BaudRate)
            {
                RefreshPort();
            }

            bool? expInput = ReadInputs(App.Settings.Configuration.Main.MainTimerModuleId, 7);

            if (expInput == null)
            {
                State = ModBusState.Error;
            }

            ReadAndViewAllModules();

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
                            _experimentTimer.Elapsed += (_, _) =>
                            {
                                lock (Data)
                                {
                                    lock (_experiment)
                                    {
                                        ExperimentData data = DataBaseService.AddExperimentData(_experiment);
                                        DataBaseService.AddExperimentDataValues(data, Data);
                                    }
                                }
                            };
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
                case ModBusState.Error:
                    {
                        RefreshPort();
                        break;
                    }

            }

            Thread.Sleep(200);
        }
    }

    private static void RefreshPort()
    {
        _fields = DataBaseService.GetModulesFields();

        while (State is ModBusState.Error or ModBusState.Close && !_exit)
        {
            if (_serialPort is { IsOpen: true })
            {
                _serialPort.Close();
                State = ModBusState.Close;
            }

            _serialPortName = App.Settings.Configuration.Serial.Port;
            _serialPortBaudRate = App.Settings.Configuration.Serial.BaudRate;

            _serialPort = new SerialPort(_serialPortName)
            {
                BaudRate = _serialPortBaudRate,
                DataBits = 8,
                StopBits = StopBits.One,
                Parity = Parity.None
            };

            try
            {
                _serialPort.Open();
            }
            catch (Exception)
            {
                ReadAndViewAllModules();
                State = ModBusState.Error;
                Thread.Sleep(800);
                continue;
            }
            _bus = ModbusSerialMaster.CreateRtu(_serialPort);
            _bus.Transport.ReadTimeout = 200;
            _bus.Transport.WriteTimeout = 200;
            State = ModBusState.Wait;
        }
    }

    private static void ReadAndViewAllModules()
    {
        lock (Data)
        {
            Data.Clear();
            List<HomeModuleDataViewModel> modules = new();
            foreach (ModuleField field in _fields)
            {
                if (!field.Active)
                {
                    continue;
                }

                ushort? data = ReadInputRegisters(field.ModuleId, field.StartAddress);
                if (data != null)
                {
                    Data.Add(new ExperimentDataValue(field, data.Value));
                }

                string module = DataBaseService.GetModule(field.ModuleId).Name;
                modules.Add(new HomeModuleDataViewModel($"{module} - {field.FieldName}", data?.ToString() ?? "NULL", field.ValueName));
            }

            DataCollected?.Invoke(modules);
        }
    }

    public static void Exit()
    {
        _exit = true;
    }

    private static ushort? ReadInputRegisters(byte slaveId, ushort startAdd)
    {
        if (_bus == null)
        {
            return null;
        }

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
        if (_bus == null)
        {
            return null;
        }

        try
        {
            return _bus.ReadInputs(slaveId, startAdd, 1)[0];
        }
        catch (Exception)
        {
            return null;
        }
    }

    //public static void ModbusSerialRtuMasterWriteRegisters()
    //{
    //    using SerialPort port = new SerialPort("COM1");
    //    // configure serial port
    //    port.BaudRate = 9600;
    //    port.DataBits = 8;
    //    port.Parity = Parity.None;
    //    port.StopBits = StopBits.One;
    //    port.Open();

    //    // create modbus master
    //    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

    //    byte slaveId = 1;
    //    ushort startAddress = 100;
    //    ushort[] registers = new ushort[] { 1, 2, 65535 };

    //    // write three registers
    //    master.WriteMultipleRegisters(slaveId, startAddress, registers);
    //}

    //public static void ModbusSerialRtuMasterReadRegisters()
    //{
    //    using SerialPort port = new SerialPort("COM1");
    //    // configure serial port
    //    port.BaudRate = 9600;
    //    port.DataBits = 8;
    //    port.Parity = Parity.None;
    //    port.StopBits = StopBits.One;
    //    port.Open();

    //    // create modbus master
    //    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

    //    byte slaveId = 1;
    //    ushort startAddress = 1;

    //    // read three registers
    //    ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 8);
    //    //uint value = ModbusUtility.GetUInt32(registers[0], registers[1]);
    //}

    ///// <summary>
    /////     Write a 32 bit value.
    ///// </summary>
    //public static void ReadWrite32BitValue()
    //{
    //    using SerialPort port = new SerialPort("COM1");
    //    // configure serial port
    //    port.BaudRate = 9600;
    //    port.DataBits = 8;
    //    port.Parity = Parity.None;
    //    port.StopBits = StopBits.One;
    //    port.Open();

    //    // create modbus master
    //    ModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

    //    byte slaveId = 1;
    //    ushort startAddress = 1008;
    //    uint largeValue = ushort.MaxValue + 5;

    //    ushort lowOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 0);
    //    ushort highOrderValue = BitConverter.ToUInt16(BitConverter.GetBytes(largeValue), 2);

    //    // write large value in two 16 bit chunks
    //    master.WriteMultipleRegisters(slaveId, startAddress, new ushort[] { lowOrderValue, highOrderValue });

    //    // read large value in two 16 bit chunks and perform conversion
    //    ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, 2);
    //    uint value = ModbusUtility.GetUInt32(registers[1], registers[0]);
    //}
}
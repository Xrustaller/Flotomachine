using Modbus.Device;
using System;
using System.IO.Ports;
using System.Threading;
using Modbus.Utility;

namespace Flotomachine.Services;

public static class ModBusService
{
    private static Thread _thread;
    private static bool _exit;


    public static readonly int[] BaudRateList = { 4800, 9600, 19200, 38400, 57600, 115200, 230400 };
    public static readonly int[] PinRaspberryList = { 4, 5, 6, 12, 13, 17, 18, 22, 23, 24, 25, 26, 27 };

    private static string _serialPort;
    private static int _baudRate;

    public static ushort[] Data { get; private set; }

    public static Exception Initialize()
    {
        try
        {
            _serialPort = App.Settings.Configuration.Serial.Port;
            _baudRate = App.Settings.Configuration.Serial.BaudRate;
            _thread = new Thread(ThisThread);
            _thread.Name = "ModBusServise";
            _thread.Start();
        }
        catch (Exception e)
        {
            return e;
        }

        return null;
    }

    private static void ThisThread()
    {
        SerialPort serialPort = new(_serialPort);
        serialPort.BaudRate = _baudRate;
        serialPort.DataBits = 8;
        serialPort.StopBits = StopBits.One;
        serialPort.Parity = Parity.None;
        serialPort.Open();

        ModbusSerialMaster modbus = ModbusSerialMaster.CreateRtu(serialPort);
        modbus.Transport.ReadTimeout = 1000;
        modbus.Transport.WriteTimeout = 1000;


        // Должен пробегаться по всем устройствам
        // Если на главном таймере нажалась кнопка активации эксперимента, то запускает запись данных в базу 
        // Стандартный режим подразумевает считывание датчиков и вывод их на главный экран. 
        while (!_exit)
        {
            if (_serialPort != App.Settings.Configuration.Serial.Port || _baudRate != App.Settings.Configuration.Serial.BaudRate)
            {
                serialPort.Close();
                _serialPort = App.Settings.Configuration.Serial.Port;
                _baudRate = App.Settings.Configuration.Serial.BaudRate;

                serialPort = new(_serialPort);
                serialPort.BaudRate = _baudRate;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.Open();
                modbus = ModbusSerialMaster.CreateRtu(serialPort);
                modbus.Transport.ReadTimeout = 1000;
                modbus.Transport.WriteTimeout = 1000;
            }

            Read(1, 0, 8);
            Thread.Sleep(2000);
        }

        void Read(byte slaveId, ushort startAdd, ushort numOfPoints)
        {
            try
            {
                Data = modbus.ReadHoldingRegisters(slaveId, startAdd, numOfPoints);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void Write(int slaveId)
        {

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
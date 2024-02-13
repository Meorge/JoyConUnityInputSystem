using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Switch
{
    /// <summary>
    /// Set of addresses to use for SPI flash read/write.
    /// See https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/spi_flash_notes.md for more precisions on how to use it.
    /// </summary>
    /// <remarks>
    /// The length depends on what you want to retrieve, but the generic ones are noted next to the addresses below.
    /// </remarks>
    public enum SPIFlashReadAddressEnum : uint 
    {
        SerialNumber = 0x6000, // 0x10
        DeviceType = 0x6012, // 0x01
        ColorInfoExists = 0x601B, // 0x01
        FactoryIMUCalibration = 0x6020, // 0x18
        FactoryStickCalibration = 0x603D, // 0x12
        ColorData = 0x6050, // 0x0B
        UserStickCalibration = 0x8010, // 0x16
        UserIMUCalibration = 0x8026, // 0x1A
    }
    
    public enum BatteryLevelEnum
    {
        Full = 8,
        Medium = 6,
        Low = 4,
        Critical = 2,
        Empty = 0
    }

    public enum ControllerTypeEnum
    {
        JoyCon = 3,
        ProControllerOrChargingGrip = 0
    }

    public enum SpecificControllerTypeEnum : byte
    {
        Unknown = 0,
        LeftJoyCon = 1,
        RightJoyCon = 2,
        ProController = 3
    }
    
    public enum LEDStatusEnum {
        Off = 0,
        On = 0b0000_0001,
        Flashing = 0b0001_0000
    }
    
    public enum InputModeEnum
    {
        Standard = 0x30,
        NFCOrIR = 0x31,
        ReadSubcommands = 0x21,
        Simple = 0x3F
    }
    
    public enum SubcommandIDEnum : byte
    {
        GetOnlyControllerState = 0x00,
        BluetoothManualPairing = 0x01,
        RequestDeviceInfo = 0x02,
        SetInputReportMode = 0x03,
        TriggerButtonsElapsedTime = 0x04,
        GetPageListState = 0x05,
        SetHCIState = 0x06,
        ResetPairingInfo = 0x07,
        SetShipmentLowPowerState = 0x08,
        SPIFlashRead = 0x10,
        SPIFlashWrite = 0x11,
        SPISectorErase = 0x12,
        ResetNFCAndIRMCU = 0x20,
        SetNFCAndIRMCUConfig = 0x21,
        SetNFCAndIRMCUState = 0x22,
        Unknown_0x24 = 0x24,
        Unknown_0x25 = 0x25,
        Unknown_0x28 = 0x28,
        Get0x28NFCAndIRMCUData = 0x29,
        SetGPIOPinOutputValuePin2Port2 = 0x2A,
        Get0x29NFCAndIRMCUData = 0x2B,
        SetPlayerLights = 0x30,
        GetPlayerLights = 0x31,
        SetHOMELight = 0x38,
        EnableDisableIMU = 0x40,
        SetIMUSensitivity = 0x41,
        WriteIMURegisters = 0x42,
        ReadIMURegisters = 0x43,
        EnableDisableVibration = 0x48,
        GetRegulatedVoltage = 0x50,
        SetGPIOPinOutputValuePins7And15Port1 = 0x51,
        GetGPIOPinInputOutputValue = 0x52
    }
}

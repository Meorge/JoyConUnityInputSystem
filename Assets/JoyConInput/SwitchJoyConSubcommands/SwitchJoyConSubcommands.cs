using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System.Collections;

[StructLayout(LayoutKind.Sequential)]
public struct SwitchJoyConBaseSubcommandStruct
{
    public byte subcommandId;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] arguments;
}

public class SwitchJoyConBaseSubcommand
{
    public virtual byte SubcommandID { get; }

    public SwitchJoyConBaseSubcommandStruct GetSubcommand()
    {
        return new SwitchJoyConBaseSubcommandStruct {
            subcommandId = SubcommandID,
            arguments = GetArguments()
        };
    }

    protected virtual byte[] GetArguments() => new byte[0x1];
}

public class SwitchJoyConEmptySubcommand : SwitchJoyConBaseSubcommand
{
    public override byte SubcommandID => (byte)SwitchJoyConSubcommandID.GetOnlyControllerState;
}

public enum SwitchJoyConSubcommandID
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
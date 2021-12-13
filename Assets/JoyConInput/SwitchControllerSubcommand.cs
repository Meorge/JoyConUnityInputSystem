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

namespace UnityEngine.InputSystem.Switch
{
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    public unsafe struct SwitchControllerBaseSubcommandStruct
    {
        [FieldOffset(0)]
        public byte subcommandId;

        [FieldOffset(1)]
        public fixed byte arguments[0x40 - 10 - 1];
    }
    
    public class SwitchControllerBaseSubcommand
    {
        public virtual byte SubcommandID { get; }

        public unsafe SwitchControllerBaseSubcommandStruct GetSubcommand()
        {
            var subcommand = new SwitchControllerBaseSubcommandStruct
            {
                subcommandId = SubcommandID
            };

            IntPtr ptr = new IntPtr((void*)subcommand.arguments);

            var args = GetArguments();
            Marshal.Copy(args, 0, ptr, args.Length);

            return subcommand;
        }

        protected virtual byte[] GetArguments() => new byte[0x1] { 0x00 };
    }
    
    public class SwitchControllerEmptySubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.GetOnlyControllerState;
    }
    
    public class SwitchControllerRequestInfoSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.RequestDeviceInfo;
    }
    
    public class SwitchControllerBluetoothManualPairingSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.BluetoothManualPairing;

        public byte ValueByte = 0x01;
        protected override byte[] GetArguments() => new byte[0x1] { ValueByte };
    }
    
    public class SwitchControllerSetImuEnabledSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.EnableDisableIMU;

        public bool Enabled = true;
        protected override byte[] GetArguments() => new byte[0x1] { (byte)(Enabled ? 0x01 : 0x00) };
    }
    
    public class SwitchControllerSetVibrationEnabledSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.EnableDisableVibration;

        public bool Enabled = true;
        protected override byte[] GetArguments() => new byte[0x1] { (byte)(Enabled ? 0x01 : 0x00) };
    }
    
    public class SwitchControllerReadSPIFlashSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.SPIFlashRead;

        public uint Address = 0x0;
        public byte Length = 0x0;

        protected override byte[] GetArguments()
        {
            var addrAsBytes = BitConverter.GetBytes(Address);

            byte[] output = new byte[5];

            Array.Copy(addrAsBytes, output, 4);
            output[4] = Length;
            return output;
        }

        public SwitchControllerReadSPIFlashSubcommand(uint atAddress = 0x0, byte withLength = 0x1)
        {
            Address = atAddress;
            Length = withLength;
        }
    }
    
    public class SwitchControllerSetLEDSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SwitchControllerSubcommandIDEnum.SetPlayerLights;

        public SwitchControllerLEDStatusEnum Player1LED = SwitchControllerLEDStatusEnum.Off;
        public SwitchControllerLEDStatusEnum Player2LED = SwitchControllerLEDStatusEnum.Off;
        public SwitchControllerLEDStatusEnum Player3LED = SwitchControllerLEDStatusEnum.Off;
        public SwitchControllerLEDStatusEnum Player4LED = SwitchControllerLEDStatusEnum.Off;

        public SwitchControllerSetLEDSubcommand(
            SwitchControllerLEDStatusEnum p1 = SwitchControllerLEDStatusEnum.Off,
            SwitchControllerLEDStatusEnum p2 = SwitchControllerLEDStatusEnum.Off,
            SwitchControllerLEDStatusEnum p3 = SwitchControllerLEDStatusEnum.Off,
            SwitchControllerLEDStatusEnum p4 = SwitchControllerLEDStatusEnum.Off)
        {
            Player1LED = p1;
            Player2LED = p2;
            Player3LED = p3;
            Player4LED = p4;
        }

        protected override byte[] GetArguments()
        {
            return new byte[0x1] { (byte)((byte)Player1LED | (byte)Player2LED << 1 | (byte)Player3LED << 2 | (byte)Player4LED << 3) };
        }
    }

    public enum SwitchControllerLEDStatusEnum {
        Off = 0,
        On = 0b0000_0001,
        Flashing = 0b0001_0000
    }
    
    public enum SwitchControllerInputModeEnum
    {
        Standard = 0x30,
        NFCOrIR = 0x31,
        ReadSubcommands = 0x21,
        Simple = 0x3F
    }

    public class SwitchControllerInputModeSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte) SwitchControllerSubcommandIDEnum.SetInputReportMode;
        public SwitchControllerInputModeEnum InputMode = SwitchControllerInputModeEnum.Standard;

        protected override byte[] GetArguments() => new byte[1] {(byte) InputMode};

        public SwitchControllerInputModeSubcommand(SwitchControllerInputModeEnum mode)
        {
            InputMode = mode;
        }
    }
    
    public enum SwitchControllerSubcommandIDEnum : byte
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
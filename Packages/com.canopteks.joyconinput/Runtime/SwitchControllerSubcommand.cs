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
    //* For more clarity, read this first: https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/bluetooth_hid_notes.md

    #region Base data types
    /// <summary>
    /// Contiguous data structure that will be sent to the controller through 0x01 output report.
    /// </summary>
    /// <remarks>
    /// It will be filled with different stuff or different sizes based on the subcommand used.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 39)]
    public unsafe struct SwitchControllerBaseSubcommandStruct
    {
        [FieldOffset(0)]
        public byte subcommandId;

        [FieldOffset(1)]
        public fixed byte arguments[38];
    }

    /// <summary>
    /// Wrapper class to formulate this mumbo-jumbo more easily through C#/Unity. See <see cref="SwitchControllerBaseSubcommandStruct"/>
    /// </summary>        
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
    #endregion

    #region Subcommand variants
    public class SwitchControllerEmptySubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.GetOnlyControllerState;
    }
    
    public class SwitchControllerRequestInfoSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.RequestDeviceInfo;
    }
    
    public class SwitchControllerBluetoothManualPairingSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.BluetoothManualPairing;

        public byte ValueByte = 0x01;
        protected override byte[] GetArguments() => new byte[0x1] { ValueByte };
    }
    
    public class SwitchControllerSetImuEnabledSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.EnableDisableIMU;

        public bool Enabled = true;
        protected override byte[] GetArguments() => new byte[0x1] { (byte)(Enabled ? 0x01 : 0x00) };
    }
    
    public class SwitchControllerSetVibrationEnabledSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.EnableDisableVibration;

        public bool Enabled = true;
        protected override byte[] GetArguments() => new byte[0x1] { (byte)(Enabled ? 0x01 : 0x00) };
    }
    
    public class SwitchControllerReadSPIFlashSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte)SubcommandIDEnum.SPIFlashRead;

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
        public override byte SubcommandID => (byte)SubcommandIDEnum.SetPlayerLights;

        public LEDStatusEnum Player1LED = LEDStatusEnum.Off;
        public LEDStatusEnum Player2LED = LEDStatusEnum.Off;
        public LEDStatusEnum Player3LED = LEDStatusEnum.Off;
        public LEDStatusEnum Player4LED = LEDStatusEnum.Off;

        public SwitchControllerSetLEDSubcommand(
            LEDStatusEnum p1 = LEDStatusEnum.Off,
            LEDStatusEnum p2 = LEDStatusEnum.Off,
            LEDStatusEnum p3 = LEDStatusEnum.Off,
            LEDStatusEnum p4 = LEDStatusEnum.Off)
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

    public class SwitchControllerInputModeSubcommand : SwitchControllerBaseSubcommand
    {
        public override byte SubcommandID => (byte) SubcommandIDEnum.SetInputReportMode;
        public InputModeEnum InputMode = InputModeEnum.Standard;

        protected override byte[] GetArguments() => new byte[1] {(byte) InputMode};

        public SwitchControllerInputModeSubcommand(InputModeEnum mode)
        {
            InputMode = mode;
        }
    }
    #endregion
}
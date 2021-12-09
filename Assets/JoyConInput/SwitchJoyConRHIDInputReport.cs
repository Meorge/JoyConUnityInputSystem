using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Switch.LowLevel
{
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    unsafe struct SwitchControllerFullInputReport
    {
        public const int kSize = 362;
        public const byte ExpectedReportId = 0x30;

        [FieldOffset(0)] public byte reportId;
        [FieldOffset(1)] public byte timer;
        [FieldOffset(2)] public byte batteryAndConnectionInfo;

        [FieldOffset(3)] public byte rightButtons;

        [FieldOffset(4)] public byte sharedButtons;

        [FieldOffset(5)] public byte leftButtons;

        [FieldOffset(6)] public fixed byte leftStick[3];
        [FieldOffset(9)] public fixed byte rightStick[3];

        [FieldOffset(12)] public byte vibratorInputReport;

        // For 0x21 (Subcommand replies)
        [FieldOffset(13)] public byte subcommandAck;
        [FieldOffset(14)] public byte subcommandReplyId;
        [FieldOffset(15)] public fixed byte subcommandReplyData[35];


        // For 0x23 (NFC/IR MCU FW)
        [FieldOffset(13)] public fixed byte nfcIRMCUFWDataInputReport[37];

        // For 0x30, 0x31, 0x32, 0x33 (normal mode)
        [FieldOffset(13)] public IMUData imuData0ms;
        [FieldOffset(25)] public IMUData imuData5ms;
        [FieldOffset(37)] public IMUData imuData10ms;

        // For 0x31 (NFC/IR?)
        [FieldOffset(49)] public fixed byte nfcIRDataInputReport[313];


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SwitchControllerVirtualInputState ToHIDInputReport(ref SwitchControllerHID.StickCalibrationData lStickCalibData, ref SwitchControllerHID.StickCalibrationData rStickCalibData)
        {
            // Left analog stick data
            var l0 = leftStick[0];
            var l1 = leftStick[1];
            var l2 = leftStick[2];
            var rawLeftStickHoriz = l0 | ((l1 & 0xF) << 8);
            var rawLeftStickVert = (l1 >> 4) | (l2 << 4);

            var leftStickX = (Mathf.InverseLerp(lStickCalibData.xMin, lStickCalibData.xMax, rawLeftStickHoriz) * 2) - 1;
            var leftStickY = (Mathf.InverseLerp(lStickCalibData.yMin, lStickCalibData.yMax, rawLeftStickVert) * 2) - 1;

            // Right analog stick data
            var r0 = rightStick[0];
            var r1 = rightStick[1];
            var r2 = rightStick[2];
            var rawRightStickHoriz = r0 | ((r1 & 0xF) << 8);
            var rawRightStickVert = (r1 >> 4) | (r2 << 4);

            var rightStickX = (Mathf.InverseLerp(rStickCalibData.xMin, rStickCalibData.xMax, rawRightStickHoriz) * 2) - 1;
            var rightStickY = (Mathf.InverseLerp(rStickCalibData.yMin, rStickCalibData.yMax, rawRightStickVert) * 2) - 1;

            var state = new SwitchControllerVirtualInputState
            {
                leftStickX = leftStickX,
                leftStickY = leftStickY,
                rightStickX = rightStickX,
                rightStickY = rightStickY
            };

            state.Set(SwitchControllerVirtualInputState.Button.Y, (rightButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.X, (rightButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.B, (rightButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.A, (rightButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.SR, ((leftButtons | rightButtons) & 0x10) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.SL, ((leftButtons | rightButtons) & 0x20) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.R, (rightButtons & 0x40) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.ZR, (rightButtons & 0x80) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Minus, (sharedButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Plus, (sharedButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.StickR, (sharedButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.StickL, (sharedButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Down, (leftButtons & 0x01) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Up, (leftButtons & 0x02) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Right, (leftButtons & 0x04) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.Left, (leftButtons & 0x08) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.L, (leftButtons & 0x40) != 0);
            state.Set(SwitchControllerVirtualInputState.Button.ZL, (leftButtons & 0x80) != 0);

            return state;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 96)]
    struct IMUData
    {
        [FieldOffset(0)] short accelX;
        [FieldOffset(2)] short accelY;
        [FieldOffset(4)] short accelZ;

        [FieldOffset(6)] short gyro1;
        [FieldOffset(8)] short gyro2;
        [FieldOffset(10)] short gyro3;
    }
}
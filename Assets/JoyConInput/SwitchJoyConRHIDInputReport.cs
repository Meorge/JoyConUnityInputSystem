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

[StructLayout(LayoutKind.Explicit, Size = 362)]
unsafe struct SwitchJoyConRHIDInputState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    [FieldOffset(0)] public byte reportId;
    [FieldOffset(1)] public byte timer;
    [FieldOffset(2)] public byte batteryAndConnectionInfo;


    [InputControl(name = "buttonNorthR", displayName = "Y", layout = "Button", format = "BIT", bit = 0)]
    [InputControl(name = "buttonEastR", displayName = "X", layout = "Button", format = "BIT", bit = 1)]
    [InputControl(name = "buttonWestR", displayName = "B", layout = "Button", format = "BIT", bit = 2)]
    [InputControl(name = "buttonSouthR", displayName = "A", layout = "Button", format = "BIT", bit = 3)]
    [InputControl(name = "srR", displayName = "SR", layout = "Button", format = "BIT", bit = 4)]
    [InputControl(name = "slR", displayName = "SL", layout = "Button", format = "BIT", bit = 5)]
    [InputControl(name = "r", displayName = "R", layout = "Button", format = "BIT", bit = 6)]
    [InputControl(name = "zr", displayName = "ZR", layout = "Button", format = "BIT", bit = 7)]
    [FieldOffset(3)]
    public byte rightButtons;

    [InputControl(name = "minus", displayName = "Minus", layout = "Button", format = "BIT", bit = 0)]
    [InputControl(name = "plus", displayName = "Plus", layout = "Button", format = "BIT", bit = 1)]
    [InputControl(name = "stickPressR", displayName = "Right Stick Press", layout = "Button", format = "BIT", bit = 2)]
    [InputControl(name = "stickPressL", displayName = "Left Stick Press", layout = "Button", format = "BIT", bit = 3)]
    [InputControl(name = "home", displayName = "HOME", layout = "Button", format = "BIT", bit = 4)]
    [InputControl(name = "capture", displayName = "Capture", layout = "Button", format = "BIT", bit = 5)]
    [InputControl(name = "chargingGrip", displayName = "Charging Grip", layout = "Button", format = "BIT", bit = 7)]
    [FieldOffset(4)]
    public byte sharedButtons;

    [InputControl(name = "buttonNorthL", displayName = "Down", layout = "Button", format = "BIT", bit = 0)]
    [InputControl(name = "buttonEastL", displayName = "Up", layout = "Button", format = "BIT", bit = 1)]
    [InputControl(name = "buttonWestL", displayName = "Right", layout = "Button", format = "BIT", bit = 2)]
    [InputControl(name = "buttonSouthL", displayName = "Left", layout = "Button", format = "BIT", bit = 3)]
    [InputControl(name = "srL", displayName = "SR", layout = "Button", format = "BIT", bit = 4)]
    [InputControl(name = "slL", displayName = "SL", layout = "Button", format = "BIT", bit = 5)]
    [InputControl(name = "l", displayName = "L", layout = "Button", format = "BIT", bit = 6)]
    [InputControl(name = "zl", displayName = "ZL", layout = "Button", format = "BIT", bit = 7)]
    [FieldOffset(5)]
    public byte leftButtons;

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

    // TODO: I need to get stick controls in here, but trying to add another field causes Unity to crash...

    // TODO: figure out how to force the size of this struct without doing this
    [InputControl(name = "makeItBig", displayName = "big", layout = "Button", format = "BIT", bit = 0)]
    [FieldOffset(361)] public byte ho;
}

[StructLayout(LayoutKind.Explicit, Size = 96)]
struct IMUData {
    [FieldOffset(0)] short accelX;
    [FieldOffset(2)] short accelY;
    [FieldOffset(4)] short accelZ;

    [FieldOffset(6)] short gyro1;
    [FieldOffset(8)] short gyro2;
    [FieldOffset(10)] short gyro3;
}
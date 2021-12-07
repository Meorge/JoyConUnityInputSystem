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

[StructLayout(LayoutKind.Explicit, Size = 50)]
unsafe struct SwitchJoyConRHIDInputState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    [InputControl(name = "reportId", displayName = "Report ID", layout = "Integer", format = "BYTE")]
    [FieldOffset(0)] public byte reportId;

    // [InputControl(name = "timer", displayName = "Timer", layout = "Integer", format = "BYTE")]
    [FieldOffset(1)]
    public byte timer;

    [InputControl(name = "batteryLevel", displayName = "Battery Level", layout = "Integer", format = "BIT", sizeInBits = 3, bit = 0)]
    [InputControl(name = "batteryIsCharging", displayName = "Battery is Charging", layout = "Button", format = "BIT", bit = 3)]
    [InputControl(name = "connectionInfo", displayName = "Connection Information", layout = "Integer", format = "BIT", sizeInBits = 4, bit = 4)]
    [FieldOffset(2)]
    public byte timerAndConnectionInfo;


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
    [InputControl(name = "rStickPress", displayName = "Right Stick Press", layout = "Button", format = "BIT", bit = 2)]
    [InputControl(name = "lStickPress", displayName = "Left Stick Press", layout = "Button", format = "BIT", bit = 3)]
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


    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(6)] public byte leftStick1;
    [FieldOffset(7)] public byte leftStick2;
    [FieldOffset(8)] public byte leftStick3;


    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(9)] public byte rightStick1;
    [FieldOffset(10)] public byte rightStick2;
    [FieldOffset(11)] public byte rightStick3;


    [FieldOffset(12)] public byte vibratorInputReport;


    // [InputControl(name = "field13", displayName = "Field 13 (Unknown)", layout = "Integer", format = "BYTE")]
    // [FieldOffset(13)]
    // public byte field13;

    // [InputControl(name = "gyroY", displayName = "Gyro Y?", layout = "Integer", format = "BYTE")]
    // [FieldOffset(14)]
    // public byte gyroY;

    // [InputControl(name = "field15", displayName = "Field 15 (Unknown)", layout = "Integer", format = "BYTE")]
    // [FieldOffset(15)]
    // public byte field15;

    // [InputControl(name = "gyroX", displayName = "Gyro X?", layout = "Integer", format = "BYTE")]
    // [FieldOffset(16)]
    // public byte gyroX;

    // [InputControl(name = "field17", displayName = "Field 17 (Unknown)", layout = "Integer", format = "BYTE")]
    // [FieldOffset(17)]
    // public byte field17;

    // [InputControl(name = "gyroX2", displayName = "Gyro X2?", layout = "Integer", format = "BYTE")]
    // [FieldOffset(18)]
    // public byte gyroX_2;

    // [InputControl(name = "field19", displayName = "Field 19 (Unknown)", layout = "Integer", format = "BYTE")]
    // [FieldOffset(19)]
    // public byte field19;


    // [InputControl(name = "field20", displayName = "Field 20 (Unknown)", layout = "Integer", format = "BYTE")]
    // [FieldOffset(20)]
    // public byte field20;

    [InputControl(name = "subcommandAck", displayName = "Subcommand Acknowledgement", layout = "Integer", format = "BYTE")]
    [FieldOffset(13)]
    public byte subcommandAck;

    [InputControl(name = "subcommandReplyId", displayName = "Subcommand Reply ID", layout = "Integer", format = "BYTE")]
    [FieldOffset(14)]
    public byte subcommandReplyId;

    // [InputControl(name = "subcommandReply", displayName = "Subcommand Reply Data", layout = "Integer", format = "BYTE", arraySize = 35)]
    // [FieldOffset(15)] public fixed byte subcommandReplyData[35]; // TODO: make it an array

    [InputControl(name = "firmwareVersion", displayName = "Firmware Version", layout = "Integer", format = "USHT")]
    [FieldOffset(15)]
    public ushort firmwareVersion;

    [InputControl(name = "controllerType", displayName = "Controller Type", layout = "Integer", format = "BYTE")]
    [FieldOffset(15 + 2)]
    public byte controllerType;

    [InputControl(name = "shouldBe02", displayName = "Should be 02", layout = "Integer", format = "BYTE")]
    [FieldOffset(15 + 3)]
    public byte shouldBe02;


    
    [InputControl(name = "shouldBe01", displayName = "Should be 01", layout = "Integer", format = "BYTE")]
    [FieldOffset(15 + 10)]
    public byte shouldBe01;

    [InputControl(name = "useSPIColors", displayName = "Use Colors from SPI?", layout = "Integer", format = "BYTE")]
    [FieldOffset(15 + 11)]
    public byte useSPIColors;
}


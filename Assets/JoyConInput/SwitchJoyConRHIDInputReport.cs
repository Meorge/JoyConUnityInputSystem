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


    [FieldOffset(9)] public fixed byte rightStick[3];

    // [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    // [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    // [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    // [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    // [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    // [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    // [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    // [FieldOffset(6)] public byte leftStick1;
    // [FieldOffset(7)] public byte leftStick2;
    // [FieldOffset(8)] public byte leftStick3;


    // [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    // [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    // [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    // [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    // [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    // [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    // [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    // [FieldOffset(9)] public byte rightStick1;
    // [FieldOffset(10)] public byte rightStick2;
    // [FieldOffset(11)] public byte rightStick3;


    // [FieldOffset(12)] public byte vibratorInputReport;

    // [FieldOffset(13)] public byte subcommandAck;

    // [FieldOffset(14)] public byte subcommandReplyId;

    // [FieldOffset(15)] public ushort firmwareVersion;

    // [FieldOffset(15 + 2)] public byte controllerType;

    // [FieldOffset(15 + 3)] public byte shouldBe02;
    // [FieldOffset(15 + 10)] public byte shouldBe01;

    // [FieldOffset(15 + 11)] public byte useSPIColors;


    // TODO: figure out how to force the size of this struct without doing this
    [InputControl(name = "makeItBig", displayName = "big", layout = "Button", format = "BIT", bit = 0)]
    [FieldOffset(361)] public byte ho;
}

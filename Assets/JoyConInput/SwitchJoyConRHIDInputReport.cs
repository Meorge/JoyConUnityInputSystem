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
struct SwitchJoyConRHIDInputState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');
    [FieldOffset(0)] public byte reportId;

    [InputControl(name = "plus", displayName = "+", layout = "Button", bit = 1)]
    [InputControl(name = "stickPress", displayName = "Stick Press", layout = "Button", bit = 3)]
    [InputControl(name = "home", displayName = "Home", layout = "Button", bit = 4)]
    [InputControl(name = "r", displayName = "R", layout = "Button", bit = 6)]
    [InputControl(name = "zr", displayName = "ZR", layout = "Button", bit = 7)]
    [FieldOffset(2)] public byte triggers;

    // Main buttons
    [InputControl(name = "buttonSouth", displayName = "A", layout = "Button", format = "BIT", bit = 0)]
    [InputControl(name = "buttonEast", displayName = "X", layout = "Button", format = "BIT", bit = 1)]
    [InputControl(name = "buttonWest", displayName = "B", layout = "Button", format = "BIT", bit = 2)]
    [InputControl(name = "buttonNorth", displayName = "Y", layout = "Button", format = "BIT", bit = 3)]
    [InputControl(name = "sl", displayName = "SL", layout = "Button", format = "BIT", bit = 4)]
    [InputControl(name = "sr", displayName = "SR", layout = "Button", format = "BIT", bit = 5)]
    [FieldOffset(1)] public byte buttons;


    [InputControl(name = "hat", displayName = "HatSwitch", layout = "Dpad", format = "BIT", sizeInBits = 4, bit = 0)]
    [InputControl(name = "hat/up", displayName = "HatSwitch Up", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", format = "BIT", sizeInBits = 4, bit = 0)]
    [InputControl(name = "hat/right", displayName = "HatSwitch Right", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", format = "BIT", sizeInBits = 4, bit = 0)]
    [InputControl(name = "hat/down", displayName = "HatSwitch Down", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", format = "BIT", sizeInBits = 4, bit = 0)]
    [InputControl(name = "hat/left", displayName = "HatSwitch Left", layout = "DiscreteButton", parameters = "minValue=5,maxValue=7", format = "BIT", sizeInBits = 4, bit = 0)]
    [FieldOffset(3)] public byte stick;
}


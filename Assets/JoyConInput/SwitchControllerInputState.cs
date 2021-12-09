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

// Using https://github.com/Unity-Technologies/InputSystem/blob/67a8605dc8d2bb67d251117cbe0e371d043e7a13/Packages/com.unity.inputsystem/InputSystem/Plugins/Switch/SwitchProControllerHID.cs
// as a base here
namespace UnityEngine.InputSystem.Switch.LowLevel
{
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA
    [StructLayout(LayoutKind.Explicit, Size = 6)]
    internal struct SwitchControllerVirtualInputState : IInputStateTypeInfo
    {
        // Switch Controller Virtual State
        public static FourCC Format = new FourCC("SCVS");
        public FourCC format => Format;

        [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        [FieldOffset(0)] public byte leftStickX;
        [FieldOffset(1)] public byte leftStickY;

        [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        [FieldOffset(2)] public byte rightStickX;
        [FieldOffset(3)] public byte rightStickY;

        [InputControl(name = "dpad", format = "BIT", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/up", bit = (int)Button.Up)]
        [InputControl(name = "dpad/right", bit = (int)Button.Right)]
        [InputControl(name = "dpad/down", bit = (int)Button.Down)]
        [InputControl(name = "dpad/left", bit = (int)Button.Left)]
        [InputControl(name = "buttonWest", displayName = "Y", shortDisplayName = "Y", bit = (int)Button.Y, usage = "SecondaryAction")]
        [InputControl(name = "buttonNorth", displayName = "X", shortDisplayName = "X", bit = (int)Button.X)]
        [InputControl(name = "buttonSouth", displayName = "B", shortDisplayName = "B", bit = (int)Button.B, usage = "Back")]
        [InputControl(name = "buttonEast", displayName = "A", shortDisplayName = "A", bit = (int)Button.A, usage = "PrimaryAction")]
        [InputControl(name = "leftShoulder", displayName = "L", shortDisplayName = "L", bit = (uint)Button.L)]
        [InputControl(name = "rightShoulder", displayName = "R", shortDisplayName = "R", bit = (uint)Button.R)]
        [InputControl(name = "leftStickPress", displayName = "Left Stick", bit = (uint)Button.StickL)]
        [InputControl(name = "rightStickPress", displayName = "Right Stick", bit = (uint)Button.StickR)]
        [InputControl(name = "leftTrigger", displayName = "ZL", shortDisplayName = "ZL", format = "BIT", bit = (uint)Button.ZL)]
        [InputControl(name = "rightTrigger", displayName = "ZR", shortDisplayName = "ZR", format = "BIT", bit = (uint)Button.ZR)]
        [InputControl(name = "start", displayName = "Plus", bit = (uint)Button.Plus, usage = "Menu")]
        [InputControl(name = "select", displayName = "Minus", bit = (uint)Button.Minus)]
        [FieldOffset(4)] public ushort buttons;

        public enum Button
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,

            West = 4,
            North = 5,
            South = 6,
            East = 7,

            L = 8,
            R = 9,
            StickL = 10,
            StickR = 11,

            ZL = 12,
            ZR = 13,
            Plus = 14,
            Minus = 15,

            X = North,
            B = South,
            Y = West,
            A = East,
        }
    }
    #endif
}
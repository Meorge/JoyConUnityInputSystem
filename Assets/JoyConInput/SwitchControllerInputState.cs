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

// Using https://github.com/Unity-Technologies/InputSystem/blob/67a8605dc8d2bb67d251117cbe0e371d043e7a13/Packages/com.unity.inputsystem/InputSystem/Plugins/Switch/SwitchProControllerHID.cs
// as a base here
namespace UnityEngine.InputSystem.Switch.LowLevel
{
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SwitchControllerVirtualInputState : IInputStateTypeInfo
    {
        // Switch Controller Virtual State
        public static FourCC Format = new FourCC("SCVS");
        public FourCC format => Format;

        [InputControl(name = "leftStick", layout = "Stick", format = "VEC2")]
        // [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        // [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        // [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
        // [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        // [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        // [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        public Vector2 leftStick;


        [InputControl(name = "rightStick", layout = "Stick", format = "VEC2")]
        // [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        // [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        // [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        // [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
        // [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
        // [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
        public Vector2 rightStick;


        [InputControl(name = "dpad", layout = "Dpad", format = "BIT", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/up", layout = "Button", bit = (int)Button.Up)]
        [InputControl(name = "dpad/right", layout = "Button", bit = (int)Button.Right)]
        [InputControl(name = "dpad/down", layout = "Button", bit = (int)Button.Down)]
        [InputControl(name = "dpad/left", layout = "Button", bit = (int)Button.Left)]
        [InputControl(name = "buttonWest", displayName = "Y", shortDisplayName = "Y", layout = "Button", bit = (int)Button.Y, usage = "SecondaryAction")]
        [InputControl(name = "buttonNorth", displayName = "X", shortDisplayName = "X", layout = "Button", bit = (int)Button.X)]
        [InputControl(name = "buttonSouth", displayName = "B", shortDisplayName = "B", layout = "Button", bit = (int)Button.B, usage = "Back")]
        [InputControl(name = "buttonEast", displayName = "A", shortDisplayName = "A", layout = "Button", bit = (int)Button.A, usage = "PrimaryAction")]
        [InputControl(name = "leftShoulder", displayName = "L", shortDisplayName = "L", layout = "Button", bit = (uint)Button.L)]
        [InputControl(name = "rightShoulder", displayName = "R", shortDisplayName = "R", layout = "Button", bit = (uint)Button.R)]
        [InputControl(name = "leftStickPress", displayName = "Left Stick", layout = "Button", bit = (uint)Button.StickL)]
        [InputControl(name = "rightStickPress", displayName = "Right Stick", layout = "Button", bit = (uint)Button.StickR)]
        [InputControl(name = "leftTrigger", displayName = "ZL", shortDisplayName = "ZL", format = "BIT", layout = "Button", bit = (uint)Button.ZL)]
        [InputControl(name = "rightTrigger", displayName = "ZR", shortDisplayName = "ZR", format = "BIT", layout = "Button", bit = (uint)Button.ZR)]
        [InputControl(name = "start", displayName = "Plus", layout = "Button", bit = (uint)Button.Plus, usage = "Menu")]
        [InputControl(name = "select", displayName = "Minus", layout = "Button", bit = (uint)Button.Minus)]
        [InputControl(name = "leftShoulderMini", displayName = "SL", shortDisplayName = "SL", format = "BIT", layout = "Button", bit = (uint)Button.SL)]
        [InputControl(name = "rightShoulderMini", displayName = "SR", shortDisplayName = "SR", format = "BIT", layout = "Button", bit = (uint)Button.SR)]
        public uint buttons;


        [InputControl(name = "acceleration", layout = "Vector3", format = "VEC3", noisy = true)]
        public Vector3 acceleration;

        [InputControl(name = "gyroscope", layout = "Vector3", format = "VEC3", noisy = true)]
        public Vector3 gyroscope;
 
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
            SL = 16,
            SR = 17,

            X = North,
            B = South,
            Y = West,
            A = East,
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Button button, bool state)
        {
            Debug.Assert((int)button < 32, $"Expected button < 32, so we fit into the 32 bit wide bitmask");
            var bit = (uint)(1U << (int)button);
            if (state)
                buttons = (uint)(buttons | bit);
            else
                buttons &= (uint)~bit;
        }
    }
    #endif
}
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

public enum SwitchJoyConInputMode
{
    Standard = 0x30,
    NFCOrIR = 0x31,
    ReadSubcommands = 0x21,
    Simple = 0x3F
}

public class SwitchJoyConInputModeSubcommand : SwitchJoyConBaseSubcommand
{
    public override byte SubcommandID => (byte)SwitchJoyConSubcommandID.SetInputReportMode;
    public SwitchJoyConInputMode InputMode = SwitchJoyConInputMode.Standard;

    protected override byte[] GetArguments() => new byte[1] { (byte)InputMode };

    public SwitchJoyConInputModeSubcommand(SwitchJoyConInputMode mode)
    {
        InputMode = mode;
    }
}
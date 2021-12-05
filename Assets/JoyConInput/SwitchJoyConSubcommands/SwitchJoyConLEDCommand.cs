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

public class SwitchJoyConLEDSubcommand : SwitchJoyConBaseSubcommand
{
    public override byte SubcommandID => (byte)SwitchJoyConSubcommandID.SetPlayerLights;

    public SwitchJoyConLEDStatus Player1LED = SwitchJoyConLEDStatus.Off;
    public SwitchJoyConLEDStatus Player2LED = SwitchJoyConLEDStatus.Off;
    public SwitchJoyConLEDStatus Player3LED = SwitchJoyConLEDStatus.Off;
    public SwitchJoyConLEDStatus Player4LED = SwitchJoyConLEDStatus.Off;

    public SwitchJoyConLEDSubcommand(
        SwitchJoyConLEDStatus p1 = SwitchJoyConLEDStatus.Off,
        SwitchJoyConLEDStatus p2 = SwitchJoyConLEDStatus.Off,
        SwitchJoyConLEDStatus p3 = SwitchJoyConLEDStatus.Off,
        SwitchJoyConLEDStatus p4 = SwitchJoyConLEDStatus.Off)
    {
        Player1LED = p1;
        Player2LED = p2;
        Player3LED = p3;
        Player4LED = p4;
    }

    protected override byte[] GetArguments()
    {
        var b = new byte[0x10];
        Array.Clear(b, 0, b.Length);
        b[0] = (byte)((byte)Player1LED | (byte)Player2LED << 1 | (byte)Player3LED << 2 | (byte)Player4LED << 3);
        return b;
    }
}

public enum SwitchJoyConLEDStatus {
    Off = 0,
    On = 0b0000_0001,
    Flashing = 0b0001_0000
}
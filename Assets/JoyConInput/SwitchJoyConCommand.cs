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

[StructLayout(LayoutKind.Sequential)]
internal struct SwitchJoyConCommand : IInputDeviceCommandInfo
{
    private static byte globalNumber = 0x0;

    public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
    public FourCC typeStatic => Type;

    internal const int kSize = 0x40;

    public InputDeviceCommand baseCommand;
    public byte first;
    public byte globalCount;

    public SwitchJoyConRumbleData rumbleData;

    public SwitchJoyConBaseSubcommandStruct subcommand;

    
    public static SwitchJoyConCommand Create(SwitchJoyConRumbleProfile rumbleProfile)
    {

        var rumbleData = new SwitchJoyConRumbleData
        {
            leftJoyConRumble = SwitchJoyConRumbleAmpFreqData.Create(
                highBandFrequency: rumbleProfile.highBandFrequencyL,
                highBandAmplitude: rumbleProfile.highBandAmplitudeL,
                lowBandFrequency: rumbleProfile.lowBandFrequencyL,
                lowBandAmplitude: rumbleProfile.lowBandAmplitudeL
            ),
            rightJoyConRumble = SwitchJoyConRumbleAmpFreqData.Create(
                highBandFrequency: rumbleProfile.highBandFrequencyR,
                highBandAmplitude: rumbleProfile.highBandAmplitudeR,
                lowBandFrequency: rumbleProfile.lowBandFrequencyR,
                lowBandAmplitude: rumbleProfile.lowBandAmplitudeR
            )
        };

        var subcommand = new SwitchJoyConLEDSubcommand(
                p1: SwitchJoyConLEDStatus.Off,
                p2: SwitchJoyConLEDStatus.On,
                p3: SwitchJoyConLEDStatus.Off,
                p4: SwitchJoyConLEDStatus.Off
            ).GetSubcommand();

        return new SwitchJoyConCommand
        {
            baseCommand = new InputDeviceCommand(Type, kSize),
            first = 0x01,
            globalCount = globalNumber++,
            rumbleData = rumbleData,
            subcommand = subcommand
        };
    }
}
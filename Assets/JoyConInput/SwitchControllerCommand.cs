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

[StructLayout(LayoutKind.Explicit, Size = kSize)]
internal unsafe struct SwitchControllerCommand : IInputDeviceCommandInfo
{
    private static byte globalNumber = 0x0;

    public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
    public FourCC typeStatic => Type;

    internal const int id = 0;
    internal const int kSize = InputDeviceCommand.BaseCommandSize + 0x40;

    [FieldOffset(0)]
    public InputDeviceCommand baseCommand;


    [FieldOffset(InputDeviceCommand.BaseCommandSize)]
    public byte first;
    
    
    [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)]
    public byte globalCount;
    
    
    [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)]
    public SwitchJoyConRumbleData rumbleData;


    [FieldOffset(InputDeviceCommand.BaseCommandSize + 10)]
    public SwitchJoyConBaseSubcommandStruct subcommand;

    
    public static SwitchControllerCommand Create(SwitchJoyConRumbleProfile? rumbleProfile = null, SwitchJoyConBaseSubcommand subcommand = null)
    {
        SwitchJoyConRumbleData rumbleData;
        if (rumbleProfile != null)
            rumbleData = new SwitchJoyConRumbleData
            {
                leftJoyConRumble = SwitchJoyConRumbleAmpFreqData.Create(
                    highBandFrequency: rumbleProfile.Value.highBandFrequencyL,
                    highBandAmplitude: rumbleProfile.Value.highBandAmplitudeL,
                    lowBandFrequency: rumbleProfile.Value.lowBandFrequencyL,
                    lowBandAmplitude: rumbleProfile.Value.lowBandAmplitudeL
                ),
                rightJoyConRumble = SwitchJoyConRumbleAmpFreqData.Create(
                    highBandFrequency: rumbleProfile.Value.highBandFrequencyR,
                    highBandAmplitude: rumbleProfile.Value.highBandAmplitudeR,
                    lowBandFrequency: rumbleProfile.Value.lowBandFrequencyR,
                    lowBandAmplitude: rumbleProfile.Value.lowBandAmplitudeR
                )
            };
        else
            rumbleData = new SwitchJoyConRumbleData
            {
                leftJoyConRumble = SwitchJoyConRumbleAmpFreqData.CreateEmpty(),
                rightJoyConRumble = SwitchJoyConRumbleAmpFreqData.CreateEmpty()
            };

        if (subcommand == null)
            subcommand = new SwitchJoyConEmptySubcommand();

        var command = new SwitchControllerCommand
        {
            baseCommand = new InputDeviceCommand(Type, kSize),
            first = 0x01,
            globalCount = globalNumber++,
            rumbleData = rumbleData,
            subcommand = subcommand.GetSubcommand()
        };
        return command;
    }
}
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
internal unsafe struct SwitchJoyConCommand : IInputDeviceCommandInfo
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


    // [FieldOffset(InputDeviceCommand.BaseCommandSize + 10)]
    // public byte subcommandId;


    // [FieldOffset(InputDeviceCommand.BaseCommandSize + 11)]
    // public fixed byte subcommandArg[8];

    
    public static SwitchJoyConCommand Create(SwitchJoyConRumbleProfile? rumbleProfile = null, SwitchJoyConBaseSubcommand subcommand = null)
    {
        Debug.Log($"base command size is {InputDeviceCommand.BaseCommandSize}");

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
                leftJoyConRumble = SwitchJoyConRumbleAmpFreqData.CreateNeutral(),
                rightJoyConRumble = SwitchJoyConRumbleAmpFreqData.CreateNeutral()
            };

        if (subcommand == null)
            subcommand = new SwitchJoyConEmptySubcommand();

        var command = new SwitchJoyConCommand
        {
            baseCommand = new InputDeviceCommand(Type, kSize),
            first = 0x01,
            globalCount = globalNumber++,
            rumbleData = rumbleData,
            subcommand = subcommand.GetSubcommand()
            // subcommandId = 0x30,
        };

        // byte[] argument = new byte[1] { 0x01 };

        // IntPtr ptr = new IntPtr((void*)command.subcommandArg);
        // Marshal.Copy(argument, 0, ptr, 1);
        return command;
        // subcommandArg = new byte[1] { 0x20 }
        // subcommand = subcommand.GetSubcommand()
    }
}
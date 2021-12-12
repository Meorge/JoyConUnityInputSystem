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


[System.Serializable]
public struct SwitchJoyConRumbleProfile
{
    public float highBandFrequencyL;
    public float highBandAmplitudeL;
    public float lowBandFrequencyL;
    public float lowBandAmplitudeL;

    public float highBandFrequencyR;
    public float highBandAmplitudeR;
    public float lowBandFrequencyR;
    public float lowBandAmplitudeR;

    public static SwitchJoyConRumbleProfile CreateEmpty()
    {
        return new SwitchJoyConRumbleProfile {
            highBandFrequencyL = 0,
            highBandAmplitudeL = 0,
            lowBandFrequencyL = 0,
            lowBandAmplitudeL = 0,

            highBandFrequencyR = 0,
            highBandAmplitudeR = 0,
            lowBandFrequencyR = 0,
            lowBandAmplitudeR = 0
        };
    }

    public static SwitchJoyConRumbleProfile CreateNeutral()
    {
        return new SwitchJoyConRumbleProfile {
            highBandFrequencyL = 320,
            highBandAmplitudeL = 0,
            lowBandFrequencyL = 160,
            lowBandAmplitudeL = 0,

            highBandFrequencyR = 320,
            highBandAmplitudeR = 0,
            lowBandFrequencyR = 160,
            lowBandAmplitudeR = 0
        };
    }
}

[StructLayout(LayoutKind.Explicit, Size = 4)]
internal struct SwitchJoyConRumbleAmpFreqData
{
    [FieldOffset(0)] public byte highBandLowerFreq;
    [FieldOffset(1)] public byte highBandAmplitude;
    [FieldOffset(2)] public byte lowBandFreq;
    [FieldOffset(3)] public byte lowBandAmplitude;

    public static SwitchJoyConRumbleAmpFreqData CreateNeutral()
    {
        return new SwitchJoyConRumbleAmpFreqData {
            highBandLowerFreq = 0x00,
            highBandAmplitude = 0x01,
            lowBandFreq = 0x40,
            lowBandAmplitude = 0x40
        };
    }

    public static SwitchJoyConRumbleAmpFreqData CreateEmpty()
    {
        return new SwitchJoyConRumbleAmpFreqData {
            highBandLowerFreq = 0x00,
            highBandAmplitude = 0x00,
            lowBandFreq = 0x00,
            lowBandAmplitude = 0x00
        };
    }

    public static SwitchJoyConRumbleAmpFreqData Create(float highBandFrequency, float highBandAmplitude, float lowBandFrequency, float lowBandAmplitude)
    {
        highBandAmplitude = Mathf.Clamp01(highBandAmplitude);
        lowBandAmplitude = Mathf.Clamp01(lowBandAmplitude);
        
        ushort hf = FrequencyToHFRange(highBandFrequency);
        ushort hf_amp = AmplitudeToHFAmp(highBandAmplitude);

        byte byte0 = (byte)(hf & 0xFF);
        byte byte1 = (byte)(hf_amp + ((hf >> 8) & 0xFF));

        ushort lf = FrequencyToLFRange(lowBandFrequency);
        ushort lf_amp = AmplitudeToLFAmp(lowBandAmplitude);

        byte byte2 = (byte)(lf + ((lf_amp >> 8) & 0xFF));
        byte byte3 = (byte)(lf_amp & 0xFF);
        
        Debug.Log($"HB {highBandFrequency} amp {highBandAmplitude}; LB {lowBandFrequency} amp {lowBandAmplitude}");

        return new SwitchJoyConRumbleAmpFreqData
        {
            highBandLowerFreq = byte0,
            highBandAmplitude = byte1,
            lowBandFreq = byte2,
            lowBandAmplitude = byte3
        };
    }

    public static byte FrequencyToHex(float freq)
    {
        if (freq < 0f) freq = 0f;
        else if (freq > 1252f) freq = 1252f;
        return (byte)Mathf.RoundToInt(Mathf.Log(freq / 10.0f, 2) * 32.0f);
    }

    public static ushort FrequencyToHFRange(float freq)
    {
        byte encodedHexFrequency = FrequencyToHex(freq);
        return (ushort)((encodedHexFrequency - 0x60) * 4);
    }

    public static byte FrequencyToLFRange(float freq)
    {
        byte encodedHexFrequency = FrequencyToHex(freq);
        return (byte)(encodedHexFrequency - 0x40);
    }

    public static ushort AmplitudeToHFAmp(float amp)
    {
        return (ushort)(AmplitudeToHex(amp) * 2);
    }

    public static byte AmplitudeToLFAmp(float amp)
    {
        return (byte)(AmplitudeToHex(amp) / 2 + 64);
    }

    public static byte AmplitudeToHex(float amp)
    {
        if (amp > 1f) amp = 1f;
        if (amp > 0.23f)
            return (byte)Mathf.RoundToInt(Mathf.Log(amp * 8.7f, 2f) * 32.0f);
        else if (amp > 0.12f)
            return (byte)Mathf.RoundToInt(Mathf.Log(amp * 17.0f, 2f) * 16.0f);
        else
            return 0;
    }
}

[StructLayout(LayoutKind.Explicit, Size = 8)]
internal struct SwitchJoyConRumbleData
{
    [FieldOffset(0)] public SwitchJoyConRumbleAmpFreqData leftJoyConRumble;
    [FieldOffset(4)] public SwitchJoyConRumbleAmpFreqData rightJoyConRumble;
}
using System;
using System.Runtime.InteropServices;

namespace UnityEngine.InputSystem.Switch
{
    [Serializable]
    public struct SwitchControllerRumbleProfile
    {
        public float highBandFrequencyLeft;
        public float highBandAmplitudeLeft;
        public float lowBandFrequencyLeft;
        public float lowBandAmplitudeLeft;
    
        public float highBandFrequencyRight;
        public float highBandAmplitudeRight;
        public float lowBandFrequencyRight;
        public float lowBandAmplitudeRight;
    
        public static SwitchControllerRumbleProfile CreateEmpty()
        {
            return new SwitchControllerRumbleProfile {
                highBandFrequencyLeft = 0,
                highBandAmplitudeLeft = 0,
                lowBandFrequencyLeft = 0,
                lowBandAmplitudeLeft = 0,
    
                highBandFrequencyRight = 0,
                highBandAmplitudeRight = 0,
                lowBandFrequencyRight = 0,
                lowBandAmplitudeRight = 0
            };
        }
    
        public static SwitchControllerRumbleProfile CreateNeutral()
        {
            return new SwitchControllerRumbleProfile {
                highBandFrequencyLeft = 320,
                highBandAmplitudeLeft = 0,
                lowBandFrequencyLeft = 160,
                lowBandAmplitudeLeft = 0,
    
                highBandFrequencyRight = 320,
                highBandAmplitudeRight = 0,
                lowBandFrequencyRight = 160,
                lowBandAmplitudeRight = 0
            };
        }

        /// <summary>
        /// Play the rumble defined by the structure on the specified controller if it exists.
        /// </summary>
        /// <param name="controller"></param>
        public void PlayOn(params SwitchControllerHID[] controllers)
        {
            foreach (SwitchControllerHID controller in controllers)
            {
                if (controller == null)
                    continue;

                controller.Rumble(this);
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct SwitchControllerRumbleData
    {
        [FieldOffset(0)] public byte highBandLowerFreq;
        [FieldOffset(1)] public byte highBandAmplitude;
        [FieldOffset(2)] public byte lowBandFreq;
        [FieldOffset(3)] public byte lowBandAmplitude;
    
        public static SwitchControllerRumbleData CreateNeutral()
        {
            return new SwitchControllerRumbleData {
                highBandLowerFreq = 0x00,
                highBandAmplitude = 0x01,
                lowBandFreq = 0x40,
                lowBandAmplitude = 0x40
            };
        }
    
        public static SwitchControllerRumbleData CreateEmpty()
        {
            return new SwitchControllerRumbleData {
                highBandLowerFreq = 0x00,
                highBandAmplitude = 0x00,
                lowBandFreq = 0x00,
                lowBandAmplitude = 0x00
            };
        }
    
        public static SwitchControllerRumbleData Create(float highBandFrequency, float highBandAmplitude, float lowBandFrequency, float lowBandAmplitude)
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
    
            return new SwitchControllerRumbleData
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
    internal struct SwitchControllerDualRumbleData
    {
        [FieldOffset(0)] public SwitchControllerRumbleData LeftControllerRumble;
        [FieldOffset(4)] public SwitchControllerRumbleData RightControllerRumble;
    }
}
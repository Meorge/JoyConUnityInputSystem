using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Switch
{
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal unsafe struct SwitchControllerCommand : IInputDeviceCommandInfo
    {
        private static byte globalNumber = 0x0;
    
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
        public FourCC typeStatic => Type;
    
        internal const int id = 0;
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 49;
    
        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;
    
    
        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public byte first;
        
        
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)]
        public byte globalCount;
        
        
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)]
        public SwitchControllerDualRumbleData rumbleData;
    
    
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 10)]
        public SwitchControllerBaseSubcommandStruct subcommand;
    
        
        public static SwitchControllerCommand Create(SwitchControllerRumbleProfile? rumbleProfile = null, SwitchControllerBaseSubcommand subcommand = null)
        {
            SwitchControllerDualRumbleData rumbleData;
            if (rumbleProfile != null)
                rumbleData = new SwitchControllerDualRumbleData
                {
                    LeftControllerRumble = SwitchControllerRumbleData.Create(
                        highBandFrequency: rumbleProfile.Value.highBandFrequencyLeft,
                        highBandAmplitude: rumbleProfile.Value.highBandAmplitudeLeft,
                        lowBandFrequency: rumbleProfile.Value.lowBandFrequencyLeft,
                        lowBandAmplitude: rumbleProfile.Value.lowBandAmplitudeLeft
                    ),
                    RightControllerRumble = SwitchControllerRumbleData.Create(
                        highBandFrequency: rumbleProfile.Value.highBandFrequencyRight,
                        highBandAmplitude: rumbleProfile.Value.highBandAmplitudeRight,
                        lowBandFrequency: rumbleProfile.Value.lowBandFrequencyRight,
                        lowBandAmplitude: rumbleProfile.Value.lowBandAmplitudeRight
                    )
                };
            else
                rumbleData = new SwitchControllerDualRumbleData
                {
                    LeftControllerRumble = SwitchControllerRumbleData.CreateEmpty(),
                    RightControllerRumble = SwitchControllerRumbleData.CreateEmpty()
                };
    
            if (subcommand == null)
                subcommand = new SwitchControllerEmptySubcommand();
    
            var command = new SwitchControllerCommand
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                first = 0x01,
                globalCount = globalNumber,
                rumbleData = rumbleData,
                subcommand = subcommand.GetSubcommand()
            };
            globalNumber+=1;
            return command;
        }
    }
}

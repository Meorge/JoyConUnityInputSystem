using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System.Runtime.InteropServices;

namespace UnityEngine.InputSystem.Switch
{
    [InputControlLayout(stateType = typeof(SwitchControllerVirtualInputState), displayName = "Joy-Con")]
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class SwitchControllerHID : InputDevice, IInputUpdateCallbackReceiver, IEventPreProcessor
    {
        private bool m_colorsLoaded = false;
        public Color BodyColor { get; protected set; } = Color.black;
        public Color ButtonColor { get; protected set; } = Color.black;
        public Color LeftGripColor { get; protected set; } = Color.black;
        public Color RightGripColor { get; protected set; } = Color.black;

        private StickCalibrationData lStickCalibData = StickCalibrationData.CreateEmpty();
        private StickCalibrationData rStickCalibData = StickCalibrationData.CreateEmpty();

        public BatteryLevelEnum BatteryLevel { get; protected set; } = BatteryLevelEnum.Empty;
        public bool BatteryIsCharging { get; protected set; } = false;
        public ControllerTypeEnum ControllerType { get; protected set; } = ControllerTypeEnum.JoyCon;
        public bool IsPoweredBySwitchOrUSB { get; protected set; } = false;

        private bool m_config1DataLoaded = false;
        private bool m_config2DataLoaded = false;


        private const int configTimerDataDefault = 500;
        private int m_configDataTimer = configTimerDataDefault;

        static SwitchControllerHID()
        {
            var matcherR = new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E)
                .WithCapability("productId", 0x2007);

            var matcherL = new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E)
                .WithCapability("productId", 0x2006);

            InputSystem.RegisterLayout<SwitchControllerHID>(matches: matcherR);
            InputSystem.RegisterLayout<SwitchControllerHID>(matches: matcherL);
            Debug.Log($"Joy-Con layout registered");
        }



        [RuntimeInitializeOnLoadMethod]
        static void Init() { }

        protected override void OnAdded()
        {
            base.OnAdded();
            SetInputReportMode(SwitchJoyConInputMode.Standard);
            ReadColors();
        }


        public void OnNextUpdate() { }

        protected override void FinishSetup()
        {
            base.FinishSetup();
        }

        public void Rumble(SwitchJoyConRumbleProfile rumbleProfile)
        {
            var c = SwitchJoyConCommand.Create(rumbleProfile);
            long returned = ExecuteCommand(ref c);
            if (returned < 0)
            {
                Debug.LogError("Rumble command failed");
            }
        }

        public void RequestDeviceInfo()
        {
            Debug.Log("Requesting device info...");
            var c = SwitchJoyConCommand.Create(subcommand: new SwitchJoyConRequestInfoSubcommand());
            long returned = ExecuteCommand(ref c);
            if (returned < 0)
            {
                Debug.LogError("Request device info failed");
            }
        }

        public void DoBluetoothPairing()
        {
            // step 1
            var s1 = new SwitchJoyConBluetoothManualPairingSubcommand();
            s1.ValueByte = 0x01;
            var c1 = SwitchJoyConCommand.Create(subcommand: s1);

            var s2 = new SwitchJoyConBluetoothManualPairingSubcommand();
            s2.ValueByte = 0x02;
            var c2 = SwitchJoyConCommand.Create(subcommand: s2);

            var s3 = new SwitchJoyConBluetoothManualPairingSubcommand();
            s3.ValueByte = 0x03;
            var c3 = SwitchJoyConCommand.Create(subcommand: s3);

            if (ExecuteCommand(ref c1) < 0)
                Debug.LogError("Step 1 of bluetooth pairing failed");

            if (ExecuteCommand(ref c2) < 0)
                Debug.LogError("Step 2 of bluetooth pairing failed");

            if (ExecuteCommand(ref c3) < 0)
                Debug.LogError("Step 3 of bluetooth pairing failed");
        }

        public void SetInputReportMode(SwitchJoyConInputMode mode)
        {
            var c = SwitchJoyConCommand.Create(subcommand: new SwitchJoyConInputModeSubcommand(mode));
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set report mode to {mode} failed");
        }

        public void SetIMUEnabled(bool active)
        {
            var s = new SwitchJoyConSetIMUEnabledSubcommand();
            s.Enabled = active;

            var c = SwitchJoyConCommand.Create(subcommand: s);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set IMU active to {active} failed");
        }

        public void SetVibrationEnabled(bool active)
        {
            var s = new SwitchJoyConSetVibrationEnabledSubcommand();
            s.Enabled = active;

            var c = SwitchJoyConCommand.Create(subcommand: s);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError($"Set vibration active to {active} failed");
        }

        public void SetLEDs(
            SwitchJoyConLEDStatus p1 = SwitchJoyConLEDStatus.Off,
            SwitchJoyConLEDStatus p2 = SwitchJoyConLEDStatus.Off,
            SwitchJoyConLEDStatus p3 = SwitchJoyConLEDStatus.Off,
            SwitchJoyConLEDStatus p4 = SwitchJoyConLEDStatus.Off)
        {
            var c = SwitchJoyConCommand.Create(subcommand: new SwitchJoyConLEDSubcommand(p1, p2, p3, p4));

            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Set LEDs failed");
        }

        public void ReadColors()
        {
            var readSubcommand = new ReadSPIFlash(atAddress: 0x6050, withLength: 0x2F);
            Debug.Log($"Requesting color info...");
            var c = SwitchJoyConCommand.Create(subcommand: readSubcommand);
            if (ExecuteCommand(ref c) < 0)
                Debug.LogError("Read color info failed");
        }

        private string ThingToHexString<T>(T command)
        {
            int size = Marshal.SizeOf(command);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(command, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return BitConverter.ToString(arr).Replace("-", "");
        }

        /// <summary>
        /// The last used/added Joy-Con (R) controller.
        /// </summary>
        public static SwitchControllerHID current { get; private set; }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
        }

        public enum BatteryLevelEnum
        {
            Full = 8,
            Medium = 6,
            Low = 4,
            Critical = 2,
            Empty = 0
        }

        public enum ControllerTypeEnum
        {
            JoyCon = 3,
            ProControllerOrChargingGrip = 0
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct SwitchControllerGenericInputReport
        {
            public static FourCC Format => new FourCC('H', 'I', 'D');
            [FieldOffset(0)] public byte reportId;
        }

        [StructLayout(LayoutKind.Explicit, Size = kSize)]
        private unsafe struct SwitchControllerSubcommandResponseInputReport
        {
            public static FourCC Format => new FourCC('H', 'I', 'D');
            public const byte kSize = 50;
            public const byte expectedReportId = 0x21;

            [FieldOffset(0)] public byte reportId;
            [FieldOffset(2)] public byte batteryAndConnectionInfo;

            [FieldOffset(13)] public byte ack;
            [FieldOffset(14)] public byte subcommandId;
            [FieldOffset(15)] public fixed byte replyData[35];
        }

        unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr eventPtr)
        {
            // Use the virtual controller events as-is, and skip other delta state events
            if (eventPtr.type == DeltaStateEvent.Type)
                return DeltaStateEvent.FromUnchecked(eventPtr)->stateFormat == SwitchControllerVirtualInputState.Format;

            // Use all other non-state/non-delta-state events
            if (eventPtr.type != StateEvent.Type)
                return true;

            var stateEvent = StateEvent.FromUnchecked(eventPtr);
            var size = stateEvent->stateSizeInBytes;

            // If state format was the virtual controller, just use it
            if (stateEvent->stateFormat == SwitchControllerVirtualInputState.Format)
                return true;

            // Skip unrecognized state events?
            if (stateEvent->stateFormat != SwitchControllerGenericInputReport.Format || size < sizeof(SwitchControllerGenericInputReport))
                return false;

            var genericReport = (SwitchControllerGenericInputReport*)stateEvent->state;

            // Simple report mode!
            if (genericReport->reportId == 0x3f)
            {
                SetInputReportMode(SwitchJoyConInputMode.Standard);
                return false;
            }

            // Subcommand reply!
            else if (genericReport->reportId == 0x21)
            {
                var data = ((SwitchControllerSubcommandResponseInputReport*)stateEvent->state);
                HandleSubcommand(*data);
                return true;
            }

            // Full report mode!
            if (genericReport->reportId == 0x30)
            {
                var data = ((SwitchControllerFullInputReport*)stateEvent->state)->ToHIDInputReport(ref lStickCalibData, ref rStickCalibData);
                *((SwitchControllerVirtualInputState*)stateEvent->state) = data;
                stateEvent->stateFormat = SwitchControllerVirtualInputState.Format;
                return true;
            }
            return false;
        }

        private unsafe void HandleSubcommand(SwitchControllerSubcommandResponseInputReport response)
        {
            // Connection info
            BatteryLevel = (BatteryLevelEnum)((response.batteryAndConnectionInfo & 0xE0) >> 4);
            BatteryIsCharging = (response.batteryAndConnectionInfo & 0x10) >> 4 != 0;

            int connectionInfo = response.batteryAndConnectionInfo & 0x0F;
            ControllerType = (ControllerTypeEnum)((connectionInfo >> 1) & 3);
            IsPoweredBySwitchOrUSB = (connectionInfo & 1) == 1;

            SwitchJoyConSubcommandID subcommandReplyId = (SwitchJoyConSubcommandID)response.subcommandId;
            var subcommandWasAcknowledged = (response.ack & 0x80) != 0;

            // Debug.Log($"Subcommand response for {subcommandReplyId}: {response.ack:X2}");

            if (subcommandWasAcknowledged)
            {
                switch (subcommandReplyId)
                {
                    case SwitchJoyConSubcommandID.SPIFlashRead:
                        HandleFlashRead(response.replyData);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnUpdate()
        {

        }

        [StructLayout(LayoutKind.Explicit, Size = 12)]
        private unsafe struct FactoryConfigCalib1
        {
            [FieldOffset(0)] public fixed ushort accOriginPos[3];
            [FieldOffset(3)] public fixed ushort accSensitivity[3];
            [FieldOffset(6)] public fixed ushort gyroOrigin[3];
            [FieldOffset(9)] public fixed ushort gyroSensitivity[3];
        }

        [StructLayout(LayoutKind.Explicit, Size = 18)]
        private unsafe struct RawStickCalibrationData
        {
            [FieldOffset(0)] public fixed byte lStick[9];
            [FieldOffset(9)] public fixed byte rStick[9];
        }


        [StructLayout(LayoutKind.Explicit, Size = 3)]
        private struct Rgb24Bit
        {
            [FieldOffset(0)] public byte r;
            [FieldOffset(1)] public byte g;
            [FieldOffset(2)] public byte b;

            public Color ToUnityColor()
            {
                return new Color32(r, g, b, 255);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 3 * 4)]
        private struct ControllerColors
        {
            [FieldOffset(0)] public Rgb24Bit bodyColor;
            [FieldOffset(3)] public Rgb24Bit buttonColor;
            [FieldOffset(6)] public Rgb24Bit leftGripColor;
            [FieldOffset(9)] public Rgb24Bit rightGripColor;
        }

        private unsafe void HandleFlashRead(byte* dataPtr)
        {
            byte[] data = new byte[35];
            Marshal.Copy(new IntPtr(dataPtr), data, 0, 35);
            uint address = BitConverter.ToUInt32(data, 0);
            byte length = data[4];

            byte* response = dataPtr + 5;

            Debug.Log($"Flash read 0x{address:X4} with length 0x{length:X2}");

            // Handling factory config and calib data

            // Serial number
            if (address == 0x6000 && length == 0x10) {} 

            // Factory analog stick calibration
            else if (address == 0x603D && length == 0x12)
                DecodeStickCalibrationData(response);

            // User analog stick calibration
            else if (address == 0x8010 && length == 0x16) { }

            // Stick device parameters 1
            else if (address == 0x6086 && length == 0x12) { }

            // Stick device parameters 2
            else if (address == 0x6098 && length == 0x12) { }

            // Shipment?
            else if (address == 0x5000 && length == 0x01) { }

            // Colors
            else if (address == 0x6050 && length == 0x2F)
                DecodeColorData(response);
        }

        private unsafe void DecodeStickCalibrationData(byte* response)
        {
            DecodeLeftStickData(response);
            DecodeRightStickData(response + 9);       
        }

        private unsafe void DecodeColorData(byte* response)
        {
            ControllerColors colors = (ControllerColors)Marshal.PtrToStructure((IntPtr)response, typeof(ControllerColors));
            BodyColor = colors.bodyColor.ToUnityColor();
            ButtonColor = colors.buttonColor.ToUnityColor();
            LeftGripColor = colors.leftGripColor.ToUnityColor();
            RightGripColor = colors.rightGripColor.ToUnityColor();
            m_colorsLoaded = true;

            Debug.Log($"Colors loaded: {BodyColor}, {ButtonColor}");
        }

        private unsafe ushort[] DecodeStickData(byte* stickCal)
        {
            // yoinked from
            // https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/spi_flash_notes.md#analog-stick-factory-and-user-calibration
            ushort[] data = new ushort[6];
            data[0] = (ushort)((stickCal[1] << 8) & 0xF00 | stickCal[0]);
            data[1] = (ushort)((stickCal[2] << 4) | (stickCal[1] >> 4));
            data[2] = (ushort)((stickCal[4] << 8) & 0xF00 | stickCal[3]);
            data[3] = (ushort)((stickCal[5] << 4) | (stickCal[4] >> 4));
            data[4] = (ushort)((stickCal[7] << 8) & 0xF00 | stickCal[6]);
            data[5] = (ushort)((stickCal[8] << 4) | (stickCal[7] >> 4));
            return data;
        }

        private unsafe void DecodeLeftStickData(byte* lStickCal)
        {
            ushort[] decoded = DecodeStickData(lStickCal);

            var xAxisMaxAboveCenter = decoded[0];
            var yAxisMaxAboveCenter = decoded[1];
            var xAxisCenter = decoded[2];
            var yAxisCenter = decoded[3];
            var xAxisMinBelowCenter = decoded[4];
            var yAxisMinBelowCenter = decoded[5];


            var lStickXMin = xAxisCenter - xAxisMinBelowCenter;
            var lStickXMax = xAxisCenter + xAxisMaxAboveCenter;

            var lStickYMin = yAxisCenter - yAxisMinBelowCenter;
            var lStickYMax = yAxisCenter + yAxisMaxAboveCenter;

            lStickCalibData = new StickCalibrationData()
            {
                xMin = lStickXMin,
                xMax = lStickXMax,
                yMin = lStickYMin,
                yMax = lStickYMax
            };

            Debug.Log($"Left stick data: {lStickXMin}-{lStickXMax}");
        }

        private unsafe void DecodeRightStickData(byte* rStickCal)
        {
            ushort[] decoded = DecodeStickData(rStickCal);

            var xAxisCenter = decoded[0];
            var yAxisCenter = decoded[1];
            var xAxisMinBelowCenter = decoded[2];
            var yAxisMinBelowCenter = decoded[3];
            var xAxisMaxAboveCenter = decoded[4];
            var yAxisMaxAboveCenter = decoded[5];

            var rStickXMin = xAxisCenter - xAxisMinBelowCenter;
            var rStickXMax = xAxisCenter + xAxisMaxAboveCenter;

            var rStickYMin = yAxisCenter - yAxisMinBelowCenter;
            var rStickYMax = yAxisCenter + yAxisMaxAboveCenter;

            rStickCalibData = new StickCalibrationData()
            {
                xMin = rStickXMin,
                xMax = rStickXMax,
                yMin = rStickYMin,
                yMax = rStickYMax
            };
        }

        public struct StickCalibrationData
        {
            public int xMin;
            public int xMax;

            public int yMin;
            public int yMax;

            public static StickCalibrationData CreateEmpty()
            {
                return new StickCalibrationData() { xMin = 0, xMax = 0, yMin = 0, yMax = 0 };
            }
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using System.Runtime.InteropServices;

[InputControlLayout(stateType = typeof(SwitchJoyConRHIDInputState), displayName = "Joy-Con (R)")]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class SwitchJoyConRHID : InputDevice, IInputUpdateCallbackReceiver
{
    public ButtonControl plus { get; protected set; }
    public ButtonControl stickPressR { get; protected set; }
    public ButtonControl home { get; protected set; }
    public ButtonControl r { get; protected set; }
    public ButtonControl zr { get; protected set; }

    public ButtonControl buttonSouthR { get; protected set; }
    public ButtonControl buttonEastR { get; protected set; }
    public ButtonControl buttonWestR { get; protected set; }
    public ButtonControl buttonNorthR { get; protected set; }
    public ButtonControl slR { get; protected set; }
    public ButtonControl srR { get; protected set; }

    public Color BodyColor { get; protected set; } = Color.black;
    public Color ButtonColor { get; protected set; } = Color.black;

    private StickCalibrationData rStickCalibData = StickCalibrationData.CreateEmpty();

    public Vector2 RightStick { get; protected set; } = new Vector2();


    public BatteryLevelEnum BatteryLevel { get; protected set; } = BatteryLevelEnum.Empty;
    public bool BatteryIsCharging { get; protected set; } = false;
    public ControllerTypeEnum ControllerType { get; protected set; } = ControllerTypeEnum.JoyCon;
    public bool IsPoweredBySwitchOrUSB { get; protected set; } = false;

    private bool m_configDataLoaded = false;

    static SwitchJoyConRHID()
    {
        var matcherR = new InputDeviceMatcher()
            .WithInterface("HID")
            .WithCapability("vendorId", 0x57E)
            .WithCapability("productId", 0x2007);

        var matcherL = new InputDeviceMatcher()
            .WithInterface("HID")
            .WithCapability("vendorId", 0x57E)
            .WithCapability("productId", 0x2006);

        InputSystem.RegisterLayout<SwitchJoyConRHID>(matches: matcherR);
        InputSystem.RegisterLayout<SwitchJoyConRHID>(matches: matcherL);
        Debug.Log($"Joy-Con layout registered");
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() { }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        plus = GetChildControl<ButtonControl>("plus");
        stickPressR = GetChildControl<ButtonControl>("stickPressR");
        home = GetChildControl<ButtonControl>("home");

        buttonSouthR = GetChildControl<ButtonControl>("buttonSouthR");
        buttonEastR = GetChildControl<ButtonControl>("buttonEastR");
        buttonWestR = GetChildControl<ButtonControl>("buttonWestR");
        buttonNorthR = GetChildControl<ButtonControl>("buttonNorthR");
        slR = GetChildControl<ButtonControl>("slR");
        srR = GetChildControl<ButtonControl>("srR");
        r = GetChildControl<ButtonControl>("r");
        zr = GetChildControl<ButtonControl>("zr");
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

    public void ReadFactoryConfigCalib2()
    {
        var readSubcommand = new ReadSPIFlash(atAddress: 0x603D, withLength: 0x18);
        Debug.Log($"Requesting factory config and calibration data...");
        var c = SwitchJoyConCommand.Create(subcommand: readSubcommand);
        if (ExecuteCommand(ref c) < 0)
            Debug.LogError("Read factory config and calib 2 from SPI flash failed");
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
    public static SwitchJoyConRHID current { get; private set; }

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

    public unsafe void OnUpdate()
    {
        var currentState = new SwitchJoyConRHIDInputState();
        this.CopyState(out currentState);
        
        int reportType = currentState.reportId;

        // Connection info
        BatteryLevel = (BatteryLevelEnum)((currentState.batteryAndConnectionInfo & 0xE0) >> 4);
        BatteryIsCharging = (currentState.batteryAndConnectionInfo & 0x10) >> 4 != 0;

        int connectionInfo = currentState.batteryAndConnectionInfo & 0x0F;
        ControllerType = (ControllerTypeEnum)((connectionInfo >> 1) & 3);
        IsPoweredBySwitchOrUSB = (connectionInfo & 1) == 1;

        // Left analog stick data
        var l0 = currentState.rightStick[0];
        var l1 = currentState.rightStick[1];
        var l2 = currentState.rightStick[2];
        var rawLeftStickHoriz = l0 | ((l1 & 0xF) << 8);
        var rawLeftStickVert = (l1 >> 4) | (l2 << 4);

        // Right analog stick data
        var r0 = currentState.rightStick[0];
        var r1 = currentState.rightStick[1];
        var r2 = currentState.rightStick[2];
        var rawRightStickHoriz = r0 | ((r1 & 0xF) << 8);
        var rawRightStickVert = (r1 >> 4) | (r2 << 4);

        var trueRightStickHoriz = (Mathf.InverseLerp(rStickCalibData.xMin, rStickCalibData.xMax, rawRightStickHoriz) * 2) - 1;
        var trueRightStickVert = (Mathf.InverseLerp(rStickCalibData.yMin, rStickCalibData.yMax, rawRightStickVert) * 2) - 1;
        
        RightStick = new Vector2(trueRightStickHoriz, trueRightStickVert);

        if (reportType == 0x21)
        {
            SwitchJoyConSubcommandID subcommandReplyId = (SwitchJoyConSubcommandID)currentState.subcommandReplyId;
            int ack = currentState.subcommandAck;

            Debug.Log("Subcommand received");

            Debug.Log($"Battery: {BatteryLevel}, controller type: {ControllerType}, is powered by Switch or USB: {IsPoweredBySwitchOrUSB}");
            Debug.Log($"Subcommand response for {subcommandReplyId}: {ack:X2}");

            var subcommandWasAcknowledged = (ack & 0x80) != 0;
            if (subcommandWasAcknowledged)
            {
                switch (subcommandReplyId)
                {
                    case SwitchJoyConSubcommandID.SPIFlashRead:
                        HandleFlashRead(currentState.subcommandReplyData);
                        break;
                    default:
                        Debug.Log($"No code for handling {subcommandReplyId}");
                        break;
                }
            }
        }


        // If we haven't gotten config data yet, let's ask for it
        if (!m_configDataLoaded) ReadFactoryConfigCalib2();
    }


    [StructLayout(LayoutKind.Explicit, Size = 3 * 8 + 1)]
    private unsafe struct FactoryConfigCalib2
    {
        // left analog stick calib
        [FieldOffset(0)] public fixed byte lStick[9];

        // right analog stick calib
        [FieldOffset(9)] public fixed byte rStick[9];

        // body rgb (24 bit)
        [FieldOffset(19)] public Rgb24Bit bodyColor;

        // buttons rgb (24 bit)
        [FieldOffset(22)] public Rgb24Bit buttonColor;
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

    private unsafe void HandleFlashRead(byte* dataPtr)
    {
        byte[] data = new byte[35];
        Marshal.Copy(new IntPtr(dataPtr), data, 0, 35);
        uint address = BitConverter.ToUInt32(data, 0);
        byte length = data[4];

        byte[] response = new byte[30];
        Array.Copy(data, 5, response, 0, 30);

        Debug.Log($"Flash read 0x{address:X4} with length 0x{length:X2}");

        // Handling factory config and calib data
        if (address == 0x603D && length == 0x18)
            HandleFlashRead_FactoryConfigCalibData2(response);
    }

    private unsafe void HandleFlashRead_FactoryConfigCalibData2(byte[] response)
    {
        FactoryConfigCalib2 dataStruct = new FactoryConfigCalib2();
        // Put in a struct
        GCHandle h = GCHandle.Alloc(response, GCHandleType.Pinned);
        try
        {
            dataStruct = (FactoryConfigCalib2)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(FactoryConfigCalib2));
        }
        finally
        {
            h.Free();
        }

        DecodeRightStickData(dataStruct.rStick);

        BodyColor = dataStruct.bodyColor.ToUnityColor();
        ButtonColor = dataStruct.buttonColor.ToUnityColor();

        m_configDataLoaded = true;
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

    private struct StickCalibrationData
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
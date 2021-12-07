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

        Debug.Log($"Joy-Con (R) layout registered");
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() { }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        // plus = GetChildControl<ButtonControl>("plus");
        // stickPressR = GetChildControl<ButtonControl>("stickPressR");
        // home = GetChildControl<ButtonControl>("home");


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

    public int rightStickHoriz { get; protected set; } = 0;
    public int rightStickVert { get; protected set; } = 0;

    public int leftStickHoriz { get; protected set; } = 0;
    public int leftStickVert { get; protected set; } = 0;

    public unsafe void OnUpdate()
    {
        var currentState = new SwitchJoyConRHIDInputState();
        this.CopyState(out currentState);
        
        int reportType = currentState.reportId;

        // Connection info
        int batteryInfo = currentState.batteryAndConnectionInfo & 0xF0;
        int connectionInfo = (currentState.batteryAndConnectionInfo & 0x0F);
        int controllerTypeGlobal = (connectionInfo >> 1) & 3;
        int powerType = connectionInfo & 1;

        // Left analog stick data
        var l0 = currentState.rightStick[0];
        var l1 = currentState.rightStick[1];
        var l2 = currentState.rightStick[2];
        leftStickHoriz = l0 | ((l1 & 0xF) << 8);
        leftStickVert = (l1 >> 4) | (l2 << 4);

        // Right analog stick data
        var r0 = currentState.rightStick[0];
        var r1 = currentState.rightStick[1];
        var r2 = currentState.rightStick[2];
        rightStickHoriz = r0 | ((r1 & 0xF) << 8);
        rightStickVert = (r1 >> 4) | (r2 << 4);

        if (reportType == 0x21)
        {
            int subcommandReplyId = currentState.subcommandReplyId;
            int ack = currentState.subcommandAck;

            Debug.Log("Subcommand received");

            Debug.Log($"Battery: {batteryInfo:X2}, controller type: {controllerTypeGlobal:X2}, power type: {powerType:X2}");
            Debug.Log($"Timer is {currentState.timer}");
            Debug.Log($"Subcommand response for {subcommandReplyId:X2}: {ack:X2}");

            var subcommandWasAcknowledged = (ack & 0x80) != 0;
            if (subcommandWasAcknowledged)
            {
                Debug.Log("Subcommand was acknoledged!");

                int controllerType = currentState.subcommandReplyData[2];
                Debug.Log($"Controller type: {controllerType:X2}");
            }
        }
    }
}
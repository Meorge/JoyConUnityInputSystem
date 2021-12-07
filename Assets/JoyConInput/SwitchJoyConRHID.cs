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
    
    public IntegerControl reportId;
    public IntegerControl subcommandAck;
    public IntegerControl subcommandReplyId;
    public IntegerControl firmwareVersion;
    public IntegerControl controllerType;
    public IntegerControl shouldBe02;
    public IntegerControl shouldBe01;
    public IntegerControl useSPIColors;

    public DpadControl hat { get; protected set; }
    public ButtonControl plus { get; protected set; }
    public ButtonControl stickPress { get; protected set; }
    public ButtonControl home { get; protected set; }
    public ButtonControl r { get; protected set; }
    public ButtonControl zr { get; protected set; }

    public ButtonControl buttonSouthR { get; protected set; }
    public ButtonControl buttonEastR { get; protected set; }
    public ButtonControl buttonWestR { get; protected set; }
    public ButtonControl buttonNorthR { get; protected set; }
    public ButtonControl sl { get; protected set; }
    public ButtonControl sr { get; protected set; }

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
        Debug.Log("in finish setup");

        reportId = GetChildControl<IntegerControl>("reportId");
        subcommandAck = GetChildControl<IntegerControl>("subcommandAck");
        subcommandReplyId = GetChildControl<IntegerControl>("subcommandReplyId");
        firmwareVersion = GetChildControl<IntegerControl>("firmwareVersion");
        controllerType = GetChildControl<IntegerControl>("controllerType");
        shouldBe02 = GetChildControl<IntegerControl>("shouldBe02");
        shouldBe01 = GetChildControl<IntegerControl>("shouldBe01");
        useSPIColors = GetChildControl<IntegerControl>("useSPIColors");


        // hat = GetChildControl<DpadControl>("hat");
        // plus = GetChildControl<ButtonControl>("plus");
        // stickPress = GetChildControl<ButtonControl>("stickPress");
        // home = GetChildControl<ButtonControl>("home");
        // r = GetChildControl<ButtonControl>("r");
        // zr = GetChildControl<ButtonControl>("zr");

        buttonSouthR = GetChildControl<ButtonControl>("buttonSouthR");
        buttonEastR = GetChildControl<ButtonControl>("buttonEastR");
        buttonWestR = GetChildControl<ButtonControl>("buttonWestR");
        buttonNorthR = GetChildControl<ButtonControl>("buttonNorthR");
        // sl = GetChildControl<ButtonControl>("sl");
        // sr = GetChildControl<ButtonControl>("sr");

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

        Debug.Log(CommandToBytes(c));

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

        Debug.Log(CommandToBytes(c));

        if (ExecuteCommand(ref c) < 0)
            Debug.LogError("Set LEDs failed");
    }

    private string CommandToBytes(SwitchJoyConCommand command)
    {
        int size = Marshal.SizeOf(command);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(command, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return BitConverter.ToString(arr).Replace("-", " ");
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

    public void OnUpdate()
    {
        int reportType = reportId.ReadValue();

        if (reportType == 0x21)
        {
            Debug.Log($"Got a subcommand response for {subcommandReplyId.ReadValue()} with acknowledgement {subcommandAck.ReadValue()}");
            Debug.Log($"Firmware version: {firmwareVersion.ReadValue()}");
            Debug.Log($"Controller type: {controllerType.ReadValue()}");
            Debug.Log($"02: {shouldBe02.ReadValue()}, 01: {shouldBe01.ReadValue()}");
            Debug.Log($"Use SPI Colors: {useSPIColors.ReadValue()}");
        }
    }
}
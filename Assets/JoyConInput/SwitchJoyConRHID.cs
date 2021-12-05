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

[InputControlLayout(stateType = typeof(SwitchJoyConRHIDInputState), displayName = "Joy-Con (R) Custom")]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class SwitchJoyConRHID : Joystick
{
    public ButtonControl plus { get; protected set; }
    public ButtonControl stickPress { get; protected set; }
    public ButtonControl home { get; protected set; }
    public ButtonControl r { get; protected set; }
    public ButtonControl zr { get; protected set; }

    public ButtonControl buttonSouth { get; protected set; }
    public ButtonControl buttonEast { get; protected set; }
    public ButtonControl buttonWest { get; protected set; }
    public ButtonControl buttonNorth { get; protected set; }
    public ButtonControl sl { get; protected set; }
    public ButtonControl sr { get; protected set; }

    static SwitchJoyConRHID()
    {
        var matcher = new InputDeviceMatcher()
            .WithInterface("HID")
            .WithCapability("vendorId", 0x57E)
            .WithCapability("productId", 0x2007);

        InputSystem.RegisterLayout<SwitchJoyConRHID>(matches: matcher);

        Debug.Log($"Joy-Con (R) layout registered");
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() { }

    protected override void FinishSetup()
    {
        Debug.Log("in finish setup");

        plus = GetChildControl<ButtonControl>("plus");
        stickPress = GetChildControl<ButtonControl>("stickPress");
        home = GetChildControl<ButtonControl>("home");
        r = GetChildControl<ButtonControl>("r");
        zr = GetChildControl<ButtonControl>("zr");

        buttonSouth = GetChildControl<ButtonControl>("buttonSouth");
        buttonEast = GetChildControl<ButtonControl>("buttonEast");
        buttonWest = GetChildControl<ButtonControl>("buttonWest");
        buttonNorth = GetChildControl<ButtonControl>("buttonNorth");
        sl = GetChildControl<ButtonControl>("sl");
        sr = GetChildControl<ButtonControl>("sr");

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

    /// <summary>
    /// The last used/added Joy-Con (R) controller.
    /// </summary>
    public new static SwitchJoyConRHID current { get; private set; }

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
}
# Nintendo Switch Controllers for Unity Input System

This is a prototype for Nintendo Switch controller Bluetooth support for the new Unity Input System, brought to compatibility with the latest versions of Unity and its InputSystem package.

## Installation 

Using the Unity Package Manager, add the package from the following git URL: https://github.com/Meorge/JoyConUnityInputSystem.git?path=/Packages/com.canopteks.switchcontrollerinput.

## Notes

- Keep in mind that the left and right Joy-Cons are treated by Unity as separate controllers. As a result, you'll need to make sure you use the correct controller in bindings.
  - For example, the control `buttonSouth` exists for "Joy-Con (L)", but it is never used. Whenever you want to refer to the player pressing the "A" button, make sure the device is either "Joy-Con (R)" or "Pro Controller".
- The Input Action editor GUI won't display the controllers when attempting to add them to a control scheme. The only workaround I found for this so far was to go into the `.inputactions` file and add them manually.
- I've experienced a few strange but not (yet) reproducible issues, on occasion:
  - Even though a Joy-Con is connected, no output will appear from it in the Input Debugger. To fix this, I typically shut it off and reconnect it.
  - The Unity Editor will freeze up for a few seconds when connecting a Joy-Con.
- Reconnecting a JoyCon already paired to your computer does not work with a 5.0 Bluetooth device. It is a known hardware problem of the JoyCon, that cannot be fixed without changing the device to a 4.x one.

## Supported controllers

- Joy-Con (L)
- Joy-Con (R)
- Pro Controller

## Supported features

### Device data analysis

General data about the connected Switch controllers can be found in the menu `Windows/Switch Controller Debugger/Joycon Device Data`. This however doesn't include the input data, which can be found instead `Windows/Analysis/Input Debugger` (as well as the input data of every other controller).

### Input

- A, B, X, Y on Joy-Con (R) and Pro Controller as `buttonEast`, `buttonSouth`, `buttonNorth`, and `buttonEast` respectively
- Up, down, left, and right on Joy-Con (L) and Pro Controller as `dpad`
- Triggers and shoulders, including SL and SR for Joy-Cons
- Capture and HOME buttons
- Minus and Plus buttons
- Left and right analog sticks and stick presses
  - **Note**: Although they use the factory configuration/calibration info, they still seem to have significant drift

### HD Rumble: WIP

High-band and low-band amplitude and frequency for the left and right Joy-Cons (or, in the case of the Pro Controller, the left and right sides of the controller) can be set with the `SwitchControllerHID.Rumble()` method.

Example:

```c#
var rumbleProfile = new SwitchControllerRumbleProfile
    {
        lowBandFrequencyL = 160f,
        lowBandAmplitudeL = 0.1f,
        lowBandFrequencyR = 150f,
        lowBandAmplitudeR = 0.5f,
        
        highBandFrequencyL = 200f,
        highBandAmplitudeL = 0.8f,
        highBandFrequencyR = 150f,
        highBandAmplitudeR = 1.0f
    };
        
controller.Rumble(rumbleProfile);
```

The HD Rumble features can be tested `Windows/Switch Controller Debugger/Joycon Rumble Playground`

### Gyroscope and acceleration (partial)

Once the controller's IMU has been enabled with `SwitchControllerHID.SetIMUEnabled(true)`, it will report uncalibrated acceleration and gyroscope data. In my testing, this data isn't particularly accurate, so we should figure out how to improve it.

### Player LEDs

The green LEDs on the controllers can be set with `SwitchControllerHID.SetLEDs()`. 

Example:
```c#
controller.SetLEDs(
    p1: SwitchControllerLEDStatusEnum.Off,
    p2: SwitchControllerLEDStatusEnum.On,
    p3: SwitchControllerLEDStatusEnum.Flashing,
    p4: SwitchControllerLEDStatusEnum.On
    );
```

However, quitting the application doesn't reset the LEDs. If you want them to get back to their initial state, you should either restart the controller or set the LEDs again in the `OnApplicationQuit()` method of a GameObject.

## Credits

Fork of the repository [JoyConUnityInputSystem]https://github.com/Meorge/JoyConUnityInputSystem by [Meorge](https://github.com/Meorge), which is itself largely based on: 

- The amazing [Nintendo Switch Reverse Engineering](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering) repository
  - Format for parsing controller data and sending subcommands is from [bluetooth_hid_notes.md](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/bluetooth_hid_notes.md) and [bluetooth_hid_subcommands_notes.md](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/bluetooth_hid_subcommands_notes.md)
  - HD Rumble encoding algorithm is from [rumble_data_table.md](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md)
  - Gyroscope and accelerometer decoding algorithms are from [imu_sensor_notes.md](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/imu_sensor_notes.md)
- [Unity InputSystem fork implementing the Nintendo Switch Pro Controller](https://github.com/Unity-Technologies/InputSystem/pull/1471)
  - The `SwitchControllerFullInputReport.ToHIDInputReport()` method follows the [implementation in SwitchFullInputReport](https://github.com/Unity-Technologies/InputSystem/blob/67a8605dc8d2bb67d251117cbe0e371d043e7a13/Packages/com.unity.inputsystem/InputSystem/Plugins/Switch/SwitchProControllerHID.cs#L360).
  - The `SwitchControllerHID.PreProcessEvent()` method is an expanded version of the [implementation in SwitchProControllerHID](https://github.com/Unity-Technologies/InputSystem/blob/67a8605dc8d2bb67d251117cbe0e371d043e7a13/Packages/com.unity.inputsystem/InputSystem/Plugins/Switch/SwitchProControllerHID.cs#L213).
  - The `SwitchControllerVirtualInputState` class is an expanded version of [SwitchProControllerHIDInputState](https://github.com/Unity-Technologies/InputSystem/blob/67a8605dc8d2bb67d251117cbe0e371d043e7a13/Packages/com.unity.inputsystem/InputSystem/Plugins/Switch/SwitchProControllerHID.cs#L20), with support added for Joy-Cons and IMU data. The format was changed from "SPVS" to "SCVS", since it also supports Joy-Cons.

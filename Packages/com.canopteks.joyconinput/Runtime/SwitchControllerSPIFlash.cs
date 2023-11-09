using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.InputSystem.Switch
{
    // WIP
    public enum SwitchControllerSPIFlashReadAddressEnum : uint 
    {
        SerialNumber = 0x6000, // 0x10
        DeviceType = 0x6012, // 0x01
        ColorInfoExists = 0x601B, // 0x01
        IMUCalibration = 0x6020, // 0x18
        StickCalibration = 0x603D, // 0x12
        ColorData = 0x6050

    }
}

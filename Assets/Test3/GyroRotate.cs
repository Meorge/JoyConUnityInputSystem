using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

public class GyroRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        SwitchControllerHID.current.ReadUserIMUCalibrationData();

    }

    // Update is called once per frame
    void Update()
    {
        if (SwitchControllerHID.current.buttonSouth.wasReleasedThisFrame)
        {
            SwitchControllerHID.current.ReadUserIMUCalibrationData();
            // SwitchControllerHID.current.SetIMUEnabled(true);
        }
        if (SwitchControllerHID.current.buttonEast.wasReleasedThisFrame)
        {
            SwitchControllerHID.current.ResetYaw();
        }
        transform.localRotation = SwitchControllerHID.current.deviceRotation.ReadValue();
    }
}

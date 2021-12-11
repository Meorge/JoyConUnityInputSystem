using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

public class GyroRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SwitchControllerHID.current.buttonSouth.wasPressedThisFrame)
        {
            // SwitchControllerHID.current.ReadIMUCalibrationData();
            SwitchControllerHID.current.SetIMUEnabled(true);
        }
        transform.eulerAngles = SwitchControllerHID.current.orientation.ReadValue() * 0.1f;
    }
}

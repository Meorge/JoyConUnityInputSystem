using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RumbleTest : MonoBehaviour
{
    [SerializeField] private SwitchJoyConRumbleProfile m_profile = new SwitchJoyConRumbleProfile();
    [SerializeField] private Toggle m_toggle = null;

    private SwitchJoyConRHID c = null;
    // Start is called before the first frame update
    void Start()
    {
        c = SwitchJoyConRHID.current;
        // SwitchJoyConRHID.current.RequestDeviceInfo();
        // SwitchJoyConRHID.current.SetInputReportMode(SwitchJoyConInputMode.Simple);

        // Not being done: calibration data (not sure how to do this with Input System)

        // Bluetooth pairing
        // SwitchJoyConRHID.current.DoBluetoothPairing();

        // Setting LEDs
        c.SetLEDs(
            p1: SwitchJoyConLEDStatus.On,
            p2: SwitchJoyConLEDStatus.Off,
            p3: SwitchJoyConLEDStatus.Off,
            p4: SwitchJoyConLEDStatus.On
        );

        // Setting IMU to active
        c.SetIMUEnabled(true);

        // Setting input report mode to standard
        c.SetInputReportMode(SwitchJoyConInputMode.Standard);

        // Enabling vibration (seems to already be enabled)
        // SwitchJoyConRHID.current.SetVibrationEnabled(true);

        StartCoroutine(RumbleCoroutine());
    }

    void Update() {
        if (c.buttonSouthR.wasPressedThisFrame) {
            c.RequestDeviceInfo();
            Debug.Log("button got pressed!");
        }
    }

    IEnumerator RumbleCoroutine()
    {
        while (true)
        {
            if (m_toggle.isOn)
                SwitchJoyConRHID.current.Rumble(m_profile);

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnHFFreqUpdated(float v)
    {
        m_profile.highBandFrequencyR = v;
    }

    public void OnHFAmpUpdated(float v)
    {
        m_profile.highBandAmplitudeR = v;
    }

    public void OnLFFreqUpdated(float v)
    {
        m_profile.lowBandFrequencyR = v;
    }

    public void OnLFAmpUpdated(float v)
    {
        m_profile.lowBandAmplitudeR = v;
    }
}

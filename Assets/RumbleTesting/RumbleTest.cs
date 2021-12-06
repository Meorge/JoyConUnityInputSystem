using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RumbleTest : MonoBehaviour
{
    [SerializeField] private SwitchJoyConRumbleProfile m_profile = new SwitchJoyConRumbleProfile();
    [SerializeField] private Toggle m_toggle = null;

    // Start is called before the first frame update
    void Start()
    {
        SwitchJoyConRHID.current.RequestDeviceInfo();
        SwitchJoyConRHID.current.SetStandardReportMode();
        StartCoroutine(RumbleCoroutine());
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

using System.Net.NetworkInformation;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput pInput = null;

    [SerializeField] Vector2 m_movement = new Vector2();

    Rigidbody rb;

    float amp = 0f;
    private float ampOffset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Debug.Log("Is SwitchControllerHID null: " + SwitchControllerHID.current==null);
        // if (SwitchControllerHID.current != null)
        // {
        //     SwitchControllerHID.current.SetVibrationEnabled(true);
        //     // StartCoroutine(RumbleCoroutine());

        //     SwitchControllerHID.current.SetLEDs(
        //         p1: SwitchControllerLEDStatusEnum.Flashing,
        //         p2: SwitchControllerLEDStatusEnum.On,
        //         p3: SwitchControllerLEDStatusEnum.Off,
        //         p4: SwitchControllerLEDStatusEnum.On
        //     );
        //     // SwitchJoyConLHID.current?.SetLEDs(
        //     //     p1: SwitchControllerLEDStatusEnum.Flashing,
        //     //     p2: SwitchControllerLEDStatusEnum.On,
        //     //     p3: SwitchControllerLEDStatusEnum.Off,
        //     //     p4: SwitchControllerLEDStatusEnum.On
        //     // );
        // }
    }

    private void OnApplicationQuit()
    {
        // Debug.Log("Is SwitchControllerHID null: " + SwitchControllerHID.current);
        // if (SwitchControllerHID.current != null)
        // {
        //     SwitchControllerHID.current.SetVibrationEnabled(false);

        //     SwitchControllerHID.current.SetLEDs(
        //         p1: SwitchControllerLEDStatusEnum.On,
        //         p2: SwitchControllerLEDStatusEnum.Off,
        //         p3: SwitchControllerLEDStatusEnum.Off,
        //         p4: SwitchControllerLEDStatusEnum.Off
        //     );
        //     // SwitchJoyConLHID.current?.SetLEDs(
        //     //     p1: SwitchControllerLEDStatusEnum.On,
        //     //     p2: SwitchControllerLEDStatusEnum.Off,
        //     //     p3: SwitchControllerLEDStatusEnum.Off,
        //     //     p4: SwitchControllerLEDStatusEnum.Off
        //     // );
        // }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(m_movement.x, 0, m_movement.y));
        // rb.AddForce(new Vector3(m_movement.x, 0, m_movement.y));
        amp = Mathf.Clamp((Mathf.Sin(Time.time) + 1) / 2, 0f, 1f);
        ampOffset = (Mathf.Sin(Time.time + Mathf.PI) + 1) / 2;
    }

    IEnumerator RumbleCoroutine()
    {
        while (true)
        {
            SwitchControllerHID.current.Rumble(new SwitchControllerRumbleProfile
            {
                highBandFrequencyLeft = 160,
                highBandAmplitudeLeft = 0,
                
                lowBandFrequencyLeft = 160,
                lowBandAmplitudeLeft = 0,//amp * 0.1f,

                
                highBandFrequencyRight = 160,
                highBandAmplitudeRight = 0,
                
                lowBandFrequencyRight = 160,
                lowBandAmplitudeRight = amp * 0.5f
            });
            yield return new WaitForSeconds(1f);
        }
    }

    void OnFire()
    {
        Debug.Log("doing command");
        StartCoroutine(QuickRumble());
    }

    IEnumerator QuickRumble()
    {
        SwitchControllerHID.current.Rumble(new SwitchControllerRumbleProfile
        {
            lowBandFrequencyLeft = 0,
            lowBandAmplitudeLeft = 0,
            
            lowBandFrequencyRight = 0,
            lowBandAmplitudeRight = 0,
            
            highBandFrequencyRight = 150,
            highBandAmplitudeRight = 1
        });
        yield return new WaitForSeconds(0.1f);
        SwitchControllerHID.current.Rumble(new SwitchControllerRumbleProfile
        {
            lowBandFrequencyLeft = 0,
            lowBandAmplitudeLeft = 0,
            
            lowBandFrequencyRight = 0,
            lowBandAmplitudeRight = 0,
            
            highBandFrequencyRight = 115,
            highBandAmplitudeRight = 0
        });
    }
}

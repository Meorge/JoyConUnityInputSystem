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

        if (SwitchControllerHID.current != null)
        {
            SwitchControllerHID.current.SetVibrationEnabled(true);
            StartCoroutine(RumbleCoroutine());

            SwitchControllerHID.current.SetLEDs(
                p1: SwitchControllerLEDStatusEnum.Flashing,
                p2: SwitchControllerLEDStatusEnum.On,
                p3: SwitchControllerLEDStatusEnum.Off,
                p4: SwitchControllerLEDStatusEnum.On
            );
        }
    }

    private void OnApplicationQuit()
    {
        if (SwitchControllerHID.current != null)
        {

            SwitchControllerHID.current.SetLEDs(
                p1: SwitchControllerLEDStatusEnum.On,
                p2: SwitchControllerLEDStatusEnum.Off,
                p3: SwitchControllerLEDStatusEnum.Off,
                p4: SwitchControllerLEDStatusEnum.Off
            );
        }
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
                highBandFrequencyL = 160,
                highBandAmplitudeL = amp,
                
                lowBandFrequencyL = 160,
                lowBandAmplitudeL = 0,//amp * 0.1f,

                
                highBandFrequencyR = 160,
                highBandAmplitudeR = 0,
                
                lowBandFrequencyR = 160,
                lowBandAmplitudeR = amp * 0.1f
            });
            yield return new WaitForSeconds(1f);
        }
    }

    void OnMove(InputValue value)
    {
        var tempMovement = value.Get<Vector2>() * 0.1f;
        
        if (tempMovement.magnitude > 10)
        {
            Debug.Log($"Got a movement vector of {tempMovement}, too big so discarding");
            return;
        }
        m_movement = tempMovement;
    }

    void OnFire()
    {
        Debug.Log("doing command");
        StartCoroutine(QuickRumble());
    }

    void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Block")) return;
        Debug.Log("Hit something");
        //StartCoroutine(QuickRumble());
    }

    IEnumerator QuickRumble()
    {
        SwitchControllerHID.current.Rumble(new SwitchControllerRumbleProfile
        {
            lowBandFrequencyL = 0,
            lowBandAmplitudeL = 0,
            
            lowBandFrequencyR = 0,
            lowBandAmplitudeR = 0,
            
            highBandFrequencyR = 150,
            highBandAmplitudeR = 1
        });
        yield return new WaitForSeconds(0.1f);
        SwitchControllerHID.current.Rumble(new SwitchControllerRumbleProfile
        {
            lowBandFrequencyL = 0,
            lowBandAmplitudeL = 0,
            
            lowBandFrequencyR = 0,
            lowBandAmplitudeR = 0,
            
            highBandFrequencyR = 115,
            highBandAmplitudeR = 0
        });
    }
}

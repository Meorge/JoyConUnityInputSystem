using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput pInput = null;

    [SerializeField] Vector2 m_movement = new Vector2();

    float amp = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RumbleCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(m_movement.x, 0, m_movement.y));

        amp = (Mathf.Sin(Time.time) + 1) / 2;
        Debug.Log(amp);
    }

    IEnumerator RumbleCoroutine()
    {
        while (true)
        {
            SwitchJoyConRHID.current.Rumble(new SwitchJoyConRumbleProfile
            {
                highBandFrequencyR = 100,
                highBandAmplitudeR = 0,
                lowBandFrequencyR = 210,
                lowBandAmplitudeR = amp
            });
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnMove(InputValue value)
    {
        m_movement = value.Get<Vector2>() * 0.2f;
    }

    void OnFire()
    {
        Debug.Log("doing command");
        // SwitchJoyConRHID.current.Rumble(new SwitchJoyConRumbleProfile
        // {
        //     highBandFrequencyR = 320,
        //     highBandAmplitudeR = 0.0f,
        //     lowBandFrequencyR = 320,
        //     lowBandAmplitudeR = 0.8f
        // });
    }
}

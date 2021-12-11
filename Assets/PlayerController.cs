using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput pInput = null;

    [SerializeField] Vector2 m_movement = new Vector2();

    Rigidbody rb;

    float amp = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(m_movement.x, 0, m_movement.y));
        // rb.AddForce(new Vector3(m_movement.x, 0, m_movement.y));
        amp = (Mathf.Sin(Time.time) + 1) / 2;
    }

    // IEnumerator RumbleCoroutine()
    // {
    //     while (true)
    //     {
    //         SwitchControllerHID.current.Rumble(new SwitchJoyConRumbleProfile
    //         {
    //             highBandFrequencyR = 100,
    //             highBandAmplitudeR = 0,
    //             lowBandFrequencyR = 210,
    //             lowBandAmplitudeR = amp
    //         });
    //         yield return new WaitForSeconds(0.05f);
    //     }
    // }

    void OnMove(InputValue value)
    {
        var tempMovement = value.Get<Vector2>() * 0.1f;
        if (tempMovement.magnitude < 100) m_movement = tempMovement;
        Debug.Log(m_movement);
    }

    void OnFire()
    {
        Debug.Log("doing command");
    }

    void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Block")) return;
        Debug.Log("Hit something");
        StartCoroutine(QuickRumble());

        // rb.AddForce(c.impulse * 100f, ForceMode.Impulse);
    }

    IEnumerator QuickRumble()
    {
        SwitchControllerHID.current.Rumble(new SwitchJoyConRumbleProfile
        {
            highBandFrequencyR = 150,
            highBandAmplitudeR = 1
        });
        yield return new WaitForSeconds(0.05f);
        SwitchControllerHID.current.Rumble(new SwitchJoyConRumbleProfile
        {
            highBandFrequencyR = 115,
            highBandAmplitudeR = 0
        });
    }
}

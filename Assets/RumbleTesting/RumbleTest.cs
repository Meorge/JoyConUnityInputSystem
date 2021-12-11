using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.Switch;

public class RumbleTest : MonoBehaviour
{
    [SerializeField] private SwitchJoyConRumbleProfile m_profile = SwitchJoyConRumbleProfile.CreateEmpty();

    [SerializeField] private Slider m_hfFreq = null;
    [SerializeField] private Slider m_hfAmp = null;
    [SerializeField] private Slider m_lfFreq = null;
    [SerializeField] private Slider m_lfAmp = null;


    [SerializeField] private Toggle m_toggle = null;

    [SerializeField] private Image m_bodyColorImage = null;
    [SerializeField] private Image m_buttonColorImage = null;

    private SwitchControllerHID jc = null;

    private Coroutine playSongCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        m_hfFreq.value = 0;
        m_hfAmp.value = 0;
        m_lfFreq.value = 0;
        m_lfAmp.value = 0;

        OnHFFreqUpdated(m_hfFreq.value);
        OnHFAmpUpdated(m_hfAmp.value);
        OnLFFreqUpdated(m_lfFreq.value);
        OnLFAmpUpdated(m_lfAmp.value);
        jc = SwitchControllerHID.current;

        StartCoroutine(RumbleCoroutine());
    }

    void Update() {
        if (jc.buttonSouth.wasPressedThisFrame) {
            PlaySong();
        }

        m_bodyColorImage.color = jc.BodyColor;
        m_buttonColorImage.color = jc.ButtonColor;
    }

    void PlaySong()
    {
        if (playSongCoroutine != null)
        {
            StopCoroutine(playSongCoroutine);
            playSongCoroutine = null;
        }
        else
            playSongCoroutine = StartCoroutine(PlaySongCoroutine());
    }

    IEnumerator RumbleCoroutine()
    {
        while (true)
        {
            if (playSongCoroutine != null && m_toggle.isOn)
                jc.Rumble(m_profile);

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator PlaySongCoroutine()
    {
        var c = MusicalNote(523.25f);
        var csh = MusicalNote(554.37f);
        var d = MusicalNote(587.33f);
        var dsh = MusicalNote(622.25f);
        var e = MusicalNote(659.26f);
        var f = MusicalNote(698.46f);
        var fsh = MusicalNote(739.99f);
        var g = MusicalNote(783.99f);
        var gsh = MusicalNote(830.61f);
        var a = MusicalNote(880f);
        var ash = MusicalNote(932.33f);
        var b = MusicalNote(987.77f);
        var wait = 0.3f;

        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(c));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(e));
        yield return new WaitForSeconds(wait);
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(d));
        yield return new WaitForSeconds(wait);
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(g));
        yield return StartCoroutine(PlayNote(g));
        yield return new WaitForSeconds(wait);
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(c));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(e));
        yield return StartCoroutine(PlayNote(d));
        yield return StartCoroutine(PlayNote(c));
    }

    SwitchJoyConRumbleProfile MusicalNote(float note)
    {
        var a = SwitchJoyConRumbleProfile.CreateEmpty();
        a.highBandAmplitudeR = 1;
        a.highBandFrequencyR = note;
        return a;
    }

    IEnumerator PlayNote(SwitchJoyConRumbleProfile p)
    {
        jc.Rumble(p);
        yield return new WaitForSeconds(0.3f);

        jc.Rumble(SwitchJoyConRumbleProfile.CreateNeutral());
        yield return new WaitForSeconds(0.05f);
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

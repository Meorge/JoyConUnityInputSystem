using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowSliderValueLabel : MonoBehaviour
{
    private TextMeshProUGUI label = null;

    // Start is called before the first frame update
    void Start()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    public void OnValueChanged(float value)
    {
        label.text = $"{value}";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class WireLengthHUD : MonoBehaviour
{
    [SerializeField] private WireHolder wireHolder;
    [SerializeField] private RectTransform top;
    [SerializeField] private TextMeshProUGUI text;

    private Slider slider;

    private float topStartPosX;

    private Wire wire;

    void Awake()
    {
        wire = wireHolder.Wire;
        TryGetComponent<Slider>(out slider);
        wireHolder.SetNewWire += (newWire) => wire = newWire;
    }

    void Update()
    {
        text.text = (int)(wire.MaxLength - wire.TotalLength) + "m";
        float sliderValue = 1 - (wire.TotalLength / wire.MaxLength);
        slider.value = sliderValue;
        top.anchoredPosition = new Vector3((GetComponent<RectTransform>().rect.width + 2.5f) * sliderValue, 0, 0);
    }

}

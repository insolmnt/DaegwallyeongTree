using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageAlphaSlider : MonoBehaviour
{
    public RawImage Image;

    public void OnAlpahSliderChange(Slider slider)
    {
        Image.color = new Color(1, 1, 1, slider.value);
    }
}

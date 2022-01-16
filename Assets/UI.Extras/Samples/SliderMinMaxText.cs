using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMinMaxText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private void Awake()
    {
        Debug.Assert(text != null);
    }

    public void OnSliderMinMaxChange(float min, float max)
    {
        text.text = string.Format("({0:F2}, {1:F2})", min, max);
    }
}

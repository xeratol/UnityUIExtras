using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private void Awake()
    {
        Debug.Assert(text != null);
    }

    public void OnSliderChange(float val)
    {
        text.text = string.Format("{0:F2}", val);
    }
}

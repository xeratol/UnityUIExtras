using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider2DText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private void Awake()
    {
        Debug.Assert(text != null);
    }

    public void OnSlider2DChange(Vector2 val)
    {
        text.text = string.Format("({0:F2}, {1:F2})", val.x, val.y);
    }
}

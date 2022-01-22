using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extra;

public class ToggleGroupHandlerText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private void Awake()
    {
        Debug.Assert(text != null);
    }

    public void m_OnToggleSelected(int index)
    {
        if (index == -1)
        {
            text.text = string.Format("No option selected");
        }
        else
        {
            text.text = string.Format("Option Selected: {0}", index);
        }
    }
}

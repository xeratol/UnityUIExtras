using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.UI.Extra
{

    public class ToggleGroupHandler : MonoBehaviour
    {
        [SerializeField]
        private ToggleGroup m_group;

        [SerializeField]
        private Toggle[] m_toggles;

        [Serializable]
        public class ToggleGroupHandlerEvent : UnityEvent<int> { }

        [SerializeField]
        private ToggleGroupHandlerEvent m_OnToggleSelected = new ToggleGroupHandlerEvent();

        public ToggleGroupHandlerEvent onToggleSelected
        {
            get
            {
                return m_OnToggleSelected;
            }
            set
            {
                m_OnToggleSelected = value;
            }
        }

        private void Awake()
        {
            Debug.Assert(m_group != null);
        }

        private void Start()
        {
            for (var i = 0; i < m_toggles.Length; ++i)
            {
                var index = i;
                m_toggles[i].onValueChanged.AddListener((bool state) =>
                {
                    if (!m_group.AnyTogglesOn())
                    {
                        m_OnToggleSelected.Invoke(-1);
                        return;
                    }

                    if (!state)
                    {
                        return;
                    }

                    m_OnToggleSelected.Invoke(index);
                });
            }
        }
    }
}

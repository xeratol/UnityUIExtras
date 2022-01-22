using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extra; // ToggleGroupHandlerEvent

namespace UnityEditor.UI.Extra
{
    [CustomEditor(typeof(ToggleGroupHandler), true)]
    [CanEditMultipleObjects]
    public class ToggleGroupHandlerEditor : Editor
    {
        SerializedProperty m_group;
        SerializedProperty m_toggles;
        SerializedProperty m_OnToggleSelected;

        protected void OnEnable()
        {
            m_group = serializedObject.FindProperty("m_group");
            m_toggles = serializedObject.FindProperty("m_toggles");
            m_OnToggleSelected = serializedObject.FindProperty("m_OnToggleSelected");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_group);
            EditorGUILayout.PropertyField(m_toggles);
            EditorGUILayout.PropertyField(m_OnToggleSelected);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

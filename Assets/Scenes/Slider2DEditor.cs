using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extra; // Slider2D

namespace UnityEditor.UI.Extra
{
    [CustomEditor(typeof(Slider2D), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Slider Component.
    /// Extend this class to write a custom editor for a component derived from Slider.
    /// </summary>
    public class Slider2DEditor : SelectableEditor
    {
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleRect;
        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_WholeNumbers;
        SerializedProperty m_Value;
        SerializedProperty m_OnValueChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FillRect = serializedObject.FindProperty("m_FillRect");
            m_HandleRect = serializedObject.FindProperty("m_HandleRect");
            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
            m_Value = serializedObject.FindProperty("m_Value");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
        }

        private bool IsLessOrEqual(Vector2 v1, Vector2 v2)
        {
            return (v1.x <= v2.x || v1.y <= v2.y);
        }

        private bool IsGreaterOrEqual(Vector2 v1, Vector2 v2)
        {
            return (v1.x >= v2.x || v1.y >= v2.y);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleRect);

            if (m_FillRect.objectReferenceValue != null || m_HandleRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.BeginChangeCheck();
                var newMin = EditorGUILayout.Vector2Field("Min Value", m_MinValue.vector2Value);
                if (EditorGUI.EndChangeCheck() && IsLessOrEqual(newMin, m_MaxValue.vector2Value))
                {
                    m_MinValue.vector2Value = newMin;
                }

                EditorGUI.BeginChangeCheck();
                var newMax = EditorGUILayout.Vector2Field("Max Value", m_MaxValue.vector2Value);
                if (EditorGUI.EndChangeCheck() && IsGreaterOrEqual(newMax, m_MinValue.vector2Value))
                {
                    m_MaxValue.vector2Value = newMax;
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);
                m_Value.vector2Value = EditorGUILayout.Vector2Field("Value", m_Value.vector2Value);

                // Draw the event notification options
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider 2d fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

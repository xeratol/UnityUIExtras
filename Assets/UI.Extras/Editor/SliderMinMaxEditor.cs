using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extra; // SliderMinMax

namespace UnityEditor.UI.Extra
{
    [CustomEditor(typeof(SliderMinMax), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Slider Component.
    /// Extend this class to write a custom editor for a component derived from Slider.
    /// </summary>
    public class SliderMinMaxEditor : SelectableEditor
    {
        SerializedProperty m_Direction;
        SerializedProperty m_FillRect;
        SerializedProperty m_HandleUpperRect;
        SerializedProperty m_HandleLowerRect;
        SerializedProperty m_MinValue;
        SerializedProperty m_MaxValue;
        SerializedProperty m_WholeNumbers;
        SerializedProperty m_UpperValue;
        SerializedProperty m_LowerValue;
        SerializedProperty m_GapValue;
        SerializedProperty m_OnValueChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_FillRect = serializedObject.FindProperty("m_FillRect");
            m_HandleUpperRect = serializedObject.FindProperty("m_HandleUpperRect");
            m_HandleLowerRect = serializedObject.FindProperty("m_HandleLowerRect");
            m_Direction = serializedObject.FindProperty("m_Direction");
            m_MinValue = serializedObject.FindProperty("m_MinValue");
            m_MaxValue = serializedObject.FindProperty("m_MaxValue");
            m_WholeNumbers = serializedObject.FindProperty("m_WholeNumbers");
            m_UpperValue = serializedObject.FindProperty("m_UpperValue");
            m_LowerValue = serializedObject.FindProperty("m_LowerValue");
            m_GapValue = serializedObject.FindProperty("m_GapValue");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_FillRect);
            EditorGUILayout.PropertyField(m_HandleUpperRect);
            EditorGUILayout.PropertyField(m_HandleLowerRect);

            if (m_FillRect.objectReferenceValue != null || m_HandleUpperRect.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Direction);
                if (EditorGUI.EndChangeCheck())
                {
                    SliderMinMax.Direction direction = (SliderMinMax.Direction)m_Direction.enumValueIndex;
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        SliderMinMax slider = obj as SliderMinMax;
                        slider.SetDirection(direction, true);
                    }
                }

                EditorGUI.BeginChangeCheck();
                float newMin = EditorGUILayout.FloatField("Min Value", m_MinValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMin <= m_MaxValue.floatValue)
                {
                    m_MinValue.floatValue = newMin;
                }

                EditorGUI.BeginChangeCheck();
                float newMax = EditorGUILayout.FloatField("Max Value", m_MaxValue.floatValue);
                if (EditorGUI.EndChangeCheck() && newMax >= m_MinValue.floatValue)
                {
                    m_MaxValue.floatValue = newMax;
                }

                EditorGUILayout.PropertyField(m_WholeNumbers);
                DisplayGap();
                EditorGUILayout.Slider(m_LowerValue, m_MinValue.floatValue, m_MaxValue.floatValue);
                EditorGUILayout.Slider(m_UpperValue, m_MinValue.floatValue, m_MaxValue.floatValue);

                bool warning = false;
                foreach (var obj in serializedObject.targetObjects)
                {
                    SliderMinMax slider = obj as SliderMinMax;
                    SliderMinMax.Direction dir = slider.direction;
                    if (dir == SliderMinMax.Direction.LeftToRight || dir == SliderMinMax.Direction.RightToLeft)
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnLeft() != null || slider.FindSelectableOnRight() != null));
                    else
                        warning = (slider.navigation.mode != Navigation.Mode.Automatic && (slider.FindSelectableOnDown() != null || slider.FindSelectableOnUp() != null));
                }

                if (warning)
                    EditorGUILayout.HelpBox("The selected slider direction conflicts with navigation. Not all navigation options may work.", MessageType.Warning);

                // Draw the event notification options
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OnValueChanged);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a RectTransform for the slider fill or the slider handle or both. Each must have a parent RectTransform that it can slide within.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayGap()
        {
            var stepSize = m_WholeNumbers.boolValue ? 1f : 0f;
            m_GapValue.floatValue = EditorGUILayout.Slider(new GUIContent(m_GapValue.displayName, "Gap between upper and lower values."), m_GapValue.floatValue, stepSize, m_MaxValue.floatValue);

            if (Mathf.Approximately(m_GapValue.floatValue, 0f))
            {
                EditorGUILayout.HelpBox("Consider using a Slider if your gap value is 0.", MessageType.Warning);
            }
            if (Mathf.Approximately(m_GapValue.floatValue, m_MaxValue.floatValue))
            {
                EditorGUILayout.HelpBox("Handles will always be on extremes. Consider lowering the gap value.", MessageType.Warning);
            }

            var currentGap = m_UpperValue.floatValue - m_LowerValue.floatValue;
            if (currentGap < m_GapValue.floatValue)
            {
                var midValue = m_LowerValue.floatValue + currentGap / 2;
                m_UpperValue.floatValue = midValue + m_GapValue.floatValue * 0.5f;
                m_LowerValue.floatValue = midValue - m_GapValue.floatValue * 0.5f;

                if (m_UpperValue.floatValue > m_MaxValue.floatValue)
                {
                    m_UpperValue.floatValue = m_MaxValue.floatValue;
                    m_LowerValue.floatValue = m_MaxValue.floatValue - m_GapValue.floatValue;
                }
                else if (m_LowerValue.floatValue < m_MinValue.floatValue)
                {
                    m_UpperValue.floatValue = m_MinValue.floatValue + m_GapValue.floatValue;
                    m_LowerValue.floatValue = m_MinValue.floatValue;
                }
            }
        }
    }
}

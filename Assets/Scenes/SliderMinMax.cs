using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extra
{
    [AddComponentMenu("UI/SliderMinMax", 33)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// A standard slider that can be moved between a minimum and maximum value.
    /// </summary>
    /// <remarks>
    /// The slider component is a Selectable that controls a fill, a handle, or both. The fill, when used, spans from the minimum value to the current value while the handle, when used, follow the current value.
    /// The anchors of the fill and handle RectTransforms are driven by the Slider. The fill and handle can be direct children of the GameObject with the Slider, or intermediary RectTransforms can be placed in between for additional control.
    /// When a change to the slider value occurs, a callback is sent to any registered listeners of UI.Slider.onValueChanged.
    /// </remarks>
    public class SliderMinMax : Selectable, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        /// <summary>
        /// Setting that indicates one of four directions.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// From the left to the right
            /// </summary>
            LeftToRight,

            /// <summary>
            /// From the right to the left
            /// </summary>
            RightToLeft,

            /// <summary>
            /// From the bottom to the top.
            /// </summary>
            BottomToTop,

            /// <summary>
            /// From the top to the bottom.
            /// </summary>
            TopToBottom,
        }

        [Serializable]
        /// <summary>
        /// Event type used by the UI.Slider.
        /// </summary>
        public class SliderMinMaxEvent : UnityEvent<float, float> { }

        [SerializeField]
        private RectTransform m_FillRect;

        /// <summary>
        /// Optional RectTransform to use as fill for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform"(Child of FillArea).
        ///     public RectTransform newFillRect;
        ///
        ///     //Deactivates the old FillRect and assigns a new one.
        ///     void Start()
        ///     {
        ///         mainSlider.fillRect.gameObject.SetActive(false);
        ///         mainSlider.fillRect = newFillRect;
        ///     }
        /// }
        /// </code>
        /// </example>
        public RectTransform fillRect
        {
            get
            {
                return m_FillRect;
            }
            set
            {
                //if (SetPropertyUtility.SetClass(ref m_FillRect, value))
                if (m_FillRect != value)
                {
                    m_FillRect = value;
                    UpdateCachedReferences();
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        private RectTransform m_HandleUpperRect;

        /// <summary>
        /// Optional RectTransform to use as a handle for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform" (Child of "Handle Slide Area").
        ///     public RectTransform handleHighlighted;
        ///
        ///     //Deactivates the old Handle, then assigns and enables the new one.
        ///     void Start()
        ///     {
        ///         mainSlider.handleRect.gameObject.SetActive(false);
        ///         mainSlider.handleRect = handleHighlighted;
        ///         mainSlider.handleRect.gameObject.SetActive(true);
        ///     }
        /// }
        /// </code>
        /// </example>
        public RectTransform handleUpperRect
        {
            get { return m_HandleUpperRect; }
            set
            {
                //if (SetPropertyUtility.SetClass(ref m_HandleRect, value))
                if (m_HandleUpperRect != value)
                {
                    m_HandleUpperRect = value;
                    UpdateCachedReferences();
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        private RectTransform m_HandleLowerRect;

        /// <summary>
        /// Optional RectTransform to use as a handle for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform" (Child of "Handle Slide Area").
        ///     public RectTransform handleHighlighted;
        ///
        ///     //Deactivates the old Handle, then assigns and enables the new one.
        ///     void Start()
        ///     {
        ///         mainSlider.handleRect.gameObject.SetActive(false);
        ///         mainSlider.handleRect = handleHighlighted;
        ///         mainSlider.handleRect.gameObject.SetActive(true);
        ///     }
        /// }
        /// </code>
        /// </example>
        public RectTransform handleLowerRect
        {
            get { return m_HandleLowerRect; }
            set
            {
                //if (SetPropertyUtility.SetClass(ref m_HandleRect, value))
                if (m_HandleLowerRect != value)
                {
                    m_HandleLowerRect = value;
                    UpdateCachedReferences();
                    UpdateVisuals();
                }
            }
        }

        [Space]

        [SerializeField]
        private Direction m_Direction = Direction.LeftToRight;

        /// <summary>
        /// The direction of the slider, from minimum to maximum value.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //Changes the direction of the slider.
        ///         if (mainSlider.direction == Slider.Direction.BottomToTop)
        ///         {
        ///             mainSlider.direction = Slider.Direction.TopToBottom;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public Direction direction
        {
            get { return m_Direction; }
            set
            {
                //if (SetPropertyUtility.SetStruct(ref m_Direction, value))
                if (m_Direction != value)
                {
                    m_Direction = value;
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        private float m_MinValue = 0;

        /// <summary>
        /// The minimum allowed value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     void Start()
        ///     {
        ///         // Changes the minimum value of the slider to 10;
        ///         mainSlider.minValue = 10;
        ///     }
        /// }
        /// </code>
        /// </example>
        public float minValue
        {
            get { return m_MinValue; }
            set
            {
                //if (SetPropertyUtility.SetStruct(ref m_MinValue, value))
                if (m_MinValue != value)
                {
                    m_MinValue = value;
                    SetUpper(m_UpperValue);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        private float m_MaxValue = 1;

        /// <summary>
        /// The maximum allowed value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     void Start()
        ///     {
        ///         // Changes the max value of the slider to 20;
        ///         mainSlider.maxValue = 20;
        ///     }
        /// }
        /// </code>
        /// </example>
        public float maxValue
        {
            get { return m_MaxValue; }
            set
            {
                //if (SetPropertyUtility.SetStruct(ref m_MaxValue, value))
                if (m_MaxValue != value)
                {
                    m_MaxValue = value;
                    SetUpper(m_UpperValue);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        private bool m_WholeNumbers = false;

        /// <summary>
        /// Should the value only be allowed to be whole numbers?
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //sets the slider's value to accept whole numbers only.
        ///         mainSlider.wholeNumbers = true;
        ///     }
        /// }
        /// </code>
        /// </example>
        public bool wholeNumbers
        {
            get { return m_WholeNumbers; }
            set
            {
                //if (SetPropertyUtility.SetStruct(ref m_WholeNumbers, value))
                if (m_WholeNumbers != value)
                {
                    m_WholeNumbers = value;
                    SetUpper(m_UpperValue);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField]
        protected float m_UpperValue;

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     //Invoked when a submit button is clicked.
        ///     public void SubmitSliderSetting()
        ///     {
        ///         //Displays the value of the slider in the console.
        ///         Debug.Log(mainSlider.value);
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual float upperValue
        {
            get
            {
                return wholeNumbers ? Mathf.Round(m_UpperValue) : m_UpperValue;
            }
            set
            {
                SetUpper(value);
            }
        }

        [SerializeField]
        protected float m_LowerValue;

        /// <summary>
        /// The current value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     //Invoked when a submit button is clicked.
        ///     public void SubmitSliderSetting()
        ///     {
        ///         //Displays the value of the slider in the console.
        ///         Debug.Log(mainSlider.value);
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual float lowerValue
        {
            get
            {
                return wholeNumbers ? Mathf.Round(m_LowerValue) : m_LowerValue;
            }
            set
            {
                SetLower(value);
            }
        }

        /// <summary>
        /// Set the value of the slider without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        public virtual void SetValueWithoutNotify(float input)
        {
            SetUpper(input, false);
        }

        /// <summary>
        /// The current value of the slider normalized into a value between 0 and 1.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     //Set to invoke when "OnValueChanged" method is called.
        ///     void CheckNormalisedValue()
        ///     {
        ///         //Displays the normalised value of the slider everytime the value changes.
        ///         Debug.Log(mainSlider.normalizedValue);
        ///     }
        /// }
        /// </code>
        /// </example>
        public float normalizedUpperValue
        {
            get
            {
                if (Mathf.Approximately(minValue, maxValue))
                    return 0;
                return Mathf.InverseLerp(minValue, maxValue, upperValue);
            }
            set
            {
                this.upperValue = Mathf.Lerp(minValue, maxValue, value);
            }
        }

        /// <summary>
        /// The current value of the slider normalized into a value between 0 and 1.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     //Set to invoke when "OnValueChanged" method is called.
        ///     void CheckNormalisedValue()
        ///     {
        ///         //Displays the normalised value of the slider everytime the value changes.
        ///         Debug.Log(mainSlider.normalizedValue);
        ///     }
        /// }
        /// </code>
        /// </example>
        public float normalizedLowerValue
        {
            get
            {
                if (Mathf.Approximately(minValue, maxValue))
                    return 0;
                return Mathf.InverseLerp(minValue, maxValue, lowerValue);
            }
            set
            {
                this.lowerValue = Mathf.Lerp(minValue, maxValue, value);
            }
        }

        [SerializeField]
        private float m_GapValue = 0.1f;

        public virtual float gapValue
        {
            get
            {
                return wholeNumbers ? Mathf.Round(m_GapValue) : m_GapValue;
            }
            set
            {
                // TODO
                m_GapValue = value;
            }
        }

        [Space]

        [SerializeField]
        private SliderMinMaxEvent m_OnValueChanged = new SliderMinMaxEvent();

        /// <summary>
        /// Callback executed when the value of the slider is changed.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //Adds a listener to the main slider and invokes a method when the value changes.
        ///         mainSlider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        ///     }
        ///
        ///     // Invoked when the value of the slider changes.
        ///     public void ValueChangeCheck()
        ///     {
        ///         Debug.Log(mainSlider.value);
        ///     }
        /// }
        /// </code>
        /// </example>
        public SliderMinMaxEvent onValueChanged
        {
            get { return m_OnValueChanged; }
            set { m_OnValueChanged = value; }
        }

        // Private fields

        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private RectTransform m_HandlesContainerRect;
        private RectTransform m_SelectedHandle;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        // field is never assigned warning
#pragma warning disable 649
        private DrivenRectTransformTracker m_Tracker;
#pragma warning restore 649

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals = false;

        // Size of each step.
        float stepSize { get { return wholeNumbers ? 1 : (maxValue - minValue) * 0.1f; } }

        protected SliderMinMax()
        { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
                m_GapValue = Mathf.Round(m_GapValue);
                // TODO max-min >= gap
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                // Update rects in next update since other things might affect them even if value didn't change.
                m_DelayedUpdateVisuals = true;
            }

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(lowerValue, upperValue);
#endif
        }

        /// <summary>
        /// See ICanvasElement.LayoutComplete
        /// </summary>
        public virtual void LayoutComplete()
        { }

        /// <summary>
        /// See ICanvasElement.GraphicUpdateComplete
        /// </summary>
        public virtual void GraphicUpdateComplete()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            SetUpper(m_UpperValue, false);
            SetLower(m_LowerValue, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        /// <summary>
        /// Update the rect based on the delayed update visuals.
        /// Got around issue of calling sendMessage from onValidate.
        /// </summary>
        protected virtual void Update()
        {
            if (m_DelayedUpdateVisuals)
            {
                m_DelayedUpdateVisuals = false;
                SetUpper(m_UpperValue, false);
                SetLower(m_LowerValue, false);
                UpdateVisuals();
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_UpperValue = ClampValue(m_UpperValue);
            float oldNormalizedValue = normalizedUpperValue;
            if (m_FillContainerRect != null)
            {
                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                    oldNormalizedValue = m_FillImage.fillAmount;
                else
                    oldNormalizedValue = (reverseValue ? 1 - m_FillRect.anchorMin[(int)axis] : m_FillRect.anchorMax[(int)axis]);
            }
            else if (m_HandlesContainerRect != null)
                oldNormalizedValue = (reverseValue ? 1 - m_HandleUpperRect.anchorMin[(int)axis] : m_HandleUpperRect.anchorMin[(int)axis]);

            // TODO oldNormalizedValue for Lower

            UpdateVisuals();

            if (oldNormalizedValue != normalizedUpperValue)
            {
                UISystemProfilerApi.AddMarker("SliderMinMax.upperValue", this);
                onValueChanged.Invoke(m_LowerValue, m_UpperValue);
            }
        }

        void UpdateCachedReferences()
        {
            if (m_FillRect && m_FillRect != (RectTransform)transform)
            {
                m_FillTransform = m_FillRect.transform;
                m_FillImage = m_FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_FillRect = null;
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            m_HandlesContainerRect = null;
            RectTransform handleUpperTransformParent = null;
            RectTransform handleLowerTransformParent = null;

            if (m_HandleUpperRect && m_HandleUpperRect != (RectTransform)transform)
            {
                var handleUpperTransform = m_HandleUpperRect.transform;
                if (handleUpperTransform.parent != null)
                    handleUpperTransformParent = handleUpperTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleUpperRect = null;
            }

            if (m_HandleLowerRect && m_HandleLowerRect != (RectTransform)transform)
            {
                var handleLowerTransform = m_HandleLowerRect.transform;
                if (handleLowerTransform.parent != null)
                    handleLowerTransformParent = handleLowerTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleLowerRect = null;
            }

            if (handleUpperTransformParent != handleLowerTransformParent)
            {
                Debug.LogWarning("Parents of upper and lower handle are different", this);
            }
            else
            {
                m_HandlesContainerRect = handleUpperTransformParent;
            }
        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            if (wholeNumbers)
                newValue = Mathf.Round(newValue);
            return newValue;
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        protected virtual void SetUpper(float input, bool sendCallback = true)
        {
            // Clamp the input
            float newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_UpperValue == newValue)
                return;

            // Adjust the lower value if needed
            if (m_LowerValue > newValue - gapValue)
            {
                var newLowerValue = ClampValue(newValue - gapValue);
                newValue = newLowerValue + gapValue;
                m_LowerValue = newLowerValue; // TODO is this safe?
            }

            m_UpperValue = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("SliderMinMax.upperValue", this);
                m_OnValueChanged.Invoke(m_LowerValue, m_UpperValue);
            }
        }

        /// <summary>
        /// Set the value of the slider.
        /// </summary>
        /// <param name="input">The new value for the slider.</param>
        /// <param name="sendCallback">If the OnValueChanged callback should be invoked.</param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        protected virtual void SetLower(float input, bool sendCallback = true)
        {
            // Clamp the input
            float newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_LowerValue == newValue)
                return;

            // Adjust the upper value if needed
            if (m_UpperValue < newValue + gapValue)
            {
                var newUpperValue = ClampValue(newValue + gapValue);
                newValue = newUpperValue - gapValue;
                m_UpperValue = newUpperValue; // TODO is this safe?
            }

            m_LowerValue = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("SliderMinMax.lowerValue", this);
                m_OnValueChanged.Invoke(m_LowerValue, m_UpperValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        Axis axis
        {
            get
            {
                return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical;
            }
        }
        bool reverseValue
        {
            get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; }
        }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = normalizedUpperValue;
                }
                else
                {
                    if (reverseValue)
                    {
                        anchorMin[(int)axis] = 1 - normalizedUpperValue;
                        anchorMax[(int)axis] = 1 - normalizedLowerValue;
                    }
                    else
                    {
                        anchorMin[(int)axis] = normalizedLowerValue;
                        anchorMax[(int)axis] = normalizedUpperValue;
                    }
                }

                m_FillRect.anchorMin = anchorMin;
                m_FillRect.anchorMax = anchorMax;
            }

            if (m_HandlesContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleUpperRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[(int)axis] = anchorMax[(int)axis] = (reverseValue ? (1 - normalizedUpperValue) : normalizedUpperValue);
                m_HandleUpperRect.anchorMin = anchorMin;
                m_HandleUpperRect.anchorMax = anchorMax;

                m_Tracker.Add(this, m_HandleLowerRect, DrivenTransformProperties.Anchors);
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
                anchorMin[(int)axis] = anchorMax[(int)axis] = (reverseValue ? (1 - normalizedLowerValue) : normalizedLowerValue);
                m_HandleLowerRect.anchorMin = anchorMin;
                m_HandleLowerRect.anchorMax = anchorMax;
            }
        }

        bool GetNormalizedValueOfCursorLocation(PointerEventData eventData, Camera cam, RectTransform clickRect, ref float val)
        {
            // TODO no idea how this applies for SliderMinMax yet
            //Vector2 position = Vector2.zero;
            //if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
            //    return;
            var position = eventData.position;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out localCursor))
                return false;
            localCursor -= clickRect.rect.position;

            val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / clickRect.rect.size[(int)axis]);
            if (reverseValue)
            {
                val = 1f - val;
            }

            return true;
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandlesContainerRect ?? m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
            {
                var val = 0f;
                if (!GetNormalizedValueOfCursorLocation(eventData, cam, clickRect, ref val))
                {
                    return;
                }

                if (m_SelectedHandle == null)
                {
                    var upperValDist = Mathf.Abs(val - normalizedUpperValue);
                    var lowerValDist = Mathf.Abs(val - normalizedLowerValue);
                    if (upperValDist < lowerValDist)
                    {
                        normalizedUpperValue = val;
                        m_SelectedHandle = m_HandleUpperRect;
                    }
                    else
                    {
                        normalizedLowerValue = val;
                        m_SelectedHandle = m_HandleLowerRect;
                    }
                }
                else
                {
                    if (m_SelectedHandle == m_HandleUpperRect)
                    {
                        normalizedUpperValue = val;
                    }
                    else if (m_SelectedHandle == m_HandleLowerRect)
                    {
                        normalizedLowerValue = val;
                    }
                }
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            var isHandled = false;
            m_Offset = Vector2.zero;
            if (m_HandlesContainerRect != null)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(m_HandleUpperRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
                {
                    Vector2 localMousePos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleUpperRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    {
                        m_Offset = localMousePos;
                        m_SelectedHandle = m_HandleUpperRect;
                    }
                    isHandled = true;
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(m_HandleLowerRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
                {
                    Vector2 localMousePos;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleLowerRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out localMousePos))
                    {
                        m_Offset = localMousePos;
                        m_SelectedHandle = m_HandleLowerRect;
                    }
                    isHandled = true;
                }
            }

            if (!isHandled)
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            m_SelectedHandle = null;
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                        SetUpper(reverseValue ? upperValue + stepSize : upperValue - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        SetUpper(reverseValue ? upperValue - stepSize : upperValue + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        SetUpper(reverseValue ? upperValue - stepSize : upperValue + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        SetUpper(reverseValue ? upperValue + stepSize : upperValue - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        /// <summary>
        /// See Selectable.FindSelectableOnLeft
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnRight
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnUp
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnDown
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Sets the direction of this slider, optionally changing the layout as well.
        /// </summary>
        /// <param name="direction">The direction of the slider</param>
        /// <param name="includeRectLayouts">Should the layout be flipped together with the slider direction</param>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         mainSlider.SetDirection(Slider.Direction.LeftToRight, false);
        ///     }
        /// }
        /// </code>
        /// </example>
        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }
    }
}

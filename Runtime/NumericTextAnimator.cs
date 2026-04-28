using System;
using TMPro;
using UnityEngine;

namespace Aori.UIRework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class NumericTextAnimator : MonoBehaviour
    {
        private const float EPSILON = 0.01f;

        [Header("- Settings")]
        [SerializeField]
        [Range(1f, 30f)]
        private float m_transitionSpeed = 7.98f;

        [SerializeField]
        [Range(0, 10)]
        private int m_decimalPlaces;

        private TextMeshProUGUI m_text;
        private bool m_isAnimating;
        private float m_currentValue;
        private float m_targetValue;

        public string Prefix { get; set; }
        public string Suffix { get; set; }

        public int DecimalPlaces
        {
            get => m_decimalPlaces;
            set => m_decimalPlaces = value;
        }

        public event EventHandler OnValueReached;

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (m_isAnimating)
            {
                HandleAnimation();
            }
        }

        private void HandleAnimation()
        {
            m_currentValue = Mathf.Lerp(m_currentValue, m_targetValue, m_transitionSpeed * Time.deltaTime);
            if (Mathf.Abs(m_currentValue - m_targetValue) < EPSILON)
            {
                m_currentValue = m_targetValue;
                m_isAnimating = false;
                OnValueReached?.Invoke(sender: this, e: EventArgs.Empty);
            }

            m_text.text = $"{Prefix}{m_currentValue.ToString(format: $"F{m_decimalPlaces}")}{Suffix}";
        }

        public void AnimateWithEntries(float start, float target)
        {
            m_isAnimating = true;
            m_targetValue = target;
            m_currentValue = start;
        }

        public void ForceValueNoAnimation(float value)
        {
            m_isAnimating = false;
            m_targetValue = value;
            m_currentValue = value;
            m_text.text = $"{Prefix}{m_currentValue:F(m_decimalPlaces)}{Suffix}";
            OnValueReached?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
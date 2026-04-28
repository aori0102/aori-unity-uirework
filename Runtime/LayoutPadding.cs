using System;
using UnityEngine;

namespace Aori.UIRework
{
    [Serializable]
    public struct LayoutPadding
    {
        [SerializeField]
        [Min(0f)]
        private float m_top;

        [SerializeField]
        [Min(0f)]
        private float m_bottom;

        [SerializeField]
        [Min(0f)]
        private float m_left;

        [SerializeField]
        [Min(0f)]
        private float m_right;

        public float Top => m_top;
        public float Bottom => m_bottom;
        public float Left => m_left;
        public float Right => m_right;

        public LayoutPadding(float top, float bottom, float left, float right)
        {
            m_top = top;
            m_bottom = bottom;
            m_left = left;
            m_right = right;
        }

        public void ClampNonNegative()
        {
            m_top = Mathf.Max(0f, m_top);
            m_bottom = Mathf.Max(0f, m_bottom);
            m_left = Mathf.Max(0f, m_left);
            m_right = Mathf.Max(0f, m_right);
        }
    }
}
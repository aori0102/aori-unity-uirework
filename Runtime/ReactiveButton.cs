using UnityEngine;
using UnityEngine.EventSystems;

namespace Aori.UIRework
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public sealed class ReactiveButton : MonoBehaviour,
        IPointerUpHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [Header("- Target")]
        [SerializeField]
        private RectTransform m_reactiveTarget;

        [Header("- Settings")]
        [SerializeField]
        [Range(0.5f, 3f)]
        private float m_pointerDownScale = 0.89f;

        [SerializeField]
        [Range(0.5f, 3f)]
        private float m_pointerEnterScale = 1.32f;

        [SerializeField]
        [Range(1f, 10f)]
        private float m_buttonReactiveRate = 5.7f;

        private Vector2 m_currentSize;
        private Vector2 m_defaultSize;
        private Vector2 m_targetSize;

        private bool _isPointerDown;

        private bool IsPointerDown
        {
            set
            {
                _isPointerDown = value;
                UpdateTargetScale();
            }
        }

        private bool _isPointerEnter;

        private bool IsPointerEnter
        {
            set
            {
                _isPointerEnter = value;
                UpdateTargetScale();
            }
        }

        private void Awake()
        {
            if (!m_reactiveTarget)
            {
                return;
            }
            m_defaultSize = m_reactiveTarget.rect.size;
            m_currentSize = m_defaultSize;
            m_targetSize = m_defaultSize;
        }

        private void LateUpdate()
        {
            if (!m_reactiveTarget)
            {
                return;
            }

            m_currentSize
                = Vector2.Lerp(m_currentSize, m_targetSize, Time.deltaTime * m_buttonReactiveRate);
            m_reactiveTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_currentSize.x);
            m_reactiveTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_currentSize.y);
        }

        private void UpdateTargetScale()
        {
            if (_isPointerDown)
            {
                m_targetSize = m_defaultSize * m_pointerDownScale;
            }
            else if (_isPointerEnter)
            {
                m_targetSize = m_defaultSize * m_pointerEnterScale;
            }
            else
            {
                m_targetSize = m_defaultSize;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPointerDown = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPointerDown = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsPointerEnter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPointerEnter = false;
        }
    }
}
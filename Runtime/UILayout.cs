using System.Collections.Generic;
using UnityEngine;

namespace Aori
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)]
    public abstract class UILayout : MonoBehaviour
    {
        protected RectTransform m_rectTransform;

        private Canvas m_canvas;
        
        private readonly Dictionary<Transform, bool> m_childActiveStates = new();

#if UNITY_EDITOR
        private bool m_isDelayCallRegistered;
#endif

        protected virtual void OnEnable()
        {
            SetDirty();
        }

        private void OnDisable()
        {
            SetDirty();
        }

        protected virtual void LateUpdate()
        {
            if (HasAnyChildChanged())
            {
                SetDirty();
            }
        }

        protected virtual void OnValidate()
        {
            HandleValidate();
            SetDirty();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            SetDirty();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void OnTransformParentChanged()
        {
            TryResolveCanvas();
        }

        private bool TryResolveCanvas()
        {
            m_canvas = GetComponentInParent<Canvas>(true);
            return m_canvas;
        }

        protected void SetDirty()
        {
            if (!this)
            {
                return;
            }

            if (!m_canvas && !TryResolveCanvas())
            {
                return;
            }

            DirtyCanvasProcessor.EnsureInstance();
            DirtyCanvasProcessor.RegisterDirtyCanvas(m_canvas);
        }

        private bool HasAnyChildChanged()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child.hasChanged)
                {
                    child.hasChanged = false;
                    return true;
                }

                var active = child.gameObject.activeInHierarchy;

                if (!m_childActiveStates.TryGetValue(child, out var prev) || prev != active)
                {
                    m_childActiveStates[child] = active;
                    return true;
                }
            }

            return false;
        }

        private bool TryCacheRectTransform()
        {
            if (m_rectTransform)
            {
                return true;
            }

            if (!this || !gameObject)
            {
                return false;
            }

            m_rectTransform = GetComponent<RectTransform>();

            if (m_rectTransform)
            {
                return true;
            }

            Debug.LogWarning($"{GetType().Name} requires a RectTransform.", this);
            return false;
        }

        protected virtual void HandleValidate()
        { }

        public virtual LayoutPadding GetLayoutPadding()
        {
            return default;
        }

        protected abstract void ApplyLayout();

        private void PerformLayoutApplication()
        {
            if (!this || !gameObject)
            {
                return;
            }

            if (TryCacheRectTransform())
            {
                ApplyLayout();
            }
        }

        public void ExecuteDirtyApply()
        {
            if (!this || !gameObject || !isActiveAndEnabled)
            {
                return;
            }

            PerformLayoutApplication();
        }
    }
}
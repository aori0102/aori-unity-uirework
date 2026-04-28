using TMPro;
using UnityEngine;

namespace Aori
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class TMPScaler : UILayout
    {
        [Header("- Fit Axis")]
        [SerializeField]
        private bool m_resizeVertical = true;

        [SerializeField]
        private bool m_resizeHorizontal = true;

        private TMP_Text m_tmpText;

        protected override void OnEnable()
        {
            base.OnEnable();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(HandleTmpTextChanged);
        }

        private void OnDestroy()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(HandleTmpTextChanged);
        }

        protected override void ApplyLayout()
        {
            if (!TryGetComponent(out m_tmpText) || !m_tmpText)
            {
                return;
            }

            m_tmpText.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();

            var textBounds = m_tmpText.textBounds;
            var hasNoCharacters = string.IsNullOrEmpty(m_tmpText.text) || m_tmpText.textInfo.characterCount == 0;
            var hasInvalidBounds = !float.IsFinite(textBounds.min.x)
                                   || !float.IsFinite(textBounds.min.y)
                                   || !float.IsFinite(textBounds.max.x)
                                   || !float.IsFinite(textBounds.max.y);

            var contentWidth = hasNoCharacters || hasInvalidBounds ? 0f : textBounds.size.x;
            var contentHeight = hasNoCharacters || hasInvalidBounds ? 0f : textBounds.size.y;

            var padding = CollectSiblingPadding();
            var totalWidth = contentWidth + padding.Left + padding.Right;
            var totalHeight = contentHeight + padding.Top + padding.Bottom;

            var hasWordWrapping = m_tmpText.textWrappingMode != TextWrappingModes.NoWrap;

            if (m_resizeHorizontal && !hasWordWrapping)
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
            }

            if (m_resizeVertical)
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
            }
        }

        private void HandleTmpTextChanged(Object changed)
        {
            if (!this || !isActiveAndEnabled || changed == null)
            {
                return;
            }

            if (!TryGetComponent(out TMP_Text localTmpText) || !localTmpText)
            {
                return;
            }

            if (changed == localTmpText || changed == gameObject)
            {
                SetDirty();
                return;
            }

            if (changed is Component component && component.gameObject == gameObject)
            {
                SetDirty();
            }
        }

        private LayoutPadding CollectSiblingPadding()
        {
            var siblingLayouts = GetComponents<UILayout>();
            float top = 0, bottom = 0, left = 0, right = 0;

            foreach (var layout in siblingLayouts)
            {
                if (layout == this) continue;

                var padding = layout.GetLayoutPadding();
                top = Mathf.Max(top, padding.Top);
                bottom = Mathf.Max(bottom, padding.Bottom);
                left = Mathf.Max(left, padding.Left);
                right = Mathf.Max(right, padding.Right);
            }

            return new LayoutPadding(top, bottom, left, right);
        }
    }
}


using UnityEngine;

namespace Aori
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class HorizontalLayoutGroup : UILayout
    {
        private static readonly Vector2 TOP_LEFT_ANCHOR = new(0f, 1f);

        [Header("- Layout")]
        [Header("+ Arrangement")]
        [SerializeField]
        private ChildPivot m_childPivot = ChildPivot.UpperLeft;

        [SerializeField]
        private bool m_reverseArrangement;

        [SerializeField]
        private bool m_forceExpandChildHeight;

        [SerializeField, Min(0f)]
        private float m_spacing;

        [Header("+ Padding")]
        [SerializeField]
        private LayoutPadding m_padding;

        protected override void HandleValidate()
        {
            m_spacing = Mathf.Max(0f, m_spacing);
            m_padding.ClampNonNegative();
        }

        public override LayoutPadding GetLayoutPadding()
        {
            return m_padding;
        }

        protected override void ApplyLayout()
        {

            var parentRect = m_rectTransform.rect;
            var parentPivot = m_rectTransform.pivot;
            var childPivot = m_childPivot.GetPivotVector();

            var topLeft = new Vector2(parentRect.xMin, parentRect.yMax);
            var forcedHeight = Mathf.Max(0f, parentRect.height - m_padding.Top - m_padding.Bottom);

            var childCount = 0;
            var totalChildWidth = 0f;
            var tallestChild = 0f;

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is not RectTransform childRect || !childRect.gameObject.activeSelf)
                {
                    continue;
                }

                childRect.anchorMin = TOP_LEFT_ANCHOR;
                childRect.anchorMax = TOP_LEFT_ANCHOR;
                childRect.pivot = childPivot;

                if (m_forceExpandChildHeight)
                {
                    childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, forcedHeight);
                }

                childCount++;
                totalChildWidth += childRect.rect.width;
                tallestChild = Mathf.Max(tallestChild, childRect.rect.height);
            }

            if (childCount == 0)
            {
                return;
            }

            var totalSpacing = m_spacing * Mathf.Max(0, childCount - 1);
            var totalWidth = m_padding.Left + m_padding.Right + totalChildWidth + totalSpacing;
            var totalHeight = m_padding.Top + m_padding.Bottom + tallestChild;

            var boundsMin = new Vector2(-totalWidth * parentPivot.x, -totalHeight * parentPivot.y);
            var boundsMax = new Vector2(totalWidth * (1f - parentPivot.x), totalHeight * (1f - parentPivot.y));

            var contentLeft = boundsMin.x + m_padding.Left;
            var contentRight = boundsMax.x - m_padding.Right;
            var contentTop = boundsMax.y - m_padding.Top;
            var contentBottom = boundsMin.y + m_padding.Bottom;

            var cursor = m_reverseArrangement ? contentRight : contentLeft;

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is not RectTransform childRect || !childRect.gameObject.activeSelf)
                {
                    continue;
                }

                var childWidth = childRect.rect.width;
                var yPivot = Mathf.Lerp(contentBottom, contentTop, childPivot.y);
                float xPivot;

                if (m_reverseArrangement)
                {
                    xPivot = cursor - childWidth * (1f - childPivot.x);
                    cursor -= childWidth + m_spacing;
                }
                else
                {
                    xPivot = cursor + childWidth * childPivot.x;
                    cursor += childWidth + m_spacing;
                }

                var localPivotPosition = new Vector2(xPivot, yPivot);
                childRect.anchoredPosition = localPivotPosition - topLeft;
            }
        }
    }
}

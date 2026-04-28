using UnityEngine;

namespace Aori
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class VerticalLayoutGroup : UILayout
    {
        private static readonly Vector2 TOP_LEFT_ANCHOR = new(0f, 1f);

        [Header("- Layout")]
        [Header("+ Arrangement")]
        [SerializeField]
        private ChildPivot m_childPivot = ChildPivot.UpperLeft;

        [SerializeField]
        private bool m_reverseArrangement;

        [SerializeField]
        private bool m_forceExpandChildWidth;

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
            var forcedWidth = Mathf.Max(0f, parentRect.width - m_padding.Left - m_padding.Right);

            var childCount = 0;
            var tallestOrWidestChild = 0f;
            var totalChildHeight = 0f;

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is not RectTransform childRect || !childRect.gameObject.activeSelf)
                {
                    continue;
                }

                childRect.anchorMin = TOP_LEFT_ANCHOR;
                childRect.anchorMax = TOP_LEFT_ANCHOR;
                childRect.pivot = childPivot;

                if (m_forceExpandChildWidth)
                {
                    childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, forcedWidth);
                }

                childCount++;
                totalChildHeight += childRect.rect.height;
                tallestOrWidestChild = Mathf.Max(tallestOrWidestChild, childRect.rect.width);
            }

            if (childCount == 0)
            {
                return;
            }

            var totalSpacing = m_spacing * Mathf.Max(0, childCount - 1);
            var totalHeight = m_padding.Top + m_padding.Bottom + totalChildHeight + totalSpacing;
            var totalWidth = m_padding.Left + m_padding.Right + tallestOrWidestChild;

            var boundsMin = new Vector2(-totalWidth * parentPivot.x, -totalHeight * parentPivot.y);
            var boundsMax = new Vector2(totalWidth * (1f - parentPivot.x), totalHeight * (1f - parentPivot.y));

            var contentLeft = boundsMin.x + m_padding.Left;
            var contentRight = boundsMax.x - m_padding.Right;
            var contentTop = boundsMax.y - m_padding.Top;
            var contentBottom = boundsMin.y + m_padding.Bottom;

            var cursor = m_reverseArrangement ? contentBottom : contentTop;

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is not RectTransform childRect || !childRect.gameObject.activeSelf)
                {
                    continue;
                }

                var childHeight = childRect.rect.height;
                var xPivot = Mathf.Lerp(contentLeft, contentRight, childPivot.x);
                float yPivot;

                if (m_reverseArrangement)
                {
                    yPivot = cursor + childHeight * childPivot.y;
                    cursor += childHeight + m_spacing;
                }
                else
                {
                    yPivot = cursor - childHeight * (1f - childPivot.y);
                    cursor -= childHeight + m_spacing;
                }

                var localPivotPosition = new Vector2(xPivot, yPivot);
                childRect.anchoredPosition = localPivotPosition - topLeft;
            }
        }
    }
}
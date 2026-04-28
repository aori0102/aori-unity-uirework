using UnityEngine;

namespace Aori.UIRework
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class ContentSizeFitter : UILayout
    {
        [Header("- Fit Axis")]
        [SerializeField]
        private bool m_resizeVertical = true;

        [SerializeField]
        private bool m_resizeHorizontal = true;

        private readonly Vector3[] m_childCorners = new Vector3[4];

        protected override void ApplyLayout()
        {
            var calculateResult = TryCalculateChildrenBounds(out var min, out var max);
            if (!calculateResult)
            {
                return;
            }

            var padding = CollectSiblingPadding();

            var contentWidth = max.x - min.x;
            var contentHeight = max.y - min.y;

            var totalWidth = contentWidth + padding.Left + padding.Right;
            var totalHeight = contentHeight + padding.Top + padding.Bottom;

            if (m_resizeHorizontal)
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
            }

            if (m_resizeVertical)
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
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

        private bool TryCalculateChildrenBounds(out Vector2 min, out Vector2 max)
        {
            min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            var hasChild = false;

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                UpdateBoundsForTransform(child, ref min, ref max, ref hasChild);
            }

            return hasChild;
        }

        private void UpdateBoundsForTransform(
            Transform target,
            ref Vector2 min,
            ref Vector2 max,
            ref bool hasChild)
        {
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                return;
            }

            if (!target.TryGetComponent(out RectTransform targetRect))
            {
                return;
            }

            hasChild = true;

            UpdateBoundsWithChildRect(targetRect, ref min, ref max);
        }

        private void UpdateBoundsWithChildRect(RectTransform childRect, ref Vector2 min, ref Vector2 max)
        {
            childRect.GetWorldCorners(m_childCorners);

            foreach (var corner in m_childCorners)
            {
                var local = m_rectTransform.InverseTransformPoint(corner);
                min = Vector2.Min(min, local);
                max = Vector2.Max(max, local);
            }
        }

    }
}
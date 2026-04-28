using System.Collections.Generic;
using UnityEngine;

namespace Aori.UIRework
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class GridLayoutGroup : UILayout
    {
        private static readonly Vector2 TOP_LEFT_ANCHOR = new(0f, 1f);

        public enum StartCorner
        {
            UpperLeft,
            UpperRight,
            LowerLeft,
            LowerRight
        }

        public enum StartAxis
        {
            Horizontal,
            Vertical
        }

        public enum Constraint
        {
            Flexible,
            FixedColumnCount,
            FixedRowCount
        }

        [Header("- Layout")]
        [Header("+ Arrangement")]
        [SerializeField]
        private ChildPivot m_childPivot = ChildPivot.UpperLeft;

        [SerializeField]
        private StartAxis m_startAxis = StartAxis.Horizontal;

        [SerializeField]
        private StartCorner m_startCorner = StartCorner.UpperLeft;

        [SerializeField]
        private Constraint m_constraint = Constraint.Flexible;

        [SerializeField, Min(1)]
        private int m_constraintCount = 2;

        [Header("+ Cell")]
        [SerializeField]
        private Vector2 m_cellSize = new(100f, 100f);

        [SerializeField]
        private Vector2 m_spacing;

        [Header("+ Padding")]
        [SerializeField]
        private LayoutPadding m_padding;

        private readonly List<RectTransform> m_activeChildren = new();

        protected override void HandleValidate()
        {
            m_constraintCount = Mathf.Max(1, m_constraintCount);
            m_cellSize = new Vector2(Mathf.Max(0f, m_cellSize.x), Mathf.Max(0f, m_cellSize.y));
            m_spacing = new Vector2(Mathf.Max(0f, m_spacing.x), Mathf.Max(0f, m_spacing.y));
            m_padding.ClampNonNegative();
        }

        public override LayoutPadding GetLayoutPadding()
        {
            return m_padding;
        }

        protected override void ApplyLayout()
        {
            m_activeChildren.Clear();

            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is not RectTransform childRect || !childRect.gameObject.activeSelf)
                {
                    continue;
                }

                childRect.anchorMin = TOP_LEFT_ANCHOR;
                childRect.anchorMax = TOP_LEFT_ANCHOR;
                childRect.pivot = m_childPivot.GetPivotVector();
                childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_cellSize.x);
                childRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_cellSize.y);

                m_activeChildren.Add(childRect);
            }

            var childCount = m_activeChildren.Count;
            if (childCount == 0)
            {
                return;
            }

            ResolveGridDimensions(childCount, out var columns, out var rows);

            var parentRect = m_rectTransform.rect;
            var parentPivot = m_rectTransform.pivot;
            var childPivot = m_childPivot.GetPivotVector();
            var topLeft = new Vector2(parentRect.xMin, parentRect.yMax);

            var totalWidth = m_padding.Left + m_padding.Right + columns * m_cellSize.x + Mathf.Max(0, columns - 1) * m_spacing.x;
            var totalHeight = m_padding.Top + m_padding.Bottom + rows * m_cellSize.y + Mathf.Max(0, rows - 1) * m_spacing.y;

            var boundsMin = new Vector2(-totalWidth * parentPivot.x, -totalHeight * parentPivot.y);
            var boundsMax = new Vector2(totalWidth * (1f - parentPivot.x), totalHeight * (1f - parentPivot.y));

            var contentLeft = boundsMin.x + m_padding.Left;
            var contentTop = boundsMax.y - m_padding.Top;

            for (var i = 0; i < childCount; i++)
            {
                GetGridCoordinates(i, columns, rows, out var column, out var row);

                var xPivot = contentLeft + column * (m_cellSize.x + m_spacing.x) + m_cellSize.x * childPivot.x;
                var yPivot = contentTop - row * (m_cellSize.y + m_spacing.y) - m_cellSize.y * (1f - childPivot.y);

                var localPivotPosition = new Vector2(xPivot, yPivot);
                m_activeChildren[i].anchoredPosition = localPivotPosition - topLeft;
            }
        }

        private void ResolveGridDimensions(int childCount, out int columns, out int rows)
        {
            switch (m_constraint)
            {
                case Constraint.FixedColumnCount:
                    columns = Mathf.Max(1, m_constraintCount);
                    rows = Mathf.CeilToInt(childCount / (float)columns);
                    return;

                case Constraint.FixedRowCount:
                    rows = Mathf.Max(1, m_constraintCount);
                    columns = Mathf.CeilToInt(childCount / (float)rows);
                    return;

                default:
                    var root = Mathf.CeilToInt(Mathf.Sqrt(childCount));
                    if (m_startAxis == StartAxis.Horizontal)
                    {
                        columns = Mathf.Max(1, root);
                        rows = Mathf.CeilToInt(childCount / (float)columns);
                    }
                    else
                    {
                        rows = Mathf.Max(1, root);
                        columns = Mathf.CeilToInt(childCount / (float)rows);
                    }

                    return;
            }
        }

        private void GetGridCoordinates(int index, int columns, int rows, out int column, out int row)
        {
            if (m_startAxis == StartAxis.Horizontal)
            {
                column = index % columns;
                row = index / columns;
            }
            else
            {
                row = index % rows;
                column = index / rows;
            }

            switch (m_startCorner)
            {
                case StartCorner.UpperRight:
                    column = columns - 1 - column;
                    break;

                case StartCorner.LowerLeft:
                    row = rows - 1 - row;
                    break;

                case StartCorner.LowerRight:
                    column = columns - 1 - column;
                    row = rows - 1 - row;
                    break;
            }
        }
    }
}


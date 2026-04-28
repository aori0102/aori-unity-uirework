using UnityEngine;

namespace Aori.UIRework
{
    public static class ChildPivotExtension
    {
        public static Vector2 GetPivotVector(this ChildPivot childPivot)
        {
            return childPivot switch
            {
                ChildPivot.UpperLeft => new Vector2(0f, 1f),
                ChildPivot.UpperCenter => new Vector2(0.5f, 1f),
                ChildPivot.UpperRight => new Vector2(1f, 1f),
                ChildPivot.MiddleLeft => new Vector2(0f, 0.5f),
                ChildPivot.MiddleCenter => new Vector2(0.5f, 0.5f),
                ChildPivot.MiddleRight => new Vector2(1f, 0.5f),
                ChildPivot.LowerLeft => new Vector2(0f, 0f),
                ChildPivot.LowerCenter => new Vector2(0.5f, 0f),
                ChildPivot.LowerRight => new Vector2(1f, 0f),
                _ => new Vector2(0f, 1f)
            };
        }
    }
}
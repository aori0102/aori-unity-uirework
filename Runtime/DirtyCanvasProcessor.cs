using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aori.UIRework
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(10000)]
    public sealed class DirtyCanvasProcessor : MonoBehaviour
    {
        private static DirtyCanvasProcessor s_instance;

        private static readonly HashSet<Canvas> s_dirtyCanvasSet = new();

        public static void EnsureInstance()
        {
            if (s_instance)
            {
                return;
            }

            var go = new GameObject(nameof(DirtyCanvasProcessor))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            s_instance = go.AddComponent<DirtyCanvasProcessor>();
        }

        private void OnDestroy()
        {
            s_instance = null;
        }

        private void LateUpdate()
        {
            ProcessDirtyCanvases();
        }

        private static void ProcessDirtyCanvases()
        {
            var dirtySet = new HashSet<Canvas>(s_dirtyCanvasSet);
            s_dirtyCanvasSet.Clear();
            foreach (var canvas in dirtySet)
            {
                try
                {
                    _ = canvas.gameObject;
                }
                catch (MissingReferenceException)
                {
                    Debug.LogWarning($"Canvas {canvas} is missing!");
                    continue;
                }

                var uiLayouts = canvas.GetComponentsInChildren<UILayout>();
                Array.Sort(
                    uiLayouts,
                    (a, b)
                        => GetDepth(a.transform).CompareTo(GetDepth(b.transform))
                );
                foreach (var uiLayout in uiLayouts)
                {
                    uiLayout.ExecuteDirtyApply();
                }
            }
        }

        private static int GetDepth(Transform t)
        {
            int depth = 0;

            while (t.parent)
            {
                depth++;
                t = t.parent;
            }

            return depth;
        }

        public static void RegisterDirtyCanvas(Canvas dirtyCanvas)
        {
            if (!dirtyCanvas)
            {
                return;
            }

            s_dirtyCanvasSet.Add(dirtyCanvas);
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Aori.UI.Editor
{
    [CustomEditor(typeof(GridLayoutGroup))]
    public sealed class GridLayoutGroupEditor : UnityEditor.Editor
    {
        private SerializedProperty m_childPivot;
        private SerializedProperty m_startAxis;
        private SerializedProperty m_startCorner;
        private SerializedProperty m_constraint;
        private SerializedProperty m_constraintCount;
        private SerializedProperty m_cellSize;
        private SerializedProperty m_spacing;
        private SerializedProperty m_padding;

        private void OnEnable()
        {
            m_childPivot = serializedObject.FindProperty("m_childPivot");
            m_startAxis = serializedObject.FindProperty("m_startAxis");
            m_startCorner = serializedObject.FindProperty("m_startCorner");
            m_constraint = serializedObject.FindProperty("m_constraint");
            m_constraintCount = serializedObject.FindProperty("m_constraintCount");
            m_cellSize = serializedObject.FindProperty("m_cellSize");
            m_spacing = serializedObject.FindProperty("m_spacing");
            m_padding = serializedObject.FindProperty("m_padding");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_childPivot);
            EditorGUILayout.PropertyField(m_startAxis);
            EditorGUILayout.PropertyField(m_startCorner);
            EditorGUILayout.PropertyField(m_constraint);

            var constraint = (GridLayoutGroup.Constraint)m_constraint.enumValueIndex;
            if (constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                EditorGUILayout.PropertyField(m_constraintCount, new GUIContent("Column Count"));
            }
            else if (constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                EditorGUILayout.PropertyField(m_constraintCount, new GUIContent("Row Count"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Cell", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_cellSize);
            EditorGUILayout.PropertyField(m_spacing);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Padding", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_padding);

            serializedObject.ApplyModifiedProperties();
        }
    }
}


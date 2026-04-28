using UnityEditor;
using UnityEngine;

namespace Aori.UI.Editor
{
    [CustomPropertyDrawer(typeof(LayoutPadding))]
    public sealed class LayoutPaddingDrawer : PropertyDrawer
    {
        private const float SPACING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3f + SPACING * 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var topProp = property.FindPropertyRelative("m_top");
            var bottomProp = property.FindPropertyRelative("m_bottom");
            var leftProp = property.FindPropertyRelative("m_left");
            var rightProp = property.FindPropertyRelative("m_right");

            EditorGUI.BeginProperty(position, label, property);

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var labelRect = new Rect(position.x, position.y, position.width, lineHeight);
            var firstRowY = labelRect.yMax + SPACING;
            var secondRowY = firstRowY + lineHeight + SPACING;
            var fieldWidth = (position.width - SPACING) * 0.5f;

            var topRect = new Rect(position.x, firstRowY, fieldWidth, lineHeight);
            var bottomRect = new Rect(position.x + fieldWidth + SPACING, firstRowY, fieldWidth, lineHeight);
            var leftRect = new Rect(position.x, secondRowY, fieldWidth, lineHeight);
            var rightRect = new Rect(position.x + fieldWidth + SPACING, secondRowY, fieldWidth, lineHeight);

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(topRect, topProp, new GUIContent("Top"));
            EditorGUI.PropertyField(bottomRect, bottomProp, new GUIContent("Bottom"));
            EditorGUI.PropertyField(leftRect, leftProp, new GUIContent("Left"));
            EditorGUI.PropertyField(rightRect, rightProp, new GUIContent("Right"));

            EditorGUI.EndProperty();
        }
    }
}


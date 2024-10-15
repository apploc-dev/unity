#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace AppLoc.Editor {
    [CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
    public class LocalizationKeyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect fieldPosition = new Rect(position.x, position.y, position.width, lineHeight);
            EditorGUI.PropertyField(fieldPosition, property, label);

            Rect helpBoxPosition = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight * 2);

            if (property.propertyType != SerializedPropertyType.String) {
                EditorGUI.HelpBox(helpBoxPosition, "Use [LocalizationKey] with string fields only.", MessageType.Error);
                return;
            }

            if (string.IsNullOrEmpty(property.stringValue)) {
                EditorGUI.HelpBox(helpBoxPosition, "Key is empty", MessageType.Warning);
            }
            else {
                string value = Utils.TryGetValue(property.stringValue);

                if (value == null) {
                    EditorGUI.HelpBox(helpBoxPosition, $"Key '{property.stringValue}' not found", MessageType.Warning);
                }
                else {
                    EditorGUI.HelpBox(helpBoxPosition, value, MessageType.Info);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing;
    }
}

#endif
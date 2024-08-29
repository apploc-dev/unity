#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AppLoc.Editor {
    [CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
    public class LocalizationKeyDrawer : PropertyDrawer {
        private Localization _localization;
        private bool _isInitialized;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (!_isInitialized) {
                _localization = Resources.Load<LocalizationsObject>(LocalizationManager.LocalizationsObjectName).localizations[0];
                _isInitialized = true;
            }

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
            else if (_localization.keys.All(e => e.key != property.stringValue)) {
                EditorGUI.HelpBox(helpBoxPosition, $"Key '{property.stringValue}' not found", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float helpBoxHeight = lineHeight * 2;

            bool shouldShowHelpBox = property.propertyType != SerializedPropertyType.String ||
                                     string.IsNullOrEmpty(property.stringValue) ||
                                     (_localization != null && _localization.keys.All(e => e.key != property.stringValue));

            return lineHeight + (shouldShowHelpBox ? helpBoxHeight + spacing : 0);
        }
    }
}

#endif
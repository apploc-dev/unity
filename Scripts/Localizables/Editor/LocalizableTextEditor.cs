using System.Linq;
using UnityEditor;

namespace AppLoc.Localizables.Editor {
    public class LocalizableTextEditor : UnityEditor.Editor {
        private SerializedProperty _keyProperty;
        private Localization _localization;

        private void OnEnable() {
            _keyProperty = serializedObject.FindProperty("key");
            _localization = AssetDatabase.LoadAssetAtPath<LocalizationsObject>(LocalizationManager.LocalizationsObjectPath).localizations[0];
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_keyProperty);

            serializedObject.ApplyModifiedProperties();

            string key = _keyProperty.stringValue;

            if (_localization.keys.All(e => e.key != key)) {
                EditorGUILayout.HelpBox($"Key '{key}' not found", MessageType.Warning);
            }
        }
    }
}
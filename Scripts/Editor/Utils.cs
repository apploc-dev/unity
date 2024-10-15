using UnityEngine;

namespace AppLoc.Editor {
    public static class Utils {
        public static string TryGetValue(string key) {
            if (Application.isPlaying) {
                return LocalizationManager.GetValue(key);
            }

            LocalizationsObject editorLocalization = Resources.Load<LocalizationsObject>(LocalizationManager.LocalizationsObjectName);

            if (editorLocalization == null || editorLocalization.localizations.Length <= 0) {
                return null;
            }

            return editorLocalization.localizations[0].GetValue(key);
        }
    }
}
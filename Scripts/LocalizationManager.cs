using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AppLoc {
    public static class LocalizationManager {
        public delegate void LocalizeEventHandler();

        public static LocalizeEventHandler OnLocalize;

        public const string LocalizationsObjectPath = "Assets/ATLocalizationsObject.asset";

        private static readonly LocalizationsObject LocalizationsObject;

        private static Localization _currentLocalization;

        private static void Log(string text) {
            Debug.Log("[" + nameof(AppLoc) + "] " + text);
        }

        public static string[] GetLocalizationCodes() => LocalizationsObject.localizations.Select(e => e.code).ToArray();

        public static void SetLocalization(string code) {
            _currentLocalization = LocalizationsObject.localizations.First(e => e.code == code);
            OnLocalize?.Invoke();
        }

        public static string GetValue(string key) => _currentLocalization.GetValue(key);


        static LocalizationManager() {
            LocalizationsObject = AssetDatabase.LoadAssetAtPath<LocalizationsObject>(LocalizationsObjectPath);

            string[] codes = GetLocalizationCodes();
            string code = codes.Contains("EN") ? "EN" : codes[0];

            Log("loaded localizations from '" + LocalizationsObjectPath + "', default localization: '" + code + "'");

            SetLocalization(code);
        }
    }
}
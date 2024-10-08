using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace AppLoc {
    public static class LocalizationManager {
        public delegate void LocalizeEventHandler();

        public static LocalizeEventHandler OnLocalize;

        internal const string LocalizationsObjectName = "ATLocalizationsObject";
        internal const string LocalizationsObjectPath = "Assets/Resources/" + LocalizationsObjectName + ".asset";

        private static readonly LocalizationsObject LocalizationsObject;

        private static Localization _currentLocalization;

        private static void Log(string text) {
            Debug.Log("[" + nameof(AppLoc) + "] " + text);
        }

        public static ReadOnlyCollection<string> LocalizationCodes { get; private set; }

        public static void SetLocalization(string code) {
            _currentLocalization = LocalizationsObject.localizations.First(e => e.code == code);
            OnLocalize?.Invoke();
        }

        public static string GetValue(string key) => _currentLocalization.GetValue(key);


        static LocalizationManager() {
            LocalizationsObject = Resources.Load<LocalizationsObject>(LocalizationsObjectName);

            LocalizationCodes = new ReadOnlyCollection<string>(
                LocalizationsObject.localizations.Select(e => e.code).ToArray()
            );
            string code = LocalizationCodes.Contains("EN") ? "EN" : LocalizationCodes[0];

            Log("loaded localizations from '" + LocalizationsObjectPath + "', default localization: '" + code + "'");

            SetLocalization(code);
        }
    }
}
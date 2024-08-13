using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

namespace AppLoc.Editor {
    public class AppLocWindow : EditorWindow {
        private const string Url = "https://us-central1-app-translation-acac8.cloudfunctions.net/getProjectData?id={0}&secret={1}";

        private static AppLocWindow _window;

        [MenuItem("Window/" + nameof(AppLoc))]
        public static void ShowWindow() {
            _window = GetWindow<AppLocWindow>(true, nameof(AppLoc));
        }

        private static readonly string PreferencesLocation = Path.Combine(Application.dataPath, "../ProjectSettings/app-translation-prefs.json");
        private static Preferences _preferences;

        private MessageType _statusType;
        private string _status;

        private void OnEnable() {
            if (_preferences == null) {
                _preferences = File.Exists(PreferencesLocation)
                    ? JsonUtility.FromJson<Preferences>(File.ReadAllText(PreferencesLocation))
                    : new Preferences();
            }

            _statusType = MessageType.None;
        }

        private static void SavePrefs() {
            File.WriteAllText(PreferencesLocation, JsonUtility.ToJson(_preferences));
        }

        private void OnGUI() {
            string currentProjectId = EditorGUILayout.TextField("Project ID", _preferences.id);
            string currentProjectSecret = EditorGUILayout.PasswordField("Project Secret", _preferences.secret);

            if (currentProjectId != _preferences.id || currentProjectSecret != _preferences.secret) {
                _preferences.id = currentProjectId;
                _preferences.secret = currentProjectSecret;

                SavePrefs();
            }

            GUILayout.FlexibleSpace();

            if (_statusType != MessageType.None) {
                EditorGUILayout.HelpBox(_status, _statusType);
            }

            if (GUILayout.Button("Update")) {
                _window.StartCoroutine(MakeRequest());
            }
        }

        private IEnumerator MakeRequest() {
            DateTime startDateTime = DateTime.Now;

            UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(Url, _preferences.id, _preferences.secret));
            webRequest.SendWebRequest();

            while (webRequest.result == UnityWebRequest.Result.InProgress) {
                int seconds = ((int)(DateTime.Now.Subtract(startDateTime).TotalSeconds * 2) + 2) % 3;

                SetStatus("Fetching updates" + new string('.', seconds + 1), MessageType.Info);

                yield return null;
            }

            if (webRequest.result == UnityWebRequest.Result.Success) {
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(webRequest.downloadHandler.text);

                if (!response.ok) {
                    SetStatus("Api error: " + response.message, MessageType.Error);
                }
                else {
                    string data = response.data;

                    StringBuilder hashBuilder = new();

                    using (SHA256 sha256 = SHA256.Create()) {
                        byte[] result = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

                        foreach (byte b in result) {
                            hashBuilder.Append(b.ToString("x2"));
                        }
                    }

                    string hash = hashBuilder.ToString();

                    LocalizationsObject currentLocalizations = AssetDatabase.LoadAssetAtPath<LocalizationsObject>(LocalizationManager.LocalizationsObjectPath);
                    
                    if (currentLocalizations != null && currentLocalizations.hash == hash) {
                        SetStatus("Nothing has changed since the last version.", MessageType.Info);
                    }
                    else {
                        try {
                            SetStatus("Creating " + nameof(LocalizationsObject) + "...", MessageType.Info);

                            Localization apiLocalization = JsonUtility.FromJson<Localization>(data);

                            Dictionary<string, List<AppLoc.Localization.KeyValuePair>> localizations = new();

                            foreach (Localization.LocalizationKey key in apiLocalization.keys) {
                                foreach (Localization.LocalizationKey.LocalizationValue localization in key.localizations) {
                                    string code = localization.code;

                                    if (!localizations.ContainsKey(code)) {
                                        localizations.Add(code, new List<AppLoc.Localization.KeyValuePair>());
                                    }

                                    localizations[code]
                                        .Add(new AppLoc.Localization.KeyValuePair { key = key.key, value = localization.value });
                                }
                            }

                            LocalizationsObject newLocalization = CreateInstance<LocalizationsObject>();

                            newLocalization.hash = hash;
                            
                            newLocalization.localizations = new AppLoc.Localization[localizations.Keys.Count];

                            for (int i = 0; i < localizations.Keys.Count; i++) {
                                string code = localizations.Keys.ElementAt(i);

                                newLocalization.localizations[i] = new AppLoc.Localization {
                                    code = code, keys = localizations[code].ToArray()
                                };
                            }

                            AssetDatabase.CreateAsset(newLocalization, LocalizationManager.LocalizationsObjectPath);
                            AssetDatabase.SaveAssets();

                            SetStatus(string.Format(CultureInfo.InvariantCulture,
                                "Done in {0:F1}s", DateTime.Now.Subtract(startDateTime).TotalSeconds
                            ), MessageType.Info);
                        }
                        catch (Exception exception) {
                            SetStatus("Error: " + exception.Message, MessageType.Error);
                        }
                    }
                }
            }
            else {
                SetStatus("Web request error: " + webRequest.error, MessageType.Error);
            }
        }

        private void SetStatus(string status, MessageType statusType) {
            _status = status;
            _statusType = statusType;
            Repaint();
        }

        [Serializable]
        private class Preferences {
            public string id;
            public string secret;
        }

        [Serializable]
        private class ApiResponse {
            public bool ok;
            public string message;
            public string data;
        }

        [Serializable]
        private class Localization {
            public LocalizationKey[] keys;

            [Serializable]
            public class LocalizationKey {
                public string key;
                public LocalizationValue[] localizations;
                public string uid;

                [Serializable]
                public class LocalizationValue {
                    public string code;
                    public string value;
                    public bool locked;
                }
            }
        }
    }
}
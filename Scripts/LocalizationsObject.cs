using System;
using UnityEngine;

namespace AppLoc {
    public class LocalizationsObject : ScriptableObject {
        public string hash;
        
        public Localization[] localizations;
    }

    [Serializable]
    public class Localization {
        public string code;
        public KeyValuePair[] keys;

        public string GetValue(string key) {
            foreach (KeyValuePair kvp in keys) {
                if (key == kvp.key) {
                    return kvp.value;
                }
            }

            return null;
        }

        [Serializable]
        public class KeyValuePair {
            public string key;
            public string value;
        }
    }
}
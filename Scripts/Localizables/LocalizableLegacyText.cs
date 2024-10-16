using AppLoc.Editor;
using UnityEngine;
using UnityEngine.UI;

namespace AppLoc.Localizables {
    [RequireComponent(typeof(Text))]
    public class LocalizableLegacyText : AbstractLocalizable {
        [SerializeField] [LocalizationKey] private string key;

        private bool _initialized;
        private Text _text;

        protected override void OnEnable() {
            if (!_initialized) {
                _text = GetComponent<Text>();

                _initialized = true;
            }

            base.OnEnable();
        }

        protected override void Localize() {
            _text.text = LocalizationManager.GetValue(key);
        }

#if UNITY_EDITOR
        private void OnValidate() {
            string value = Utils.TryGetValue(key);

            if (value != null) {
                GetComponent<Text>().text = value;
            }
        }
#endif
    }
}
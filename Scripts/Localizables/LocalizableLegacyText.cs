using UnityEngine;
using UnityEngine.UI;

namespace AppLoc.Localizables {
    [RequireComponent(typeof(Text))]
    public class LocalizableLegacyText : AbstractLocalizable {
        [SerializeField] private string key;

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
    }
}
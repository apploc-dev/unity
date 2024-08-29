using UnityEngine;
using TMPro;

namespace AppLoc.Localizables {
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizableTextMeshPro : AbstractLocalizable {
        [SerializeField] [LocalizationKey] private string key;

        private bool _initialized;
        private TMP_Text _text;

        protected override void OnEnable() {
            if (!_initialized) {
                _text = GetComponent<TMP_Text>();

                _initialized = true;
            }

            base.OnEnable();
        }

        protected override void Localize() {
            _text.text = LocalizationManager.GetValue(key);
        }
    }
}
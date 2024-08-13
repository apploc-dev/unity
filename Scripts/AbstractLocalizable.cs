using UnityEngine;

namespace AppLoc {
    public abstract class AbstractLocalizable : MonoBehaviour {
        private bool _initialized;

        protected virtual void OnEnable() {
            if (_initialized) {
                return;
            }

            _initialized = true;

            LocalizationManager.OnLocalize += Localize;
            Localize();
        }

        protected abstract void Localize();

        private void OnDestroy() {
            LocalizationManager.OnLocalize -= Localize;
        }
    }
}
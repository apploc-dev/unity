using UnityEngine;

namespace AppLoc {
    public abstract class AbstractLocalizable : MonoBehaviour {
        private bool _abstractLocalizableInitialized;

        protected virtual void OnEnable() {
            if (_abstractLocalizableInitialized) {
                return;
            }

            _abstractLocalizableInitialized = true;

            LocalizationManager.OnLocalize += Localize;
            Localize();
        }

        protected abstract void Localize();

        private void OnDestroy() {
            LocalizationManager.OnLocalize -= Localize;
        }
    }
}
#if UNITY_EDITOR

using UnityEditor;

namespace AppLoc.Localizables.Editor {
    [CustomEditor(typeof(LocalizableLegacyText))]
    public class LocalizableLegacyTextEditor : LocalizableTextEditor { }
}

#endif
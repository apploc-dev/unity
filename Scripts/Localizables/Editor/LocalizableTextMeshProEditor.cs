#if UNITY_EDITOR

using UnityEditor;

namespace AppLoc.Localizables.Editor {
    [CustomEditor(typeof(LocalizableTextMeshPro))]
    public class LocalizableTextMeshProEditor : LocalizableTextEditor { }
}

#endif
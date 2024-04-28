using System;
using UnityEditor;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SharkBinder))]
    public class SharkBinderEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            foreach (var genericTarget in targets)
            {
                var target = genericTarget as SharkBinder;
                if (!target) continue;
                target.OnValidate();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
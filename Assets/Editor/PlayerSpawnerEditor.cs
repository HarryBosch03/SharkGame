using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerSpawner))]
    public class PlayerSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var target = this.target as PlayerSpawner;
            
            base.OnInspectorGUI();
            if (GUILayout.Button("Spawn Dummy Player"))
            {
                target.SpawnDummyPlayer();
            }
        }
    }
}
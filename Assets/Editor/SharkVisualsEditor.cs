using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [InitializeOnLoad]
    public static class SharkVisualsEditor
    {
        static SharkVisualsEditor() { SceneView.duringSceneGui += Draw; }

        private static void Draw(SceneView sceneView)
        {
            if (Application.isPlaying) return;
            
            foreach (var target in Object.FindObjectsOfType<SharkVisuals>())
            {
                if (!target.segmentInstance) return;

                var sphere = target.segmentInstance.GetComponent<MeshFilter>().sharedMesh;
                var mat = target.segmentInstance.GetComponent<MeshRenderer>().sharedMaterial;


                for (var i = 0; i < target.segmentCount; i++)
                {
                    var p = i / (target.segmentCount - 1f);
                    var position = target.transform.position - target.transform.right * p * target.length;
                    var rotation = target.transform.rotation;
                    var scale = Vector3.one * target.Profile(p) * 2f;
                    var matrix = Matrix4x4.TRS(position, rotation, scale);
                    Graphics.DrawMesh(sphere, matrix, mat, 0);
                }
            }
        }
    }
}
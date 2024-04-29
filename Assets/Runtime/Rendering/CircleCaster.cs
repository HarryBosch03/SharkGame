using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Rendering
{
    [RequireComponent(typeof(ShadowCaster2D))]
    public class CircleCaster : MonoBehaviour
    {
        public float radius = 0.5f;
        public Vector2 offset;
        public int resolution = 32;
        
        private void OnValidate()
        {
            var caster = gameObject.GetComponent<ShadowCaster2D>();
            if (!caster) caster = gameObject.AddComponent<ShadowCaster2D>();
            
            resolution = Mathf.Max(3, resolution);
            radius = Mathf.Max(0f, radius);
            
            var field = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.Instance | BindingFlags.NonPublic);

            var shapePath = new Vector3[resolution];
            for (var i = 0; i < resolution; i++)
            {
                var a = (i / (float)resolution) * Mathf.PI * 2f;
                shapePath[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius + offset;
            }
            field.SetValue(caster, shapePath);
        }
    }
}
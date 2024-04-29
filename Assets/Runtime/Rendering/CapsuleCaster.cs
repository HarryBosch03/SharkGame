using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Rendering
{
    [RequireComponent(typeof(ShadowCaster2D))]
    public class CapsuleCaster : MonoBehaviour
    {
        public float radius = 0.5f;
        public float length = 1f;
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
            var halfRes = resolution / 2;
            cap(0, Vector2.up * (length - radius), 1);
            cap(halfRes, Vector2.down * (length - radius), -1);
            
            field.SetValue(caster, shapePath);

            void cap(int start, Vector2 offset, int sign)
            {
                for (var i = 0; i < halfRes; i++)
                {
                    var a = i / (halfRes - 1f) * Mathf.PI;
                    shapePath[start + i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * sign * radius + offset + this.offset;
                }    
            }
        }
    }
}
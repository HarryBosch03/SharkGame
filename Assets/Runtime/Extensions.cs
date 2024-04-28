using UnityEngine;

namespace Runtime
{
    public static class Extensions
    {
        public static float ToAngle(this Vector2 v) => Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        public static Vector2 FromAngle(this float a) => new Vector2(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad));
    }
}
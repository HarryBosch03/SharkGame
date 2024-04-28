using UnityEngine;

namespace Runtime
{
    public static class Utility
    {
        public static Color HueShift(Color color, float h)
        {
            Color.RGBToHSV(color, out _, out var s, out var v);
            return Color.HSVToRGB(h, s, v);
        }
    }
}
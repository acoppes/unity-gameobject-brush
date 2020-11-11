using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public static class PixelPerfectUtils
    {
        public static void RoundToPixelPerfect(Transform t, int dpi)
        {
            var l = t.localPosition;
            l.x = Mathf.RoundToInt(l.x * dpi) / (float) dpi;
            l.y = Mathf.RoundToInt(l.y * dpi) / (float) dpi;
            t.localPosition = l;
        }
    }
}
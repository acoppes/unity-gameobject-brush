using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Pixel Perfect Brush")]
    public class PixelPerfectDefaultBrushAsset : ScriptableDefaultBrushAsset
    {
        public int dpi;

        public override void UpdatePosition(Vector2 p)
        {
            base.UpdatePosition(p);
            // now fix to pixel perfect positions

            if (dpi <= 0)
                return;

            foreach (var previewInstance in previewInstances)
            {
                var l = previewInstance.transform.localPosition;
                l.x = Mathf.RoundToInt(l.x * dpi) / (float) dpi;
                l.y = Mathf.RoundToInt(l.y * dpi) / (float) dpi;
                previewInstance.transform.localPosition = l;
            }
        }
    }
}
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Pixel Perfect Brush")]
    public class PixelPerfectBrushAsset : ScriptableBrushBaseAsset
    {
        public int dpi;

        public override void UpdatePosition(Vector2 p)
        {
            base.UpdatePosition(p);
            // now fix to pixel perfect positions
        }
    }
}
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Modifiers/Pixel Perfect")]
    public class PixelPerfectModifier : BrushModifierAsset
    {
        public int dpi = 100;

        public override void UpdatePosition(ScriptableBrushBaseAsset brush)
        {
            base.UpdatePosition(brush);
            if (brush.previewParent != null)
                PixelPerfectUtils.RoundToPixelPerfect(brush.previewParent.transform, dpi);
        }

        public override void ApplyModifier(ScriptableBrushBaseAsset brush)
        {
            var previewInstances = brush.previewInstances;
            foreach (var previewInstance in previewInstances)
            {
                PixelPerfectUtils.RoundToPixelPerfect(previewInstance.transform, dpi);
            }
        }
    }
}
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Modifiers/Distribute")]
    public class BrushDistributeModifier : BrushModifierAsset
    {
        public float minDistributionOffset = 0.0f;
        public float maxDistributionOffset = 1.0f;
        
        public override void ApplyModifier(ScriptableBrushBaseAsset brush)
        {
            var previewParent = brush.previewParent;

            if (previewParent.childCount <= 1)
                return;
            
            for (var i = 0; i < previewParent.childCount; i++)
            {
                var t = previewParent.GetChild(i);
                var len = UnityEngine.Random.Range(minDistributionOffset, maxDistributionOffset);
                var angle = UnityEngine.Random.Range(0, 360);
                var offset = Quaternion.Euler(0, 0, angle) * new Vector3(len, 0, 0);
                t.localPosition = offset;
            }
        }
    }
}
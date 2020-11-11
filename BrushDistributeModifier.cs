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
            var previewInstances = brush.previewInstances;
            
            if (previewInstances.Count > 1)
            {
                var offset = Vector2.zero; 

                foreach (var previewInstance in previewInstances)
                {
                    var len = UnityEngine.Random.Range(minDistributionOffset, maxDistributionOffset);
                    var angle = UnityEngine.Random.Range(0, 360);
                    offset = Quaternion.Euler(0, 0, angle) * new Vector3(len, 0, 0);
                    previewInstance.transform.localPosition = offset;
                }

            }
        }
    }
}
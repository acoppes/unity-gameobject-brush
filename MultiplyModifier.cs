using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Modifiers/Multiply")]
    public class MultiplyModifier : BrushModifierAsset
    {
        public int min, max;
        
        public override void ApplyModifier(ScriptableBrushBaseAsset brush)
        {
            if (min <= 0 || max <= 1)
                return;
            
            var previewInstances = brush.previewInstances;

            foreach (var previewInstance in previewInstances)
            {
                var count = UnityEngine.Random.Range(min, max);
                for (var i = 0; i < count; i++)
                {
                    #if UNITY_EDITOR
                    var prefabRoot = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(previewInstance);
         
                    if (prefabRoot != null)
                        UnityEditor.PrefabUtility.InstantiatePrefab (prefabRoot, previewInstance.transform.parent);
                    #else
                        Instantiate (previewInstance, previewInstance.transform.parent);
                    #endif
                    
                    // else
                    //     Instantiate (Selection.activeGameObject);
                    
                    // PrefabUtility.InstantiatePrefab(prefab);
                    // GameObject.Instantiate(previewInstance);
                }
            }
            
        }
    }
}
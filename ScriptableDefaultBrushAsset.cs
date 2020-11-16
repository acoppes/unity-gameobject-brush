using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Default Brush")]
    public class ScriptableDefaultBrushAsset : ScriptableBrushBaseAsset
    {
        public override void CreatePreview(IEnumerable<PaletteObject> paletteObjects)
        {
            DestroyPreview();
            CreateParent();
            
            foreach (var paletteObject in paletteObjects)
            {
#if UNITY_EDITOR
                var preview = UnityEditor.PrefabUtility.InstantiatePrefab(paletteObject.prefab, previewParent) as GameObject;
                preview.transform.localPosition = Vector2.zero;
#endif
            }

            foreach (var modifier in modifiers)
            {
                modifier.ApplyModifier(this);
            }
        }

        public override void Paint()
        {
            foreach (var previewInstance in previewInstances)
            {
                
#if UNITY_EDITOR
                var prefabRoot = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(previewInstance);
         
                if (prefabRoot != null)
                {
                    var paintedObject = UnityEditor.PrefabUtility.InstantiatePrefab(prefabRoot, previewParent.parent) 
                        as GameObject;
                    paintedObject.transform.position = previewInstance.transform.position;
                    UnityEditor.Undo.RegisterCreatedObjectUndo (paintedObject, "Painted");
                }
#else
                Instantiate (previewInstance, previewParent.parent);
                paintedObject.transform.position = previewInstance.transform.position;
#endif
            }
        }
    }
}
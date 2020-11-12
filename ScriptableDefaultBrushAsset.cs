using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Default Brush")]
    public class ScriptableDefaultBrushAsset : ScriptableBrushBaseAsset
    {
        public override void CreatePreview(List<GameObject> prefabs)
        {
            DestroyPreview();
            CreateParent();
            
            foreach (var prefab in prefabs)
            {
#if UNITY_EDITOR
                var preview = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, previewParent) as GameObject;
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
            var transforms = new List<Transform>();
            for (var i = 0; i < previewParent.childCount; i++)
            {
                transforms.Add(previewParent.GetChild(i));
            }
            
            foreach (var t in transforms)
            {
                t.parent = previewParent.parent;
                #if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo (t.gameObject, "Painted");
                #endif
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Palette/GameObject Palette")]
    public class ObjectPaletteAsset : ObjectPaletteBaseAsset
    {
        public List<GameObject> prefabs;
        
        public override List<PaletteObject> CreatePaletteObjects()
        {
            #if UNITY_EDITOR
            return prefabs.Select(p => new PaletteObject
            {
                name = p.name,
                sourceObject = p,
                preview = UnityEditor.AssetPreview.GetAssetPreview(p)
            }).ToList();
            #else
            return null;
            #endif
        }
    }
}
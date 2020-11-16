using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Palette/Sprites Palette")]
    public class ObjectPaletteSpritesAsset : ObjectPaletteBaseAsset
    {
        public List<Sprite> sprites;
        
        public override List<PaletteObject> CreatePaletteObjects()
        {
#if UNITY_EDITOR
            return sprites.Select(s => new PaletteObject
            {
                name = s.name,
                sourceObject = s,
                preview = UnityEditor.AssetPreview.GetAssetPreview(s)
            }).ToList();
#else
            return null;
#endif
        }
    }
}
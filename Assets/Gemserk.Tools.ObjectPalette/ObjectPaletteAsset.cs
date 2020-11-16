using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Palette/GameObject Palette")]
    public class ObjectPaletteAsset : ObjectPaletteBaseAsset
    {
        public List<GameObject> prefabs;
        
        public override List<GameObject> GetObjects()
        {
            return prefabs;
        }
    }
}
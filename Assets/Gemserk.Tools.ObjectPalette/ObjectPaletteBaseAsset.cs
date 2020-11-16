using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public abstract class ObjectPaletteBaseAsset : ScriptableObject
    {
        public abstract List<GameObject> GetObjects();
    }
}
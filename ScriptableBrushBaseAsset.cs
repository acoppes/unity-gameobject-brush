using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public abstract class ScriptableBrushBaseAsset : ScriptableObject, IBrush
    {
        public abstract void UpdatePosition(Vector2 position);
        public abstract void CreatePreview(List<GameObject> prefabs);
        public abstract void DestroyPreview();
        public abstract void Paint();
    }
}
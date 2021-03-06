using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public interface IBrush
    {
        void UpdatePosition(Vector2 position);
        
        void CreatePreview(IEnumerable<PaletteObject> paletteObjects);

        void DestroyPreview();

        void Paint();

        bool RegenerateOnPaint
        {
            get;
        }
    }
}
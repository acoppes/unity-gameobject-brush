using System.Collections.Generic;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    public class PaletteSelection
    {
        public readonly List<PaletteObject> selection = new List<PaletteObject>();

        public bool IsEmpty => selection.Count == 0;

        public void Clear()
        {
            selection.Clear();
        }

        public bool Contains(PaletteObject p)
        {
            return selection.Contains(p);
        }

        public void Add(PaletteObject p)
        {
            selection.Add(p);
        }
    }
}
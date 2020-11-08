using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    public class PaletteSelection
    {
        public List<PaletteObject> selection = new List<PaletteObject>();

        public bool IsEmpty => selection.Count == 0;

        public List<GameObject> SelectedPrefabs
        {
            get
            {
                return selection.Select(s => s.prefab).ToList();
            }
        }

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
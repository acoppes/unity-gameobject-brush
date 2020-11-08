using System;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    public class PaletteObject : IEquatable<PaletteObject>
    {
        public string name;
        public GameObject prefab;
        public Texture preview;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PaletteObject) obj);
        }

        public bool Equals(PaletteObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return name == other.name && Equals(prefab, other.prefab) && Equals(preview, other.preview);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (prefab != null ? prefab.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (preview != null ? preview.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
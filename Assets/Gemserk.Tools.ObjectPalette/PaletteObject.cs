using System;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public class PaletteObject : IEquatable<PaletteObject>
    {
        public string name;
        public UnityEngine.Object sourceObject;
        public Texture preview;

        public GameObject Instantiate()
        {
            if (sourceObject is GameObject)
            {
                #if UNITY_EDITOR
                return UnityEditor.PrefabUtility.InstantiatePrefab(sourceObject) as GameObject;
                #else
                return null;
                #endif
            } 
            
            if (sourceObject is Sprite sprite)
            {
                var go = new GameObject(sprite.name);
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                return go;
            }
            
            return null;
        }

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
            return name == other.name && Equals(sourceObject, other.sourceObject) && Equals(preview, other.preview);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (name != null ? name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (sourceObject != null ? sourceObject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (preview != null ? preview.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
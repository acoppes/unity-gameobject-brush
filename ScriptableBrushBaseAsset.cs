using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    public abstract class ScriptableBrushBaseAsset : ScriptableObject, IBrush
    {
        [NonSerialized]
        public Vector2 position;
        
        [NonSerialized]
        public readonly List<GameObject> previewInstances = new List<GameObject>();

        [NonSerialized]
        public Transform previewParent;
        
        [SerializeField]
        protected List<BrushModifierAsset> modifiers = new List<BrushModifierAsset>();
        
        public virtual void UpdatePosition(Vector2 p)
        {
            position = p;
            if (previewParent != null)
                previewParent.position = p;
            
            foreach (var modifier in modifiers)
            {
                modifier.UpdatePosition(this);
            }
        }
        
        public abstract void CreatePreview(List<GameObject> prefabs);

        protected void CreateParent()
        {
            if (previewParent != null) 
                return;
            var brushPreviewObject = new GameObject("~BrushPreview")
            {
                hideFlags = HideFlags.NotEditable, 
                tag = "EditorOnly"
            };

            brushPreviewObject.AddComponent<BrushPreview>();
                
            previewParent = brushPreviewObject.transform;
            previewParent.position = position;
        }
        
        public virtual void DestroyPreview()
        {
            if (previewParent != null)
                DestroyImmediate(previewParent.gameObject);
            previewParent = null;
            previewInstances.Clear();
        }
        
        public abstract void Paint();
    }
}
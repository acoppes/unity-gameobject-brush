using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Default Brush")]
    public class ScriptableBrushBaseAsset : ScriptableObject, IBrush
    {
        [NonSerialized]
        protected Vector2 position;
        
        [NonSerialized]
        protected readonly List<GameObject> previewInstances = new List<GameObject>();

        public void UpdatePosition(Vector2 p)
        {
            position = p;

            foreach (var previewInstance in previewInstances)
            {
                previewInstance.transform.localPosition = p;
            }
        }

        public void CreatePreview(List<GameObject> prefabs)
        {
            DestroyPreview();
            foreach (var prefab in prefabs)
            {
                #if UNITY_EDITOR
                var preview = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                preview.transform.localPosition = position;
                preview.hideFlags = HideFlags.NotEditable;
                preview.tag = "EditorOnly";
                previewInstances.Add(preview);
                #endif
            }
        }

        public void DestroyPreview()
        {
            foreach (var previewInstance in previewInstances)
            {
                DestroyImmediate(previewInstance);
            }
            previewInstances.Clear();
        }

        public void Paint()
        {
            foreach (var previewInstance in previewInstances)
            {
                previewInstance.transform.localPosition = position;
                previewInstance.hideFlags = HideFlags.None;
                previewInstance.tag = "Untagged";
            }
            previewInstances.Clear();
            
            // var instance = PrefabUtility.InstantiatePrefab(selectedEntry.prefab) as GameObject;
            
            // if (currentBrush.parent != null)
            //     instance.transform.parent = currentBrush.parent;
            // instance.transform.position = currentBrush.transform.position;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette
{
    [CreateAssetMenu(menuName = "Object Palette/Default Brush")]
    public class ScriptableBrushBaseAsset : ScriptableObject, IBrush
    {
        public float minDistributionOffset = 0.0f;
        public float maxDistributionOffset = 1.0f;
        
        [NonSerialized]
        protected Vector2 position;
        
        [NonSerialized]
        protected readonly List<GameObject> previewInstances = new List<GameObject>();

        [NonSerialized]
        private Transform previewParent;

        public virtual void UpdatePosition(Vector2 p)
        {
            position = p;
            if (previewParent != null)
                previewParent.position = p;
        }

        public void CreatePreview(List<GameObject> prefabs)
        {
            DestroyPreview();

            if (previewParent == null)
            {
                var brushPreviewObject = new GameObject("~BrushPreview")
                {
                    hideFlags = HideFlags.NotEditable, 
                    tag = "EditorOnly"
                };

                brushPreviewObject.AddComponent<BrushPreview>();
                
                previewParent = brushPreviewObject.transform;
                previewParent.position = position;
            }

            foreach (var prefab in prefabs)
            {
#if UNITY_EDITOR
                var offset = Vector2.zero; 
                if (prefabs.Count > 1)
                {
                    var len = UnityEngine.Random.Range(minDistributionOffset, maxDistributionOffset);
                    var angle = UnityEngine.Random.Range(0, 360);
                    offset = Quaternion.Euler(0, 0, angle) * new Vector3(len, 0, 0);
                }
                var preview = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, previewParent) as GameObject;
                preview.transform.localPosition = offset;
                previewInstances.Add(preview);
#endif
            }
        }

        public void DestroyPreview()
        {
            if (previewParent != null)
                DestroyImmediate(previewParent.gameObject);
            previewParent = null;
            previewInstances.Clear();
        }

        public void Paint()
        {
            foreach (var previewInstance in previewInstances)
            {
                previewInstance.transform.parent = previewParent.parent;
                #if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo (previewInstance, "Painted");
                #endif
            }
            previewInstances.Clear();
        }
    }
}
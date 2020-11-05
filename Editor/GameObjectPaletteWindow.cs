using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    public class GameObjectPaletteWindow : EditorWindow
    {
        [MenuItem("Window/Object Palette/Palette Window")]
        public static void OpenWindow()
        {
            GetWindow(typeof(GameObjectPaletteWindow), false, "Object Palette");
        }

        private class PaletteEntry
        {
            public string name;
            public GameObject prefab;
            public Texture preview;

            public override bool Equals(object obj)
            {
                if (obj is PaletteEntry e)
                    return e.prefab.Equals(prefab);
                return base.Equals(obj);
            }
        }
        

        private readonly List<PaletteEntry> entries = new List<PaletteEntry>();
        private List<ScriptableBrushBaseAsset> availableBrushes = new List<ScriptableBrushBaseAsset>();

        private Vector2 verticalScroll;
        private Tool previousTool;

        private IBrush brush;
        private int selectedBrushIndex;
        private PaletteEntry selectedEntry;

        private void OnEnable()
        {
            ReloadPalette();
            
            SceneView.duringSceneGui += DuringSceneView;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            
            DestroyHangingPreview();
            
            if (brush == null)
                CreateActiveBrush();
        }

        private void DestroyHangingPreview()
        {
            var scenes = EditorSceneManager.sceneCount;
            for (var i = 0; i < scenes; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    var hangingPreview = rootObject.GetComponentsInChildren<BrushPreview>();
                    foreach (var hangingBrush in hangingPreview)
                    {
                        DestroyImmediate(hangingBrush.gameObject);
                    }
                }
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneView;
            EditorSceneManager.sceneOpened -= OnSceneOpened;

            brush?.DestroyPreview();
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                DeselectPaletteEntry();
            }
        }

        private void CreateActiveBrush()
        {
            brush = CreateInstance<ScriptableBrushBaseAsset>();
        }

        private void DuringSceneView(SceneView sceneView)
        {
            if (selectedEntry == null || brush == null)
                return;
            
            var p = Event.current.mousePosition;
            // currentBrush.transform.position = p;
            
            var ray = HandleUtility.GUIPointToWorldRay(p);
            var position = ray.origin;
            position.z = 0;
            brush.UpdatePosition(position);
            
            // currentBrush.transform.position = position;
            // Handles.DrawLine(btn.transform.position + Vector3.up, mousePosition);

            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
            {
                // TODO: this is brush logic...
                // var instance = PrefabUtility.InstantiatePrefab(selectedEntry.prefab) as GameObject;
                
                brush.Paint();
                brush.CreatePreview(new List<GameObject>
                {
                    selectedEntry.prefab
                });
                
                // if (currentBrush.parent != null)
                //     instance.transform.parent = currentBrush.parent;
                // instance.transform.position = currentBrush.transform.position;
                Event.current.Use();
            }
            
            // if (Event.current.rawType == EventType.MouseDrag)
            // {
            //     var instance = PrefabUtility.InstantiatePrefab(currentBrush.prefab) as GameObject;
            //     instance.transform.position = currentBrush.transform.position;
            //     Event.current.Use();
            // }
            
            // if (Event.current.rawType == EventType.MouseUp)
            // {
            //     Event.current.Use();
            // }
        }

        private void OnFocus()
        {
            ReloadPalette();
        }
        
        private void ReloadPalette()
        {
            var objects = AssetDatabaseExt.FindPrefabs<Renderer>(AssetDatabaseExt.FindOptions.None, new []
            {
                "Assets/Palette"
            });

            entries.Clear();
            
            foreach (var obj in objects)
            {
                entries.Add(new PaletteEntry
                {
                    name = obj.name,
                    prefab = obj,
                    preview = AssetPreview.GetAssetPreview(obj)
                });
            }

            availableBrushes = AssetDatabaseExt.FindAssets<ScriptableBrushBaseAsset>();
        }
        
        private void OnGUI()
        {
            if (availableBrushes.Count > 0)
            {
                GUILayout.BeginVertical();
                var options = new List<string>() {"None"};
                options.AddRange(availableBrushes.Select(b => b.name));
                
                EditorGUI.BeginChangeCheck();
                selectedBrushIndex = EditorGUILayout.Popup("Brush", selectedBrushIndex, options.ToArray());
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (selectedBrushIndex == 0)
                        brush = CreateInstance<ScriptableBrushBaseAsset>();
                    else
                        brush = availableBrushes[selectedBrushIndex - 1];
                }
                
                GUILayout.EndVertical();
            }
            
            var buttonSize = new Vector2(140, 140);
            
            GUILayout.BeginVertical();
                
            verticalScroll = GUILayout.BeginScrollView(verticalScroll, false, true, 
                GUIStyle.none, GUI.skin.verticalScrollbar);
            
            GUILayout.BeginHorizontal();

            var current = 0f;

            var fontStyle = new GUIStyle(GUI.skin.GetStyle("PreOverlayLabel"))
            {
                fontSize = 10
            };

            foreach (var entry in entries)
            {
                var previewSize = buttonSize;

                var previewContent = new GUIContent
                {
                    text = entry.name
                };

                var guiStyle = new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fixedWidth = previewSize.x,
                    fixedHeight = previewSize.y
                };

                var isSelected = entry.Equals(selectedEntry);

                if (GUILayout.Button(previewContent, guiStyle))
                {
                    if (isSelected)
                        DeselectPaletteEntry();
                    else
                    {
                        DeselectPaletteEntry();
                        SelectBrushObject(entry);
                    }
                }
                
                var r = GUILayoutUtility.GetLastRect();

                if (entry.preview == null)
                {
                    // If preview was unloaded by unity, regenerate it
                    entry.preview = AssetPreview.GetAssetPreview(entry.prefab);
                }

                GUI.DrawTexture(r, entry.preview, ScaleMode.StretchToFill);
                EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, r.height - 0.0f), entry.name, 
                    fontStyle);

                if (isSelected)
                {
                    EditorGUI.DrawRect(r, new Color(0, 0, 0.5f, 0.15f));
                }
                
                current += buttonSize.x;

                if (current >= position.width - buttonSize.x)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    current = 0;
                }

            }
                
            GUILayout.EndHorizontal();
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DeselectPaletteEntry()
        {
            brush.DestroyPreview();
            
            selectedEntry = null;
            UnityEditor.Tools.current = previousTool;
        }

        private void SelectBrushObject(PaletteEntry entry)
        {
            selectedEntry = entry;

            brush.CreatePreview(new List<GameObject>
            {
                selectedEntry.prefab
            });

            previousTool = UnityEditor.Tools.current;
            UnityEditor.Tools.current = Tool.None;
        }
    }
}

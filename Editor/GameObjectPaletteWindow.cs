using System.Collections.Generic;
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
        }
        

        private readonly List<PaletteEntry> entries = new List<PaletteEntry>();

        private Vector2 verticalScroll;
        private Tool previousTool;

        private GameObjectBrush currentBrush;
        private PaletteEntry selectedEntry;

        private void OnEnable()
        {
            ReloadPalette();
            SceneView.duringSceneGui += DuringSceneView;
            EditorSceneManager.sceneClosed += OnSceneClosed;
            EditorSceneManager.sceneOpened += OnSceneOpened;

            DestroyPreviousBrushes();
            // CreateActiveBrush();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneView;
            EditorSceneManager.sceneClosed -= OnSceneClosed;
            EditorSceneManager.sceneOpened -= OnSceneOpened;

            DestroyPreviousBrushes();
        }
        
        private void OnSceneClosed(Scene scene)
        {
            
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                DeselectPaletteEntry();
                DestroyPreviousBrushes();
                // CreateActiveBrush();
            }
        }

        private void CreateActiveBrush()
        {
            var brushObject = new GameObject("~BrushObject")
            {
                hideFlags = HideFlags.NotEditable, tag = "EditorOnly"
            };
            currentBrush = brushObject.AddComponent<GameObjectBrush>();

            var root = FindObjectOfType<BrushRoot>();
            if (root != null)
            {
                currentBrush.parent = root.transform;
            }
        }

        private void DestroyActiveBrush()
        {
            if (currentBrush != null)
            {
                DestroyImmediate(currentBrush.gameObject);
                currentBrush = null;
            }
        }

        private void DestroyPreviousBrushes()
        {
            var scenes = EditorSceneManager.sceneCount;
            for (int i = 0; i < scenes; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                var rootObjects = scene.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    var hangingBrushes = rootObject.GetComponentsInChildren<GameObjectBrush>();
                    foreach (var hangingBrush in hangingBrushes)
                    {
                        DestroyImmediate(hangingBrush.gameObject);
                    }
                }
            }
            currentBrush = null;
        }

        private void DuringSceneView(SceneView sceneView)
        {
            if (selectedEntry == null || currentBrush == null)
                return;
            
            var p = Event.current.mousePosition;
            // currentBrush.transform.position = p;
            
            var ray = HandleUtility.GUIPointToWorldRay(p);
            var position = ray.origin;
            position.z = 0;
            currentBrush.transform.position = position;
            // Handles.DrawLine(btn.transform.position + Vector3.up, mousePosition);

            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 0)
            {
                // TODO: this is brush logic...
                var instance = PrefabUtility.InstantiatePrefab(selectedEntry.prefab) as GameObject;
                if (currentBrush.parent != null)
                    instance.transform.parent = currentBrush.parent;
                instance.transform.position = currentBrush.transform.position;
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
            // CreateActiveBrush();
        }

        private void OnLostFocus()
        {
            // Unselect brush tool
            // DeselectBrushObject();
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
        }
        
        private void OnGUI()
        {
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

                var isSelected = selectedEntry == entry;

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
            // currentBrush.prefab = null;
            //
            // // var childCount = currentBrush.transform.childCount;
            // // for (var i = 0; i < childCount; i++)
            // // {
            // //     var c = currentBrush.transform.GetChild(i);
            // //     DestroyImmediate(c.gameObject);
            // // }
            //
            // if (currentBrush.preview != null)
            //     DestroyImmediate(currentBrush.preview);
            // currentBrush.preview = null;

            DestroyActiveBrush();
            selectedEntry = null;
            UnityEditor.Tools.current = previousTool;
        }

        private void SelectBrushObject(PaletteEntry entry)
        {
            selectedEntry = entry;
            
            CreateActiveBrush();
            
            currentBrush.preview = Instantiate(entry.prefab, currentBrush.transform);
            currentBrush.preview.transform.localPosition= new Vector3();
            currentBrush.preview.hideFlags = HideFlags.NotEditable;
            currentBrush.preview.tag = "EditorOnly";

            previousTool = UnityEditor.Tools.current;
            UnityEditor.Tools.current = Tool.None;
        }
    }
}

using System;
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
            var window = GetWindow(typeof(GameObjectPaletteWindow), false, "Object Palette");
            window.minSize = new Vector2(300, 300);
        }

        private class PaletteEntry : IEquatable<PaletteEntry>
        {
            public string name;
            public GameObject prefab;
            public Texture preview;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PaletteEntry) obj);
            }

            public bool Equals(PaletteEntry other)
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

        private class PaletteSelection
        {
            public List<PaletteEntry> selection = new List<PaletteEntry>();

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

            public bool Contains(PaletteEntry p)
            {
                return selection.Contains(p);
            }

            public void Add(PaletteEntry p)
            {
                selection.Add(p);
            }
        }

        private enum PaletteTool
        {
            Paint,
            Erase
        }
        

        private readonly List<PaletteEntry> entries = new List<PaletteEntry>();
        private List<ScriptableBrushBaseAsset> availableBrushes = new List<ScriptableBrushBaseAsset>();

        private Vector2 verticalScroll;
        private Tool previousTool;

        private IBrush brush;
        private int selectedBrushIndex;
        
        private PaletteSelection selection = new PaletteSelection();

        // TODO: configurable through scriptable object...
        private static readonly float buttonPreviewMinSize = 50;
        private static readonly float buttonPreviewMaxSize = 150;

        private float currentButtonSize;

        private PaletteTool currentTool = PaletteTool.Paint;

        private void OnEnable()
        {
            ReloadPalette();
            
            // SceneView.duringSceneGui += DuringSceneView;
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
            // SceneView.duringSceneGui -= DuringSceneView;
            EditorSceneManager.sceneOpened -= OnSceneOpened;

            brush?.DestroyPreview();
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (mode == OpenSceneMode.Single)
            {
                UnselectPalette();
            }
        }

        private void CreateActiveBrush()
        {
            brush = CreateInstance<ScriptableBrushBaseAsset>();
        }

        private void OnSceneViewGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            
            var r = sceneView.camera.pixelRect;
            var toolsRect = new Rect(r.xMax - 100, r.yMax - 50 , 75, 50);
            GUILayout.BeginArea(toolsRect);
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            var eraseToggle = GUILayout.Toggle(currentTool == PaletteTool.Erase, "Erase", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                if (eraseToggle)
                {
                    currentTool = PaletteTool.Erase;
                    UnselectUnityTool();
                    brush?.DestroyPreview();
                }
                else
                {
                    currentTool = PaletteTool.Paint;
                    RestoreUnityTool();
                    if (brush != null && !selection.IsEmpty)
                        brush.CreatePreview(selection.SelectedPrefabs);  
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            Handles.EndGUI();
            
            if (Event.current.rawType == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftAlt)
            {
                currentTool = PaletteTool.Erase;
                UnselectUnityTool();
            
                brush?.DestroyPreview();
            }
            
            if (Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.LeftAlt)
            {
                currentTool = PaletteTool.Paint;
                RestoreUnityTool();
            
                if (brush != null && !selection.IsEmpty)
                {
                    brush.CreatePreview(selection.SelectedPrefabs);
                }
            }
            
            if (Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                UnselectPalette();
                Repaint();
            }

            if (Event.current.rawType == EventType.MouseDown && Event.current.button == 1)
            {
                UnselectPalette();
                Repaint();
            }
            
            // if (selectedEntry == null || brush == null)
            //     return;
            
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
                if (currentTool == PaletteTool.Paint)
                {
                    if (brush != null && !selection.IsEmpty)
                    {
                        brush.Paint();
                        brush.CreatePreview(selection.SelectedPrefabs);
                        Event.current.Use();
                    }
                }
                else if (currentTool == PaletteTool.Erase)
                {
                    var go = HandleUtility.PickGameObject(p, true);
                    
                    if (go != null)
                    {
                        Debug.Log($"Erase: {go.name}");
                    }
                    
                    
                    // Custom
                    // d_color_picker
                    
                    // check if it is a prefab instance since now we instantiate prefabs (but that could change)
                    // TODO: check if object in root, etc.
                    if (go != null && PrefabUtility.GetPrefabInstanceStatus(go) != PrefabInstanceStatus.NotAPrefab)
                    {
                        Undo.DestroyObjectImmediate(go);
                        Event.current.Use();
                    }
                }
            }

        }
        
        private void OnBecameVisible()
        {
            SceneView.duringSceneGui += OnSceneViewGui;
            // Regenerate brush preview if window became visible, if some selection was active
            if (!selection.IsEmpty)
            {
                brush?.CreatePreview(selection.SelectedPrefabs);
            }
        }

        private void OnBecameInvisible()
        {
            SceneView.duringSceneGui -= OnSceneViewGui;
            brush?.DestroyPreview();
        }

        private void OnFocus()
        {
            ReloadPalette();
        }

        private void ReloadPalette()
        {
            var objects = AssetDatabaseExt.FindPrefabs<Renderer>(AssetDatabaseExt.FindOptions.ConsiderChildren, new []
            {
                "Assets"
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
            if (Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                UnselectPalette();
            }
            
            GUILayout.BeginVertical();

            if (availableBrushes.Count > 0)
            {
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
                
            }
            
            var buttonSize = new Vector2(currentButtonSize, currentButtonSize);
            
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
                
            verticalScroll = GUILayout.BeginScrollView(verticalScroll, false, true, 
                GUIStyle.none, GUI.skin.verticalScrollbar);
            
            GUILayout.BeginHorizontal();

            var current = 0f;

            var fontStyle = new GUIStyle(GUI.skin.GetStyle("PreOverlayLabel"))
            {
                fontSize = 10
            };

            var multiselection = Event.current.shift;

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

                var isSelected = selection.Contains(entry);

                if (GUILayout.Button(previewContent, guiStyle))
                {
                    if (multiselection)
                    {
                        SelectBrushObject(entry);
                    }
                    else
                    {
                        if (isSelected)
                            UnselectPalette();
                        else
                        {
                            UnselectPalette();
                            SelectBrushObject(entry);
                        }    
                    }
                    
                }
                
                var r = GUILayoutUtility.GetLastRect();

                if (entry.preview == null)
                {
                    // If preview was unloaded by unity, regenerate it
                    entry.preview = AssetPreview.GetAssetPreview(entry.prefab);
                }

                // var smallerRect = new Rect(r.x, r.y, r.width * 0.5f, r.height * 0.5f);
                if (entry.preview != null)
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
            
            currentButtonSize = EditorGUILayout.Slider("Preview Size", currentButtonSize, 
                buttonPreviewMinSize, buttonPreviewMaxSize);
            
            GUILayout.EndVertical();
        }

        private void UnselectPalette()
        {
            brush.DestroyPreview();
            selection.Clear();
            RestoreUnityTool();
        }

        private void SelectBrushObject(PaletteEntry entry)
        {
            selection.Add(entry);
            brush.CreatePreview(selection.SelectedPrefabs);
            UnselectUnityTool();
        }

        private void UnselectUnityTool()
        {
            if (UnityEditor.Tools.current != Tool.None)
            {
                previousTool = UnityEditor.Tools.current;
                Debug.Log($"Storing selected tool: {previousTool}");
            }
            UnityEditor.Tools.current = Tool.None;
        }

        public void RestoreUnityTool()
        {
            if (previousTool != Tool.None)
            {
                UnityEditor.Tools.current = previousTool;
                Debug.Log($"Restoring selected tool: {previousTool}");
            }
            previousTool = Tool.None;
        }
    }
}

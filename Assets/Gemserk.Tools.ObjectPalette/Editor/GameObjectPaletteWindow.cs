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

        private readonly List<PaletteObject> entries = new List<PaletteObject>();
        private List<ScriptableBrushBaseAsset> availableBrushes = new List<ScriptableBrushBaseAsset>();

        private Vector2 verticalScroll;
        private Tool previousTool;

        private int selectedBrushIndex;
        
        public static bool windowVisible = false;

        // TODO: configurable through scriptable object...
        private static readonly float buttonPreviewMinSize = 50;
        private static readonly float buttonPreviewMaxSize = 150;

        private float currentButtonSize;

        private void OnEnable()
        {
            ReloadPalette();
            
            // SceneView.duringSceneGui += DuringSceneView;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            
            DestroyHangingPreview();
            
            if (PaletteCommon.brush == null)
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
            PaletteCommon.brush?.DestroyPreview();
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
            PaletteCommon.brush = CreateInstance<ScriptableBrushBaseAsset>();
        }

        private void OnSceneViewGui(SceneView sceneView)
        {
            Handles.BeginGUI();
            
            var r = sceneView.camera.pixelRect;
            var toolsRect = new Rect(r.xMax - 100, r.yMax - 50 , 75, 50);
            GUILayout.BeginArea(toolsRect);
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            var eraseToggle = GUILayout.Toggle(PaletteCommon.mode == PaletteToolMode.Erase, "Erase", "Button");
            if (EditorGUI.EndChangeCheck())
            {
                if (eraseToggle)
                {
                    PaletteCommon.mode = PaletteToolMode.Erase;
                    UnselectUnityTool();
                    PaletteCommon.brush?.DestroyPreview();
                }
                else
                {
                    PaletteCommon.mode = PaletteToolMode.Paint;
                    RestoreUnityTool();
                    if (PaletteCommon.brush != null && !PaletteCommon.selection.IsEmpty)
                        PaletteCommon.brush.CreatePreview(PaletteCommon.selection.SelectedPrefabs);  
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            Handles.EndGUI();

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
            PaletteCommon.brush.UpdatePosition(position);
            
            // currentBrush.transform.position = position;
            // Handles.DrawLine(btn.transform.position + Vector3.up, mousePosition);

            var rawEvent = Event.current.rawType;
            // var isPaintEvent = rawEvent == EventType.MouseDown || rawEvent == EventType.MouseDrag || rawEvent == EventType.MouseMove;
            

        }
        
        private void OnBecameVisible()
        {
            windowVisible = true;

            SceneView.duringSceneGui += OnSceneViewGui;
            // Regenerate brush preview if window became visible, if some selection was active
            if (!PaletteCommon.selection.IsEmpty)
            {
                PaletteCommon.brush?.CreatePreview(PaletteCommon.selection.SelectedPrefabs);
            }
        }

        private void OnBecameInvisible()
        {
            windowVisible = false;
            
            SceneView.duringSceneGui -= OnSceneViewGui;
            PaletteCommon.brush?.DestroyPreview();
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
                entries.Add(new PaletteObject
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
                        PaletteCommon.brush = CreateInstance<ScriptableBrushBaseAsset>();
                    else
                        PaletteCommon.brush = availableBrushes[selectedBrushIndex - 1];
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

                var isSelected = PaletteCommon.selection.Contains(entry);

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
            PaletteCommon.brush.DestroyPreview();
            PaletteCommon.selection.Clear();
            RestoreUnityTool();
        }

        private void SelectBrushObject(PaletteObject o)
        {
            PaletteCommon.selection.Add(o);
            PaletteCommon.brush.CreatePreview(PaletteCommon.selection.SelectedPrefabs);
            UnselectUnityTool();
        }

        private void UnselectUnityTool()
        {
            var type = UnityEditor.EditorTools.EditorTools.activeToolType;
            if (type != typeof(PalettePaintTool))
                UnityEditor.EditorTools.EditorTools.SetActiveTool<PalettePaintTool>();
        }

        public void RestoreUnityTool()
        {
            var type = UnityEditor.EditorTools.EditorTools.activeToolType;
            if (type == typeof(PalettePaintTool))
                UnityEditor.EditorTools.EditorTools.RestorePreviousTool();
        }
    }
}

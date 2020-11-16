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
        public class SelectedPalette
        {
            public List<PaletteObject> cachedEntries;
        }
        
        [MenuItem("Window/Object Palette/Palette Window")]
        public static void OpenWindow()
        {
            var window = GetWindow(typeof(GameObjectPaletteWindow), false, "Object Palette");
            window.minSize = new Vector2(300, 300);
        }

        private List<ObjectPaletteBaseAsset> availablePalettes;
        private SelectedPalette _selectedSelectedPalette = new SelectedPalette();
        
        private List<ScriptableBrushBaseAsset> availableBrushes = new List<ScriptableBrushBaseAsset>();

        private Vector2 verticalScroll;
        private Tool previousTool;

        private int selectedBrushIndex;
        private int selectedPaletteIndex;
        
        public static bool windowVisible = false;

        // TODO: configurable through scriptable object...
        private static readonly float buttonPreviewMinSize = 50;
        private static readonly float buttonPreviewMaxSize = 150;

        private float currentButtonSize;

        [SerializeField]
        private ScriptableBrushBaseAsset defaultBrush = null;

        private void OnEnable()
        {
            ReloadPalettesAndBrushes();
            
            // SceneView.duringSceneGui += DuringSceneView;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            
            DestroyHangingPreview();

            if (PaletteCommon.brush == null)
                PaletteCommon.brush = defaultBrush;
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
                        PaletteCommon.brush.CreatePreview(PaletteCommon.selection);  
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
        }
        
        private void OnBecameVisible()
        {
            windowVisible = true;

            SceneView.beforeSceneGui += OnBeforeSceneGui;
            SceneView.duringSceneGui += OnSceneViewGui;
            
            // Regenerate brush preview if window became visible, if some selection was active
            if (!PaletteCommon.selection.IsEmpty)
            {
                PaletteCommon.brush?.CreatePreview(PaletteCommon.selection.selection);
            }
        }

        private void OnBeforeSceneGui(SceneView view)
        {
            if (Event.current.type == EventType.ScrollWheel && Event.current.control)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                PaletteCommon.brush.CreatePreview(PaletteCommon.selection.selection);
                Event.current.Use();
            }
        }

        private void OnBecameInvisible()
        {
            windowVisible = false;
            
            SceneView.duringSceneGui -= OnSceneViewGui;
            SceneView.beforeSceneGui -= OnBeforeSceneGui;
            PaletteCommon.brush?.DestroyPreview();
        }

        private void OnFocus()
        {
            ReloadPalettesAndBrushes();
        }

        private void ReloadPalettesAndBrushes()
        {
            availablePalettes = AssetDatabaseExt.FindAssets<ObjectPaletteBaseAsset>();
            availableBrushes = AssetDatabaseExt.FindAssets<ScriptableBrushBaseAsset>();
        }

        private void ReloadSelectedPalette()
        {
            var palette = availablePalettes[selectedPaletteIndex];

            // var objects = palette.CreatePaletteObjects();
            // var objects = AssetDatabaseExt.FindPrefabs<Renderer>(AssetDatabaseExt.FindOptions.ConsiderChildren, new []
            // {
            //     "Assets"
            // });

            _selectedSelectedPalette.cachedEntries = palette.CreatePaletteObjects();
        }

        private void OnGUI()
        {
            if (Event.current.rawType == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                UnselectPalette();
            }
            
            DrawBrushList();
            DrawPaletteList();

            if (availablePalettes.Count == 0)
                return;

            ReloadSelectedPalette();
            
            var buttonSize = new Vector2(currentButtonSize, currentButtonSize);

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

            foreach (var entry in _selectedSelectedPalette.cachedEntries)
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
        
        private void DrawPaletteList()
        {
            GUILayout.BeginVertical();

            if (availablePalettes.Count > 0)
            {
                var options = new List<string>();
                options.AddRange(availablePalettes.Select(b => b.name));

                EditorGUI.BeginChangeCheck();
                selectedPaletteIndex = EditorGUILayout.Popup("Palette", selectedPaletteIndex, options.ToArray());
                
                if (EditorGUI.EndChangeCheck())
                {
                    // Nullify cached entires to force regenerate selected palette.
                    _selectedSelectedPalette.cachedEntries = null;
                }
            }
            else
            {
                GUILayout.Label("No palettes found");
            }

            GUILayout.EndVertical();
        }

        private void DrawBrushList()
        {
            GUILayout.BeginVertical();

            if (availableBrushes.Count > 0)
            {
                var options = new List<string>();
                options.AddRange(availableBrushes.Select(b => b.name));

                selectedBrushIndex = availableBrushes.IndexOf(PaletteCommon.brush as ScriptableBrushBaseAsset);
                
                EditorGUI.BeginChangeCheck();
                selectedBrushIndex = EditorGUILayout.Popup("Brush", selectedBrushIndex, options.ToArray());
                
                if (EditorGUI.EndChangeCheck())
                {
                    PaletteCommon.brush = availableBrushes[selectedBrushIndex];
                }
            }

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
            PaletteCommon.brush.CreatePreview(PaletteCommon.selection.selection);
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

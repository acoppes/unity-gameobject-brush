using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Gemserk.Tools.ObjectPalette.Editor
{
    [EditorTool("Platform Tool")]
    public class PalettePaintTool : EditorTool
    {
        // Serialize this value to set a default value in the Inspector.
        [SerializeField]
        private Texture2D m_ToolIcon = null;

        private GUIContent m_IconContent;

        private void OnEnable()
        {
            m_IconContent = new GUIContent
            {
                image = m_ToolIcon,
                text = "GameObject Palette Tool",
                tooltip = "GameObject Palette Tool"
            };
        }

        public override GUIContent toolbarIcon => m_IconContent;

        // This is called for each window that your tool is active in. Put the functionality of your tool here.
        public override void OnToolGUI(EditorWindow window)
        {
            var p = Event.current.mousePosition;
            var rawEvent = Event.current.rawType;
            
            if (Event.current.rawType == EventType.KeyDown && Event.current.keyCode == KeyCode.E)
            {
                if (PaletteCommon.mode == PaletteToolMode.Paint)
                {
                    PaletteCommon.mode = PaletteToolMode.Erase;
                    PaletteCommon.brush?.DestroyPreview();
                    Event.current.Use();
                }
                else
                {
                    PaletteCommon.mode = PaletteToolMode.Paint;
                    if (PaletteCommon.brush != null && !PaletteCommon.selection.IsEmpty)
                        PaletteCommon.brush.CreatePreview(PaletteCommon.selection.SelectedPrefabs);
                    Event.current.Use();
                }
            }
            

            // This is used somehow to avoid default scene view controls to consume Event MouseDrag so we 
            // can use it to paint.
            if (Event.current.type == EventType.Layout) 
            {
                HandleUtility.AddDefaultControl(0);
            }
            
            // Debug.Log($"{Event.current.rawType}, {Event.current.type}");
            
            // TODO: repeat delay/distance.

            var painting = rawEvent == EventType.MouseDown && Event.current.button == 0;
            painting = painting || rawEvent == EventType.MouseDrag;
            
            if (painting)
            {
                if (PaletteCommon.mode == PaletteToolMode.Paint)
                {
                    if (PaletteCommon.brush != null && !PaletteCommon.selection.IsEmpty)
                    {
                        PaletteCommon.brush.Paint();
                        PaletteCommon.brush.CreatePreview(PaletteCommon.selection.SelectedPrefabs);
                        // Event.current.Use();
                    }
                }
                else if (PaletteCommon.mode == PaletteToolMode.Erase)
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
                        // Event.current.Use();
                    }
                }
            }
        }
    }
}
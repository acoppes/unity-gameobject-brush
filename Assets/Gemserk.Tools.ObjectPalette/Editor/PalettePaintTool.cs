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

            // var isPaintEvent = rawEvent == EventType.MouseDown || rawEvent == EventType.MouseDrag || rawEvent == EventType.MouseMove;
            
            // TODO: repeat delay/distance.

            if (rawEvent == EventType.MouseDown && Event.current.button == 0)
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
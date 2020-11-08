using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

// Tagging a class with the EditorTool attribute and no target type registers a global tool. Global tools are valid for any selection, and are accessible through the top left toolbar in the editor.
namespace Gemserk.Tools.ObjectPalette.Editor
{
    [EditorTool("Platform Tool")]
    public class PalettePaintTool : EditorTool
    {
        // Serialize this value to set a default value in the Inspector.
        [SerializeField]
        private Texture2D m_ToolIcon;

        private GUIContent m_IconContent;

        void OnEnable()
        {
            m_IconContent = new GUIContent()
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
            // TODO: paint using this tool, not the onscene gui
            
            // Event.current.Use();
            //
            // EditorGUI.BeginChangeCheck();
            //
            // Vector3 position = UnityEditor.Tools.handlePosition;
            //
            // using (new Handles.DrawingScope(Color.green))
            // {
            //     position = Handles.Slider(position, Vector3.right);
            // }
            //
            // if (EditorGUI.EndChangeCheck())
            // {
            //     Vector3 delta = position - UnityEditor.Tools.handlePosition;
            //
            //     Undo.RecordObjects(Selection.transforms, "Move Platform");
            //
            //     foreach (var transform in Selection.transforms)
            //         transform.position += delta;
            // }
        }
    }
}
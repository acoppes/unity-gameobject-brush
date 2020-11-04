using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        private void OnEnable()
        {
            ReloadPalette();
        }

        private void OnFocus()
        {
            ReloadPalette();
        }

        private void ReloadPalette()
        {
            var objects = AssetDatabaseExt.FindPrefabs<Renderer>();

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

                if (GUILayout.Button(previewContent, guiStyle))
                {
                    
                }
                
                var r = GUILayoutUtility.GetLastRect();

                GUI.DrawTexture(r, entry.preview, ScaleMode.StretchToFill);

                EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, r.height - 0.0f), entry.name, 
                    fontStyle);
                
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
    }
}

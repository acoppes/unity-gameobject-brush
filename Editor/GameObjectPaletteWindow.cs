﻿using System;
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

        private GameObjectBrush currentBrush;

        private void OnEnable()
        {
            ReloadPalette();
            SceneView.duringSceneGui += DuringSceneView;

            DestroyPreviousBrushes();
            CreateActiveBrush();
        }

        private void CreateActiveBrush()
        {
            var brushObject = new GameObject("~BrushObject")
            {
                hideFlags = HideFlags.DontSave
            };
            currentBrush = brushObject.AddComponent<GameObjectBrush>();
        }

        private void DestroyPreviousBrushes()
        {
            var previousBrushes = FindObjectsOfType<GameObjectBrush>();
            foreach (var previousBrush in previousBrushes)
            {
                Destroy(previousBrush.gameObject);
            }
            currentBrush = null;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneView;
        }

        private void DuringSceneView(SceneView sceneView)
        {
            if (currentBrush == null || currentBrush.prefab == null)
                return;
            Debug.Log("During Scene View");
        }

        private void OnFocus()
        {
            ReloadPalette();
        }

        private void OnLostFocus()
        {
            // Unselect brush tool
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

                var isSelected = currentBrush.prefab == entry.prefab;

                if (GUILayout.Button(previewContent, guiStyle))
                {
                    if (isSelected)
                        DeselectBrushObject();
                    else 
                        SelectBrushObject(entry.prefab);
                }
                
                var r = GUILayoutUtility.GetLastRect();

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

        private void DeselectBrushObject()
        {
            currentBrush.prefab = null;
            // TODO: destroy preview object?
        }

        private void SelectBrushObject(GameObject prefab)
        {
            currentBrush.prefab = prefab;
            // TODO: create brush object...
        }
    }
}
